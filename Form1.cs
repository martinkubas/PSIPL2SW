using System;
using System.Linq;
using System.Windows.Forms;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Projekt
{
    public partial class Form1 : Form
    {
        private LibPcapLiveDevice interface1;
        private LibPcapLiveDevice interface2;
        private PacketForwarder packetForwarder;

        public Form1()
        {
            InitializeComponent();
            InitializeDevices();
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
            interface1 = comboBoxInterface1.SelectedValue as LibPcapLiveDevice;
            interface2 = comboBoxInterface2.SelectedValue as LibPcapLiveDevice;

            if (interface1 == interface2)
            {
                MessageBox.Show("Interface 1 and Interface 2 must be different.");
                return;
            }

            try
            {
                packetForwarder = new PacketForwarder(interface1, interface2);

                packetForwarder.Start();

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                timerStatistics.Start();
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
                if ((sender as Button).Name == "btnResetInt1")
                {
                    InterfaceStatistics int1Stats = packetForwarder.GetStatsInterface1();
                    int1Stats.Reset();
                }
                else
                {
                    InterfaceStatistics int2Stats = packetForwarder.GetStatsInterface2();
                    int2Stats.Reset();
                }

            }
        }

        private void timerStatistics_Tick(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                
                var statsInterface1 = packetForwarder.GetStatsInterface1();
                
                UpdateLabelText(this.interface1InOut, 0, 1, $"ARP: {statsInterface1.ArpIn}");
                UpdateLabelText(this.interface1InOut, 0, 2, $"Ethernet2: {statsInterface1.Ethernet2In}");
                UpdateLabelText(this.interface1InOut, 0, 3, $"IP: {statsInterface1.IPIn}");
                UpdateLabelText(this.interface1InOut, 0, 4, $"ICMP: {statsInterface1.ICMPIn}");
                UpdateLabelText(this.interface1InOut, 0, 5, $"TCP: {statsInterface1.TCPIn}");
                UpdateLabelText(this.interface1InOut, 0, 6, $"UDP: {statsInterface1.UDPIn}");
                UpdateLabelText(this.interface1InOut, 0, 7, $"Total: {statsInterface1.totalIn}");
                
                UpdateLabelText(this.interface1InOut, 1, 1, $"ARP: {statsInterface1.ArpOut}");
                UpdateLabelText(this.interface1InOut, 1, 2, $"Ethernet2: {statsInterface1.Ethernet2Out}");
                UpdateLabelText(this.interface1InOut, 1, 3, $"IP: {statsInterface1.IPOut}");
                UpdateLabelText(this.interface1InOut, 1, 4, $"ICMP: {statsInterface1.ICMPOut}");
                UpdateLabelText(this.interface1InOut, 1, 5, $"TCP: {statsInterface1.TCPOut}");
                UpdateLabelText(this.interface1InOut, 1, 6, $"UDP: {statsInterface1.UDPOut}");
                UpdateLabelText(this.interface1InOut, 1, 7, $"Total: {statsInterface1.totalOut}");


                var statsInterface2 = packetForwarder.GetStatsInterface2();

                UpdateLabelText(this.interface2InOut, 0, 1, $"ARP: {statsInterface2.ArpIn}");
                UpdateLabelText(this.interface2InOut, 0, 2, $"Ethernet2: {statsInterface2.Ethernet2In}");
                UpdateLabelText(this.interface2InOut, 0, 3, $"IP: {statsInterface2.IPIn}");
                UpdateLabelText(this.interface2InOut, 0, 4, $"ICMP: {statsInterface2.ICMPIn}");
                UpdateLabelText(this.interface2InOut, 0, 5, $"TCP: {statsInterface2.TCPIn}");
                UpdateLabelText(this.interface2InOut, 0, 6, $"UDP: {statsInterface2.UDPIn}");
                UpdateLabelText(this.interface2InOut, 0, 7, $"Total: {statsInterface2.totalIn}");

                UpdateLabelText(this.interface2InOut, 1, 1, $"ARP: {statsInterface2.ArpOut}");
                UpdateLabelText(this.interface2InOut, 1, 2, $"Ethernet2: {statsInterface2.Ethernet2Out}");
                UpdateLabelText(this.interface2InOut, 1, 3, $"IP: {statsInterface2.IPOut}");
                UpdateLabelText(this.interface2InOut, 1, 4, $"ICMP: {statsInterface2.ICMPOut}");
                UpdateLabelText(this.interface2InOut, 1, 5, $"TCP: {statsInterface2.TCPOut}");
                UpdateLabelText(this.interface2InOut, 1, 6, $"UDP: {statsInterface2.UDPOut}");
                UpdateLabelText(this.interface2InOut, 1, 7, $"Total: {statsInterface2.totalOut}");
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (packetForwarder != null)
            {
                packetForwarder.Stop();
            }
        }
    }
}