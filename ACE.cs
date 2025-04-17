using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class ACE
    {

        public enum Direction { In, Out }
        public enum Action { Allow, Deny }
        public enum Protocol { Any, Ethernet, IPv4, ARP, ICMP, TCP, UDP, HTTP, HTTPS }

        public int Id { get; set; }
        public Direction RuleDirection { get; set; } = Direction.In;
        public Action RuleAction { get; set; } = Action.Allow;
        public Protocol RuleProtocol { get; set; } = Protocol.Any;
        public PhysicalAddress SourceMAC { get; set; } = null;
        public PhysicalAddress DestinationMAC { get; set; } = null;
        public System.Net.IPAddress SourceIP { get; set; } = null;
        public System.Net.IPAddress DestinationIP { get; set; } = null;
        public int? SourcePort { get; set; } = null;
        public int? DestinationPort { get; set; } = null;
        public int? ICMPType { get; set; } = null;


        public override string ToString()
        {
            return $"{RuleAction} {RuleDirection} | " +
                   $"SrcMAC: {SourceMAC?.ToString() ?? "Any"} | " +
                   $"DstMAC: {DestinationMAC?.ToString() ?? "Any"} | " +
                   $"SrcIP: {SourceIP?.ToString() ?? "Any"} | " +
                   $"DstIP: {DestinationIP?.ToString() ?? "Any"} | " +
                   $"Protocol: {RuleProtocol}";
        }
    }
}
