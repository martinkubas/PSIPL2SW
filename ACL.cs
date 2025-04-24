using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;

namespace Projekt
{
    public class ACL
    {
        public List<ACE> acl = new List<ACE>();

        public void AddACE(ACE ace) => acl.Add(ace);

        public void RemoveACE(int index)
        {
            if (index >= 0 && index < acl.Count) acl.RemoveAt(index);
        }

        public void ClearAllACEs()
        {
            acl.Clear();
        }

        public bool AllowPacket(Packet packet)
        {
            if (acl.Count == 0) return true;

            foreach (ACE ace in acl)
            {
                if (MatchesRule(ace, packet))
                {
                    return ace.RuleAction.Equals("Allow");
                }
            }

            return false; 
        }

        private bool MatchesRule(ACE ace, Packet packet)
        {
            if (!MatchesProtocol(ace.RuleProtocol, packet))
                return false;

            var ethernetPacket = packet.Extract<EthernetPacket>();
            if (ethernetPacket != null)
            {
                if (ace.SourceMAC != null && !ethernetPacket.SourceHardwareAddress.Equals(ace.SourceMAC))
                    return false;

                if (ace.DestinationMAC != null && !ethernetPacket.DestinationHardwareAddress.Equals(ace.DestinationMAC))
                    return false;
            }

            var ipPacket = packet.Extract<IPPacket>();
            if (ipPacket != null)
            {
                if (ace.SourceIP != null && !ipPacket.SourceAddress.Equals(ace.SourceIP))
                    return false;

                if (ace.DestinationIP != null && !ipPacket.DestinationAddress.Equals(ace.DestinationIP))
                    return false;
            }

            return MatchesPort(ace, packet);
        }

        private bool MatchesProtocol(string ruleProtocol, Packet packet)
        {
            if (ruleProtocol == "Any") return true;

            switch (ruleProtocol)
            {
                case "Ethernet":
                    return packet is EthernetPacket;
                case "IP":
                    return packet.Extract<IPPacket>() != null;
                case "ARP":
                    return packet.Extract<ArpPacket>() != null;
                case "ICMP":
                    return packet.Extract<IcmpV4Packet>() != null;
                case "TCP":
                    return packet.Extract<TcpPacket>() != null;
                case "UDP":
                    return packet.Extract<UdpPacket>() != null;
                default:
                    return false;
            }
        }

        private bool MatchesPort(ACE ace, Packet packet)
        {
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null)
            {
                if (ace.SourcePort.HasValue && tcpPacket.SourcePort != ace.SourcePort.Value)
                    return false;

                if (ace.DestinationPort.HasValue && tcpPacket.DestinationPort != ace.DestinationPort.Value)
                    return false;

                return true;
            }

            var udpPacket = packet.Extract<UdpPacket>();
            if (udpPacket != null)
            {
                if (ace.SourcePort.HasValue && udpPacket.SourcePort != ace.SourcePort.Value)
                    return false;

                if (ace.DestinationPort.HasValue && udpPacket.DestinationPort != ace.DestinationPort.Value)
                    return false;

                return true;
            }

            return true;
        }



        public override string ToString()
        {
            if (acl.Count == 0) return "No rules in ACL";

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < acl.Count; i++)
            {
                sb.AppendLine($"[{i + 1}] {acl[i]}");
            }
            return sb.ToString();
        }
    }
}