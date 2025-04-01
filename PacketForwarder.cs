﻿using Projekt;
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
using System.Threading;

public class PacketForwarder
{
    private List<LibPcapLiveDevice> interfaces;
    private List<InterfaceStatistics> interfaceStats;
    private MACTable macAddressTable;

    private ConcurrentQueue<string> receivedPackets = new ConcurrentQueue<string>();
    private ConcurrentDictionary<string, byte> receivedPacketsHashSet = new ConcurrentDictionary<string, byte>();

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
                Mode = DeviceModes.Promiscuous,
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

        receivedPackets = new ConcurrentQueue<string>();
        macAddressTable.Clear();
    }
      
    private void OnPacketArrival(object sender, PacketCapture e)
    {
        var device = sender as LibPcapLiveDevice;
        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
        string packetHash = ComputePacketHash(rawPacket.Data);

        if (checkIfWasReceivedBefore(packetHash))
        {
            return;
        }

        queueControl(packetHash);

        int incomingInterfaceIndex = interfaces.IndexOf(device);

        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            var sourceMac = ethernetPacket.SourceHardwareAddress;
            macAddressTable.AddOrUpdate(sourceMac, incomingInterfaceIndex);
        }

        checkMacAndSend(packet, incomingInterfaceIndex);

    }

    private void checkMacAndSend(Packet packet, int incomingInterfaceIndex)
    {
        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            var destinationMac = ethernetPacket.DestinationHardwareAddress;

            if (destinationMac == PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF") ||
                (destinationMac.GetAddressBytes()[0] & 0x01) == 0x01)
            {
                sendBroadcast(packet, incomingInterfaceIndex);
                return;
            }

            int destinationInterfaceIndex = macAddressTable.GetInterface(destinationMac);
            if (destinationInterfaceIndex == -1)
            {
                sendBroadcast(packet, incomingInterfaceIndex);
            }
            else if (destinationInterfaceIndex != incomingInterfaceIndex)
            {
                interfaceStats[incomingInterfaceIndex].AnalyzePacket(packet, true);
                interfaceStats[incomingInterfaceIndex].IncrementTotalIn();
                sendUnicast(packet, destinationInterfaceIndex);
            }
        }
    }

    private void sendBroadcast(Packet packet, int incomingInterfaceIndex)
    {
        interfaceStats[incomingInterfaceIndex].AnalyzePacket(packet, true);
        interfaceStats[incomingInterfaceIndex].IncrementTotalIn();

        for (int i = 0; i < interfaces.Count; i++)
        {
            if (i == incomingInterfaceIndex) continue;

            sendPacket(interfaces[i], packet, interfaceStats[i]);
            interfaceStats[i].AnalyzePacket(packet, false);
            interfaceStats[i].IncrementTotalOut();
        }
    }

    private void sendUnicast(Packet packet, int destinationInterfaceIndex)
    {
        interfaceStats[destinationInterfaceIndex].AnalyzePacket(packet, false);
        interfaceStats[destinationInterfaceIndex].IncrementTotalOut();
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
            Console.WriteLine("Cannot Send Packet!" + ex.Message);
        }
    }

    private bool checkIfWasReceivedBefore(string packetHash)
    {
        return receivedPacketsHashSet.ContainsKey(packetHash);
    }

    private void queueControl(string packetHash)
    {
        if (receivedPackets.Count >= 100)
        {
            if (receivedPackets.TryDequeue(out string oldestHash))
            {
                receivedPacketsHashSet.TryRemove(oldestHash, out _);
            }
        }

        receivedPackets.Enqueue(packetHash);
        receivedPacketsHashSet[packetHash] = 0;
    }

    private string ComputePacketHash(byte[] packetData)
    {
        var hashBytes = XxHash64.Hash(packetData);
        return BitConverter.ToString(hashBytes).Replace("-", "");
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