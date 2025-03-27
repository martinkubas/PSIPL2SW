using SharpPcap.LibPcap;
using SharpPcap;
using System;
using PacketDotNet;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;
using System.IO.Hashing;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using PacketDotNet.Ieee80211;
using System.Diagnostics;
using static log4net.Appender.FileAppender;
using Projekt;

public class PacketForwarder
{
    private List<LibPcapLiveDevice> interfaces;
    private List<InterfaceStatistics> interfaceStats;
    private MACTable macAddressTable;

    private ConcurrentQueue<ulong> receivedPackets = new ConcurrentQueue<ulong>();
    private ConcurrentDictionary<ulong, byte> receivedPacketsHashSet = new ConcurrentDictionary<ulong, byte>();
    private readonly object _lock = new object();

    public PacketForwarder(List<LibPcapLiveDevice> interfaces)
    {
        this.interfaces = interfaces;
        this.interfaceStats = new List<InterfaceStatistics>();
        this.macAddressTable = new MACTable();

        foreach (var _ in interfaces)
        {
            interfaceStats.Add(new InterfaceStatistics());
        }
    }

    public void Start()
    {
        foreach (var device in interfaces)
        {

            device.Open(new DeviceConfiguration
            {
                ReadTimeout = 50,
                BufferSize = 4 * 1024 * 1024,

            });
            device.OnPacketArrival += OnPacketArrival;
            device.StartCapture();
        }
    }

    public void Stop()
    {
        foreach (var device in interfaces)
        {
            if (device != null && device.Started)
            {
                device.StopCapture();
                device.Close();
            }
        }

        receivedPackets = new ConcurrentQueue<ulong>();
    }

    private void OnPacketArrival(object sender, PacketCapture e)
    {

        var device = sender as LibPcapLiveDevice;

        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);


        int incomingInterfaceIndex = interfaces.IndexOf(device);

        ulong packetHash = ComputePacketHash(rawPacket.Data);
        if (checkIfWasReceivedBefore(packetHash))
        {
            return;
        }

        queueControl(packetHash);

        checkMacAndSend(packet, incomingInterfaceIndex); 
        updateMacTable(packet, incomingInterfaceIndex);

    }

    private void updateMacTable(Packet packet, int incomingInterfaceIndex)
    {
        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            var sourceMac = ethernetPacket.SourceHardwareAddress;
            macAddressTable.AddOrUpdate(sourceMac, incomingInterfaceIndex);

        }
    }

    private void checkMacAndSend(Packet packet, int incomingInterfaceIndex)
    {
        lock (_lock)
        {
            var ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket == null) return;

            var srcMac = ethernetPacket.SourceHardwareAddress;
            var dstMac = ethernetPacket.DestinationHardwareAddress;

            // Always handle broadcast/multicast/ARP
            if (dstMac.IsBroadcast() || dstMac.IsMulticast() || packet.Extract<ArpPacket>() != null)
            {
                sendBroadcast(packet, incomingInterfaceIndex);
                return;
            }

            // Update source MAC mapping (always)
            macAddressTable.AddOrUpdate(srcMac, incomingInterfaceIndex);

            // Get destination interface
            int dstInterface = macAddressTable.GetInterface(dstMac);

            // Decision making
            if (dstInterface == -1)
            {
                // Unknown destination - broadcast
                sendBroadcast(packet, incomingInterfaceIndex);
            }
            else if (dstInterface == incomingInterfaceIndex)
            {
                // Destination is on same interface as incoming - drop
                Console.WriteLine($"Dropped: Destination on same interface (src={srcMac}, dst={dstMac})");
            }
            else
            {
                // Valid forwarding case
                Console.WriteLine($"Forwarding: {srcMac}->{dstMac} from {incomingInterfaceIndex} to {dstInterface}");
                sendUnicast(packet, dstInterface);
            }
        }
    }

    private void sendBroadcast(Packet packet, int incomingInterfaceIndex)
    {
        InterfaceStatistics stats = interfaceStats[incomingInterfaceIndex];
        stats.AnalyzePacket(packet, true);
        stats.IncrementTotalIn();

        for (int i = 0; i < interfaces.Count; i++)
        {
            if (i == incomingInterfaceIndex) continue;

            try
            {
                interfaces[i].SendPacket(packet.Bytes);
                interfaceStats[i].AnalyzePacket(packet, false);
                interfaceStats[i].IncrementTotalOut();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error forwarding packet: {ex.Message}");
            }
        }
    }

    private void sendUnicast(Packet packet, int destinationInterfaceIndex)
    {
        Console.WriteLine("Sent unicast packet to: " + destinationInterfaceIndex);
        sendPacket(interfaces[destinationInterfaceIndex], packet, interfaceStats[destinationInterfaceIndex]);
    }

    private void sendPacket(LibPcapLiveDevice outgoingInterface, Packet packet, InterfaceStatistics outStats)
    {
        try
        {
            outgoingInterface.SendPacket(packet.Bytes);

            outStats.AnalyzePacket(packet, false);
            outStats.IncrementTotalOut();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error forwarding packet: {ex.Message}");
        }
    }

    private bool checkIfWasReceivedBefore(ulong packetHash)
    {
        return receivedPacketsHashSet.ContainsKey(packetHash);
    }

    private void queueControl(ulong packetHash)
    {

        if (receivedPackets.Count >= 100)
        {
            if (receivedPackets.TryDequeue(out ulong oldestHash))
            {
                receivedPacketsHashSet.TryRemove(oldestHash, out _);
            }
        }


        receivedPackets.Enqueue(packetHash);
        receivedPacketsHashSet[packetHash] = 0;
    }

    private ulong ComputePacketHash(byte[] packetData)
    {
        return XxHash64.HashToUInt64(packetData);
    }

    public InterfaceStatistics GetStatsForInterface(int index)
    {
        if (index >= 0 && index < interfaceStats.Count)
        {
            return interfaceStats[index];
        }
        throw new ArgumentOutOfRangeException(nameof(index), "Invalid interface index.");
    }
    public MACTable GetMacAddressTable()
    {
        return this.macAddressTable;    
    }

}