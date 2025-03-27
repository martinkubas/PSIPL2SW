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
        private Dictionary<PhysicalAddress, List<(int InterfaceIndex, DateTime LastSeen)>> history = new Dictionary<PhysicalAddress, List<(int, DateTime)>>();
        private readonly object _lock = new object();

        public MACTable()
        {
            macTable = new Dictionary<PhysicalAddress, (int, DateTime)>();
        }

        public void AddOrUpdate(PhysicalAddress macAddress, int interfaceIndex)
        {
            lock (_lock)
            {
                // Track history
                if (!history.ContainsKey(macAddress))
                    history[macAddress] = new List<(int, DateTime)>();

                history[macAddress].Add((interfaceIndex, DateTime.Now));

                // Keep only last 5 entries
                if (history[macAddress].Count > 5)
                    history[macAddress].RemoveAt(0);

                // If flapping between interfaces, don't update
                if (history[macAddress].Select(x => x.InterfaceIndex).Distinct().Count() > 1)
                {
                    Console.WriteLine($"MAC flapping detected: {macAddress}");
                    return;
                }

                macTable[macAddress] = (interfaceIndex, DateTime.Now);
            }
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
