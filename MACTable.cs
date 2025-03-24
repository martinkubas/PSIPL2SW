using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation; 

namespace Projekt
{
    public class MACTable
    {
        private Dictionary<PhysicalAddress, (int InterfaceIndex, DateTime LastSeen)> macTable;

        public MACTable()
        {
            macTable = new Dictionary<PhysicalAddress, (int, DateTime)>();
        }

        public void AddOrUpdate(PhysicalAddress macAddress, int interfaceIndex)
        {
            macTable[macAddress] = (interfaceIndex, DateTime.Now);
        }

        public int GetInterface(PhysicalAddress macAddress)
        {
            if (macTable.TryGetValue(macAddress, out var entry))
            {
                return entry.InterfaceIndex;
            }
            return -1;
        }

        public void RemoveOldEntries(TimeSpan maxAge)
        {
            var now = DateTime.Now;
            var oldEntries = new List<PhysicalAddress>();

            foreach (var entry in macTable)
            {
                if (now - entry.Value.LastSeen > maxAge)
                {
                    oldEntries.Add(entry.Key);
                }
            }

            foreach (var macAddress in oldEntries)
            {
                macTable.Remove(macAddress);
            }
        }

        public void Remove(PhysicalAddress macAddress)
        {
            macTable.Remove(macAddress);
        }

        public void Clear()
        {
            macTable.Clear();
        }

        public IEnumerable<KeyValuePair<PhysicalAddress, (int InterfaceIndex, DateTime LastSeen)>> GetTable()
        {
            return macTable;
        }

    }
}
