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
        public string RuleAction { get; set; } = "Allow";
        public string RuleProtocol { get; set; } = "Any";
        public PhysicalAddress SourceMAC { get; set; } = null;
        public PhysicalAddress DestinationMAC { get; set; } = null;
        public System.Net.IPAddress SourceIP { get; set; } = null;
        public System.Net.IPAddress DestinationIP { get; set; } = null;
        public ushort? SourcePort { get; set; } = null;
        public ushort? DestinationPort { get; set; } = null;

        public override string ToString()
        {
            return $"{RuleAction} | " +
                   $"Protocol: {RuleProtocol} | " +
                   $"SrcMAC: {SourceMAC?.ToString() ?? "Any"} | " +
                   $"DstMAC: {DestinationMAC?.ToString() ?? "Any"} | " +
                   $"SrcIP: {SourceIP?.ToString() ?? "Any"} | " +
                   $"DstIP: {DestinationIP?.ToString() ?? "Any"} | " +
                   $"SrcPort: {SourcePort.ToString() ?? "Any"} | " +
                   $"DstPort: {DestinationPort.ToString() ?? "Any"}";
        }
    }
}
