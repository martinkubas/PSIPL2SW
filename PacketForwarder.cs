using Projekt;
using SharpPcap.LibPcap;
using SharpPcap;
using System;
using PacketDotNet;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;
using System.IO.Hashing;

public class PacketForwarder
{
    private LibPcapLiveDevice interface1;
    private LibPcapLiveDevice interface2;
    private InterfaceStatistics statsInterface1;
    private InterfaceStatistics statsInterface2;

    private Hashtable receivedPackets = new Hashtable();
    public PacketForwarder(LibPcapLiveDevice interface1, LibPcapLiveDevice interface2)
    {
        this.interface1 = interface1;
        this.interface2 = interface2;
        this.statsInterface1 = new InterfaceStatistics();
        this.statsInterface2 = new InterfaceStatistics();
    }

    public void Start()
    {
        interface1.Open(DeviceModes.Promiscuous, 100);
        interface2.Open(DeviceModes.Promiscuous, 100);

        interface1.OnPacketArrival += OnPacketArrival;
        interface2.OnPacketArrival += OnPacketArrival;

        interface1.StartCapture();
        interface2.StartCapture();
    }

    public void Stop()
    {
        if (interface1 != null && interface1.Started)
        {
            interface1.StopCapture();
            interface1.Close();
        }

        if (interface2 != null && interface2.Started)
        {
            interface2.StopCapture();
            interface2.Close();
        }

        receivedPackets.Clear();
    }

    private void OnPacketArrival(object sender, PacketCapture e)
    {
        var device = sender as LibPcapLiveDevice;

        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        string packetHash = ComputePacketHash(rawPacket.Data);

        if (receivedPackets.ContainsKey(packetHash))
        {
            receivedPackets[packetHash] = false; 
            return;
        }

        receivedPackets[packetHash] = true;

        if (device == interface1)
        {
            HandleIncomingPacket(interface2, packet, statsInterface1, statsInterface2);
        }
        else if (device == interface2)
        {
            HandleIncomingPacket(interface1, packet, statsInterface2, statsInterface1);
        }
    }
    private void HandleIncomingPacket(LibPcapLiveDevice outgoingInterface, Packet packet, InterfaceStatistics inStats, InterfaceStatistics outStats)
    {
        inStats.AnalyzePacket(packet, true);
        inStats.IncrementTotalIn();

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
    private string ComputePacketHash(byte[] packetData)
    {
        var hashBytes = XxHash64.Hash(packetData);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public InterfaceStatistics GetStatsInterface1() => statsInterface1;
    public InterfaceStatistics GetStatsInterface2() => statsInterface2;
}