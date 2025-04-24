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
                return;
            }
            activeInterfaces.Add(comboBoxInterface1.SelectedValue as LibPcapLiveDevice);
            activeInterfaces.Add(comboBoxInterface2.SelectedValue as LibPcapLiveDevice);

            try
            {
                packetForwarder = new PacketForwarder(activeInterfaces);

                packetForwarder.Start();

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                btnConfigureSyslog.Enabled = true;
                btnStartSyslog.Enabled = false;  
                btnStopSyslog.Enabled = false;

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

                btnConfigureSyslog.Enabled = false;
                btnStartSyslog.Enabled = false;
                btnStopSyslog.Enabled = false;

                timerStatistics.Stop();
                packetForwarder = null;
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
                    if (packetForwarder.IsSyslogEnabled())
                    {
                        packetForwarder.LogToSyslog($"Statistics reset for interface {i + 1}", SyslogSeverity.Notice);
                    }
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

            }
            if (packetForwarder.IsSyslogEnabled())
            {
                packetForwarder.LogToSyslog($"Test Test", SyslogSeverity.Informational);
            }
            Console.WriteLine("Int1 in: " +  packetForwarder.getACLforInt(0, true) + "Int1 out: " + packetForwarder.getACLforInt(0, false));
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
            if (packetForwarder == null) return;
            var form = new RuleEditorForm(activeInterfaces);
            if (form.ShowDialog() == DialogResult.OK)
            {
                ACE newRule = form.NewRule;
                int interfaceIndex = form.comboInterface.SelectedIndex;
                string direction = form.comboDirection.SelectedItem.ToString();

                packetForwarder.AddACE(interfaceIndex, newRule, direction == "In");
                rulesListView.Items.Insert(0, new ListViewItem(new[] { (interfaceIndex + 1).ToString(), newRule.RuleAction.ToString(), direction, newRule.ToString() }));
                if (packetForwarder.IsSyslogEnabled())
                {
                    packetForwarder.LogToSyslog($"ACL rule added to interface {interfaceIndex + 1} {direction.ToLower()}bound", SyslogSeverity.Informational);
                }
            }
        }

        private void btnRemoveRule_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null || rulesListView.SelectedItems.Count == 0)
                return;

            ListViewItem selectedItem = rulesListView.SelectedItems[0];

            int interfaceIndex = int.Parse(selectedItem.SubItems[0].Text) - 1;
            string direction = selectedItem.SubItems[2].Text;

            ACL acl = packetForwarder.getACLforInt(interfaceIndex, direction == "In");

            string ruleText = selectedItem.SubItems[3].Text;
            ACE aceToRemove = acl.acl.FirstOrDefault(ace => ace.ToString() == ruleText);

            if (aceToRemove != null)
            {
                acl.RemoveACE(acl.acl.IndexOf(aceToRemove));

                rulesListView.Items.Remove(selectedItem);

                if (packetForwarder.IsSyslogEnabled())
                {
                    packetForwarder.LogToSyslog($"ACL rule removed from interface {interfaceIndex + 1} {direction.ToLower()}bound", SyslogSeverity.Informational);
                }

            }
            else
            {
                MessageBox.Show("Could not find matching rule in ACL", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnCheckRuleForInt_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null || rulesListView.SelectedItems.Count == 0)
                return;

            ListViewItem selectedItem = rulesListView.SelectedItems[0];

            int interfaceIndex = int.Parse(selectedItem.SubItems[0].Text) - 1;
            string displayText = $"=== Interface {interfaceIndex + 1} ACL Rules ===" + Environment.NewLine + Environment.NewLine +
                       "=== INCOMING RULES ===" + Environment.NewLine +
                       packetForwarder.getACLforInt(interfaceIndex, true).ToString() + Environment.NewLine + Environment.NewLine +
                       "=== OUTGOING RULES ===" + Environment.NewLine +
                       packetForwarder.getACLforInt(interfaceIndex, false).ToString();

            var viewer = new ACLViewerForm();
            viewer.SetRulesText(displayText);
            viewer.ShowDialog();
        }
        private void btnConfigureSyslog_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null)
            {
                MessageBox.Show("You must start the switch first before configuring syslog.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = packetForwarder.ConfigureSyslog(txtSourceIP.Text, txtServerIP.Text);
            if (success)
            {
                MessageBox.Show("Syslog configuration successful.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStartSyslog.Enabled = true;
            }
            else
            {
                MessageBox.Show("Failed to configure Syslog. Please check IP addresses.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartSyslog_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null) return;

            packetForwarder.StartSyslog();
            btnStartSyslog.Enabled = false;
            btnStopSyslog.Enabled = true;
        }

        private void btnStopSyslog_Click(object sender, EventArgs e)
        {
            if (packetForwarder == null) return;

            packetForwarder.StopSyslog();
            btnStartSyslog.Enabled = true;
            btnStopSyslog.Enabled = false;
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