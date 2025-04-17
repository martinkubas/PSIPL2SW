using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Projekt
{
    public partial class Form1 : Form
    {
        private List<LibPcapLiveDevice> activeInterfaces = new List<LibPcapLiveDevice>();
        private PacketForwarder packetForwarder;
        private Dictionary<int, TableLayoutPanel> interfacePanels;
        public Form1()
        {
            InitializeComponent();
            InitializeDevices();
            interfacePanels = new Dictionary<int, TableLayoutPanel>
            {
                { 0, interface1InOut },
                { 1, interface2InOut }
            };
        }
        private void InitializeDevices()
        {
            try
            {
                LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in InitializeComponents: {ex.Message}");
            }
        }

        private void LoadDevices()
        {
            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count == 0)
            {
                MessageBox.Show("No network devices found.");
                return;
            }

            comboBoxInterface1.DataSource = devices.Select(d => new { Device = d, Name = d.Interface.FriendlyName + " | " + d.Interface.Description }).ToList();
            comboBoxInterface1.DisplayMember = "Name";
            comboBoxInterface1.ValueMember = "Device";

            comboBoxInterface2.DataSource = devices.Select(d => new { Device = d, Name = d.Interface.FriendlyName + " | " + d.Interface.Description }).ToList();
            comboBoxInterface2.DisplayMember = "Name";
            comboBoxInterface2.ValueMember = "Device";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(comboBoxInterface1.SelectedValue == comboBoxInterface2.SelectedValue)
            {
                MessageBox.Show("The devices cannot be the same");
            }
            activeInterfaces.Add(comboBoxInterface1.SelectedValue as LibPcapLiveDevice);
            activeInterfaces.Add(comboBoxInterface2.SelectedValue as LibPcapLiveDevice);

            try
            {
                packetForwarder = new PacketForwarder(activeInterfaces);

                packetForwarder.Start();

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                timerStatistics.Start();
                agingTimer.Start();

                UpdateMacAddressTableUI();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting packet forwarder: {ex.Message}");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (packetForwarder != null)
                {
                    packetForwarder.Stop();
                    activeInterfaces.Clear();
                }

                btnStart.Enabled = true;
                btnStop.Enabled = false;

                timerStatistics.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping packet forwarder: {ex.Message}");
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                for (int i = 0; i < activeInterfaces.Count; i++)
                {
                    var stats = packetForwarder.GetStatsForInterface(i);
                    stats.Reset();
                }
            }
        }
        private void btnResetMAC_Click(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                packetForwarder.GetMacAddressTable().Clear();
            }
        }
        private void timerStatistics_Tick(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                for (int i = 0; i < activeInterfaces.Count; i++)
                {
                    var stats = packetForwarder.GetStatsForInterface(i);
                    if (interfacePanels.TryGetValue(i, out TableLayoutPanel table))
                    {
                        UpdateInterfaceStatsUI(table, stats);
                    }

                }
                UpdateMacAddressTableUI();

                Console.WriteLine("ACL int 1: " + packetForwarder.getACLforInt(0));
                Console.WriteLine("ACL int 2: " + packetForwarder.getACLforInt(1));
            }
        }
        private void UpdateInterfaceStatsUI(TableLayoutPanel table, InterfaceStatistics stats)
        {
            UpdateLabelText(table, 0, 1, $"ARP: {stats.ArpIn}");
            UpdateLabelText(table, 0, 2, $"Ethernet2: {stats.Ethernet2In}");
            UpdateLabelText(table, 0, 3, $"IP: {stats.IPIn}");
            UpdateLabelText(table, 0, 4, $"ICMP: {stats.ICMPIn}");
            UpdateLabelText(table, 0, 5, $"TCP: {stats.TCPIn}");
            UpdateLabelText(table, 0, 6, $"UDP: {stats.UDPIn}");
            UpdateLabelText(table, 0, 7, $"Total: {stats.totalIn}");

            UpdateLabelText(table, 1, 1, $"ARP: {stats.ArpOut}");
            UpdateLabelText(table, 1, 2, $"Ethernet2: {stats.Ethernet2Out}");
            UpdateLabelText(table, 1, 3, $"IP: {stats.IPOut}");
            UpdateLabelText(table, 1, 4, $"ICMP: {stats.ICMPOut}");
            UpdateLabelText(table, 1, 5, $"TCP: {stats.TCPOut}");
            UpdateLabelText(table, 1, 6, $"UDP: {stats.UDPOut}");
            UpdateLabelText(table, 1, 7, $"Total: {stats.totalOut}");

        }
        private void AgingTimer_Tick(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                packetForwarder.GetMacAddressTable().RemoveOldEntries(TimeSpan.FromSeconds((double)agingTimerDuration.Value));
            }
        }

        private void UpdateMacAddressTableUI()
        {
            try
            {
                macAddressTableGrid.Rows.Clear();

                var macTable = packetForwarder.GetMacAddressTable().GetTable();
                foreach (var entry in macTable)
                {
                    macAddressTableGrid.Rows.Add(
                        entry.Key.ToString(),
                        $"Interface {entry.Value.InterfaceIndex + 1}",
                        entry.Value.LastSeen.ToString("HH:mm:ss")
                    );
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        private void UpdateLabelText(TableLayoutPanel table, int column, int row, string text)
        {
            var control = table.GetControlFromPosition(column, row);

            if (control is Label label)
            {
                label.Text = text;
            }
        }
        private void btnAddRule_Click(object sender, EventArgs e)
        {
            this.ruleTableGrid.Rows.Add("Allow", "1","In", "Any", "Any","Any","Any", "Any");
        }

        private void btnRemoveRule_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.ruleTableGrid.SelectedRows)
            {
                this.ruleTableGrid.Rows.Remove(row);
            }
        }
        private void btnSaveRules_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null)
            {
                MessageBox.Show("Please start the packet forwarder first");
                return;
            }

            try
            {
                // Clear all existing ACL rules
                packetForwarder.ClearAllACLs();

                // Add all rules from the DataGridView
                foreach (DataGridViewRow row in ruleTableGrid.Rows)
                {
                    if (row.IsNewRow) continue;

                    var ace = new ACE
                    {
                        RuleAction = row.Cells["ActionColumn"].Value?.ToString() == "Allow" ?
                            ACE.Action.Allow : ACE.Action.Deny,
                        RuleDirection = row.Cells["DirectionColumn"].Value?.ToString() == "In" ?
                            ACE.Direction.In : ACE.Direction.Out,
                        // Parse other fields as needed
                        SourceMAC = ParseMAC(row.Cells["SrcAddressColumnMAC"].Value?.ToString()),
                        DestinationMAC = ParseMAC(row.Cells["DstAddressColumnMAC"].Value?.ToString()),
                        SourceIP = ParseIP(row.Cells["SrcAddressColumnIP"].Value?.ToString()),
                        DestinationIP = ParseIP(row.Cells["DstAddressColumnIP"].Value?.ToString()),
                        RuleProtocol = ParseProtocol(row.Cells["ProtocolColumn"].Value?.ToString())
                    };
                    int interfaceIndex = int.Parse(row.Cells["InterfaceColumn"].Value?.ToString() ?? "1") - 1;


                    packetForwarder.AddACE(interfaceIndex, ace);
                }

                MessageBox.Show("Rules saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving rules: {ex.Message}");
            }
        }
        private PhysicalAddress ParseMAC(string mac)
        {
            if (string.IsNullOrEmpty(mac)) return null;
            try { return PhysicalAddress.Parse(mac); }
            catch { return null; }
        }

        private System.Net.IPAddress ParseIP(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return null;
            try { return System.Net.IPAddress.Parse(ip); }
            catch { return null; }
        }

        private ACE.Protocol ParseProtocol(string protocol)
        {
            if (string.IsNullOrEmpty(protocol)) return ACE.Protocol.Any;
            try { return (ACE.Protocol)Enum.Parse(typeof(ACE.Protocol), protocol); }
            catch { return ACE.Protocol.Any; }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (packetForwarder != null)
            {
                packetForwarder.Stop();
            }
        }
    }
}