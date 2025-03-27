using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public static class PhysicalAddressExtensions
    {
        public static bool IsBroadcast(this PhysicalAddress address)
        {
            byte[] broadcastBytes = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            return address.GetAddressBytes().SequenceEqual(broadcastBytes);
        }

        public static bool IsMulticast(this PhysicalAddress address)
        {
            return (address.GetAddressBytes()[0] & 0x01) != 0;
        }
    }
}
