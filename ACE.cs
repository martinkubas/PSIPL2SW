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
        public Direction RuleDirection { get; set; }
        public Action RuleAction { get; set; }
        public Protocol RuleProtocol { get; set; }
        public PhysicalAddress SourceMAC { get; set; }
        public PhysicalAddress DestinationMAC { get; set; }
        public System.Net.IPAddress SourceIP { get; set; }
        public System.Net.IPAddress DestinationIP { get; set; }
        public int? SourcePort { get; set; }
        public int? DestinationPort { get; set; }
        public int? ICMPType { get; set; }
        public int InterfaceIndex { get; set; }
    }
}
