using Projekt;
using SharpPcap.LibPcap;
using SharpPcap;
using System;
using PacketDotNet;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;


public class PacketForwarder
{
    private LibPcapLiveDevice interface1;
    private LibPcapLiveDevice interface2;
    private InterfaceStatistics statsInterface1;
    private InterfaceStatistics statsInterface2;

    // Hashtable to track received packets
    private Hashtable receivedPackets = new Hashtable(100000);
    public PacketForwarder(LibPcapLiveDevice interface1, LibPcapLiveDevice interface2)
    {
        this.interface1 = interface1;
        this.interface2 = interface2;
        this.statsInterface1 = new InterfaceStatistics();
        this.statsInterface2 = new InterfaceStatistics();
    }

    public void Start()
    {
        // Open both interfaces in promiscuous mode
        interface1.Open(DeviceModes.Promiscuous, 100);
        interface2.Open(DeviceModes.Promiscuous, 100);

        // Set up packet capture handlers for both interfaces
        interface1.OnPacketArrival += OnPacketArrival;
        interface2.OnPacketArrival += OnPacketArrival;

        // Start capturing packets on both interfaces
        interface1.StartCapture();
        interface2.StartCapture();
    }

    public void Stop()
    {
        // Stop capturing packets on both interfaces
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

        // Clear the hashtable
        receivedPackets.Clear();
    }

    private void OnPacketArrival(object sender, PacketCapture e)
    {
        var device = sender as LibPcapLiveDevice;

        // Get the raw packet bytes
        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        // Generate a unique identifier for the packet (SHA-256 hash of the packet data)
        string packetHash = ComputePacketHash(rawPacket.Data);

        // Check if the packet has already been processed
        if (receivedPackets.ContainsKey(packetHash))
        {
            // Packet has already been processed, drop it to prevent cycling
            return;
        }

        // Add the packet to the hashtable
        receivedPackets[packetHash] = true;

        // Determine the direction of the packet
        if (device == interface1)
        {
            // Packet arrived on Interface 1 (incoming)
            HandleIncomingPacket(interface2, packet);
        }
        else if (device == interface2)
        {
            // Packet arrived on Interface 2 (incoming)
            HandleIncomingPacket(interface1, packet);
        }
    }
    private void HandleIncomingPacket(LibPcapLiveDevice outgoingInterface, Packet packet)
    {
        // Increment incoming packet count for the incoming interface
        if (outgoingInterface == interface2)
        {
            statsInterface1.IncrementIncomingPackets();
        }
        else
        {
            statsInterface2.IncrementIncomingPackets();
        }

        // Forward the packet to the other interface
        try
        {
            outgoingInterface.SendPacket(packet.Bytes);

            // Increment outgoing packet count for the outgoing interface
            if (outgoingInterface == interface2)
            {
                statsInterface2.IncrementOutgoingPackets();
            }
            else
            {
                statsInterface1.IncrementOutgoingPackets();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error forwarding packet: {ex.Message}");
        }
    }
    private string ComputePacketHash(byte[] packetData)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(packetData);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }

    // Expose statistics for the UI
    public InterfaceStatistics GetStatsInterface1() => statsInterface1;
    public InterfaceStatistics GetStatsInterface2() => statsInterface2;
}