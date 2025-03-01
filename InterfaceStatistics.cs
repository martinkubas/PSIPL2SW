using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap.LibPcap;
using SharpPcap;
using PacketDotNet;

namespace Projekt
{
    public class InterfaceStatistics
    {
        public int Ethernet2In { get; private set; }
        public int Ethernet2Out { get; private set; }

        public int ArpIn { get; private set; }
        public int ArpOut { get; private set; }

        public int IPIn { get; private set; }
        public int IPOut { get; private set; }

        public int ICMPIn { get; private set; }
        public int ICMPOut { get; private set; }

        public int TCPIn { get; private set; }
        public int TCPOut { get; private set; }

        public int UDPIn { get; private set; }
        public int UDPOut { get; private set; }

        public int totalIn { get; private set; }
        public int totalOut { get; private set; }
        public InterfaceStatistics()
        {
            Reset();
        }

        public void IncrementEthernet2In() => Ethernet2In++;
        public void IncrementEthernet2Out() => Ethernet2Out++;

        public void IncrementArpIn() => ArpIn++;
        public void IncrementArpOut() => ArpOut++;

        public void IncrementIPIn() => IPIn++;
        public void IncrementIPOut() => IPOut++;

        public void IncrementICMPIn() => ICMPIn++;
        public void IncrementICMPOut() => ICMPOut++;

        public void IncrementTCPIn() => TCPIn++;
        public void IncrementTCPOut() => TCPOut++;

        public void IncrementUDPIn() => UDPIn++;
        public void IncrementUDPOut() => UDPOut++;

        public void IncrementTotalIn() => totalIn++;
        public void IncrementTotalOut() => totalOut++;

        public void Reset()
        {
            Ethernet2In = 0;
            Ethernet2Out = 0;
            ArpIn = 0;
            ArpOut = 0;
            IPIn = 0;
            IPOut = 0;
            ICMPIn = 0;
            ICMPOut = 0;
            TCPIn = 0;
            TCPOut = 0;
            UDPIn = 0;
            UDPOut = 0;
            totalIn = 0;
            totalOut = 0;
        }

        public void AnalyzePacket(Packet packet, bool isIncoming)
        {
            var ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket != null)
            {
                if (isIncoming) IncrementEthernet2In(); 
                else IncrementEthernet2Out(); 

                var arpPacket = packet.Extract<ArpPacket>();
                if (arpPacket != null)
                {
                    if (isIncoming) IncrementArpIn();
                    else IncrementArpOut(); 
                    return;
                }

                var ipPacket = packet.Extract<IPPacket>();
                if (ipPacket != null)
                {
                    if (isIncoming) IncrementIPIn(); 
                    else IncrementIPOut(); 

                    var tcpPacket = packet.Extract<TcpPacket>();
                    if (tcpPacket != null)
                    {
                        if (isIncoming) IncrementTCPIn(); 
                        else IncrementTCPOut(); 

                        //HTTP/HTTPS v buducnosti
                        return;
                    }

                    var udpPacket = packet.Extract<UdpPacket>();
                    if (udpPacket != null)
                    {
                        if (isIncoming) IncrementUDPIn(); 
                        else IncrementUDPOut(); 
                        return;
                    }

                    var icmpPacket = packet.Extract<IcmpV4Packet>();
                    if (icmpPacket != null)
                    {
                        if (isIncoming) IncrementICMPIn();
                        else IncrementICMPOut(); 
                        return;
                    }
                }
            }
        }
    }
}