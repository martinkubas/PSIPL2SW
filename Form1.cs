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
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            LoadDevices();
        }

        private void LoadDevices()
        {
            // Get all available network devices
            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count == 0)
            {
                MessageBox.Show("No network devices found.");
                return;
            }

            // Populate the ComboBoxes with available devices
            comboBoxInterface1.DataSource = devices.ToList();
            comboBoxInterface1.DisplayMember = "Interface.FriendlyName";

            comboBoxInterface2.DataSource = devices.ToList(); // Create a new list to avoid binding issues
            comboBoxInterface2.DisplayMember = "Interface.FriendlyName";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Get the selected interfaces
            interface1 = comboBoxInterface1.SelectedItem as LibPcapLiveDevice;
            interface2 = comboBoxInterface2.SelectedItem as LibPcapLiveDevice;

            if (interface1 == null || interface2 == null)
            {
                MessageBox.Show("Please select both interfaces.");
                return;
            }

            if (interface1 == interface2)
            {
                MessageBox.Show("Interface 1 and Interface 2 must be different.");
                return;
            }

            try
            {
                // Initialize the packet forwarder with the selected interfaces
                packetForwarder = new PacketForwarder(interface1, interface2);

                // Start the packet forwarder
                packetForwarder.Start();

                // Update UI
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                // Start the timer to update the UI with packet statistics
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

                // Stop the timer
                timerStatistics.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping packet forwarder: {ex.Message}");
            }
        }

        private void timerStatistics_Tick(object sender, EventArgs e)
        {
            if (packetForwarder != null)
            {
                // Update statistics for Interface 1
                var statsInterface1 = packetForwarder.GetStatsInterface1();
                lblInterface1Incoming.Text = $"Interface 1 Incoming: {statsInterface1.IncomingPackets}";
                lblInterface1Outgoing.Text = $"Interface 1 Outgoing: {statsInterface1.OutgoingPackets}";

                // Update statistics for Interface 2
                var statsInterface2 = packetForwarder.GetStatsInterface2();
                lblInterface2Incoming.Text = $"Interface 2 Incoming: {statsInterface2.IncomingPackets}";
                lblInterface2Outgoing.Text = $"Interface 2 Outgoing: {statsInterface2.OutgoingPackets}";
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