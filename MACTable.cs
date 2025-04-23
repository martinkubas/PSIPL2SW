using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Mail;

namespace Projekt
{
    public class MACTable
    {
        private Dictionary<PhysicalAddress, (int InterfaceIndex, DateTime LastSeen)> macTable;
        private object _lock = new object();
        private Dictionary<PhysicalAddress, Queue<int>> history = new Dictionary<PhysicalAddress, Queue<int>>();

        private PacketForwarder packetForwarder;

        public MACTable()
        {
            macTable = new Dictionary<PhysicalAddress, (int, DateTime)>();
            history = new Dictionary<PhysicalAddress, Queue<int>>();
        }
        public void SetPacketForwarder(PacketForwarder forwarder)
        {
            packetForwarder = forwarder;
        }

        public void AddOrUpdate(PhysicalAddress macAddress, int interfaceIndex)
        {
            lock (_lock)
            {
                var currentTime = DateTime.Now;

                if (!history.TryGetValue(macAddress, out var packetHistory))
                {
                    packetHistory = new Queue<int>();
                    history[macAddress] = packetHistory;

                    if (packetForwarder != null)
                    {
                        packetForwarder.LogToSyslog($"New MAC {macAddress} discovered on interface {interfaceIndex + 1}", SyslogSeverity.Informational);
                    }
                }

                packetHistory.Enqueue(interfaceIndex);

                while (packetHistory.Count > 3)
                {
                    packetHistory.Dequeue();
                }

                if (macTable.TryGetValue(macAddress, out var existingEntry))
                {
                    int newInterface = ShouldUpdateInterface(macAddress, interfaceIndex)
                        ? interfaceIndex
                        : existingEntry.InterfaceIndex;

                    if (newInterface != existingEntry.InterfaceIndex)
                    {
                        if (packetForwarder != null)
                        {
                            packetForwarder.LogToSyslog($"MAC {macAddress} moved from interface {existingEntry.InterfaceIndex + 1} to interface {newInterface + 1}", SyslogSeverity.Notice);
                        }

                        RemoveEntriesForInterface(existingEntry.InterfaceIndex);
                        RemoveEntriesForInterface(newInterface);
                    }

                    macTable[macAddress] = (newInterface, currentTime);
                }
                else
                {
                    macTable[macAddress] = (interfaceIndex, currentTime);
                }
            }
        }
        private bool ShouldUpdateInterface(PhysicalAddress macAddress, int newInterface)
        {
            if (!history.TryGetValue(macAddress, out var packetHistory) || packetHistory.Count < 3)
            {

                return true;
            }

            bool allSameInterface = packetHistory.All(x => x == newInterface);

            int currentInterface = macTable.TryGetValue(macAddress, out var entry) ? entry.InterfaceIndex : -1;

            return allSameInterface && (newInterface != currentInterface);
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
        public void RemoveEntriesForInterface(int interfaceIndex)
        {
            lock (_lock)
            {
                var interfaceEntries = macTable.Where(entry => entry.Value.InterfaceIndex == interfaceIndex)
                                             .Select(entry => entry.Key)
                                             .ToList();

                foreach (var macAddress in interfaceEntries)
                {
                    Remove(macAddress);
                }
            }
        }

        public void RemoveOldEntries(TimeSpan maxAge)
        {
            lock (_lock)
            {
                var now = DateTime.Now;
                var oldEntries = macTable.Where(entry => (now - entry.Value.LastSeen) > maxAge)
                                         .Select(entry => entry.Key)
                                         .ToList();

                foreach (var macAddress in oldEntries)
                {
                    if (macTable.TryGetValue(macAddress, out var entry))
                    {
                        if(packetForwarder != null)
                        {
                            packetForwarder.LogToSyslog($"MAC {macAddress} aged out from interface {entry.InterfaceIndex + 1}", SyslogSeverity.Notice);
                        }
                       
                        macTable.Remove(macAddress);
                    }
                }
            }
        }

        public void Remove(PhysicalAddress macAddress)
        {
            macTable.Remove(macAddress);
            history.Remove(macAddress);
        }

        public void Clear()
        {
            lock (_lock)
            {
                if (packetForwarder != null)
                {
                    packetForwarder.LogToSyslog($"MAC address table cleared ({macTable.Count} entries)", SyslogSeverity.Notice);
                }
                macTable.Clear();
                history.Clear();
            }
        }

        public IEnumerable<KeyValuePair<PhysicalAddress, (int InterfaceIndex, DateTime LastSeen)>> GetTable()
        {
            return macTable;
        }
        public List<(PhysicalAddress Mac, int Interface, int AgeSeconds)> GetTableEntries()
        {
            lock (_lock)
            {
                var now = DateTime.Now;
                return macTable.Select(entry =>
                    (entry.Key, entry.Value.InterfaceIndex, (int)(now - entry.Value.LastSeen).TotalSeconds))
                    .ToList();
            }
        }

    }
}