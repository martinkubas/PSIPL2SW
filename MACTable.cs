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
        private readonly object _lock = new object();

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
            lock (_lock)
            {
                var now = DateTime.Now;
                var oldEntries = macTable.Where(entry => now - entry.Value.LastSeen > maxAge)
                                        .Select(entry => entry.Key)
                                        .ToList();

                foreach (var mac in oldEntries)
                {
                    macTable.Remove(mac);
                }
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
            lock (_lock)
            {
                return new Dictionary<PhysicalAddress, (int, DateTime)>(macTable);
            }
        }

    }
}
