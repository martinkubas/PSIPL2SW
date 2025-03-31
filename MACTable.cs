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
        private TimeSpan minimumHoldTime = TimeSpan.FromSeconds(2); // Minimum time before allowing interface change

        public MACTable()
        {
            macTable = new Dictionary<PhysicalAddress, (int, DateTime)>();
        }

        public void AddOrUpdate(PhysicalAddress macAddress, int interfaceIndex)
        {
            lock (_lock)
            {
                if (macTable.TryGetValue(macAddress, out var current))
                {
                    // Only update if:
                    // 1. Interface actually changed, AND
                    // 2. Enough time has passed since last update, OR
                    // 3. The MAC hasn't been seen in a while (stale entry)
                    if (current.InterfaceIndex != interfaceIndex)
                    {
                        var timeSinceLastSeen = DateTime.Now - current.LastSeen;
                        if (timeSinceLastSeen > minimumHoldTime || timeSinceLastSeen > TimeSpan.FromMinutes(5))
                        {
                            macTable[macAddress] = (interfaceIndex, DateTime.Now);
                            Console.WriteLine($"MAC {macAddress} moved from interface {current.InterfaceIndex} to {interfaceIndex}");
                        }
                    }
                    else
                    {
                        // Just update timestamp for existing entry
                        macTable[macAddress] = (current.InterfaceIndex, DateTime.Now);
                    }
                }
                else
                {
                    // New MAC address
                    macTable[macAddress] = (interfaceIndex, DateTime.Now);
                }
            }
        }

        public int GetInterface(PhysicalAddress macAddress)
        {
            lock (_lock)
            {

                if (macTable.TryGetValue(macAddress, out var entry))
                {
                    return entry.InterfaceIndex;
                }
                return -1;
            }
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
