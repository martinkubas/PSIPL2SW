using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class InterfaceStatistics
    {
        public int IncomingPackets { get; private set; }
        public int OutgoingPackets { get; private set; }

        public InterfaceStatistics()
        {
            IncomingPackets = 0;
            OutgoingPackets = 0;
        }

        public void IncrementIncomingPackets()
        {
            IncomingPackets++;
        }

        public void IncrementOutgoingPackets()
        {
            OutgoingPackets++;
        }

        public void Reset()
        {
            IncomingPackets = 0;
            OutgoingPackets = 0;
        }
    }
}