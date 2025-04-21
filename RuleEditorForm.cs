using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using SharpPcap.LibPcap;

namespace Projekt
{
    public partial class RuleEditorForm : Form
    {
        public ComboBox comboInterface, comboDirection;
        private ComboBox comboAction, comboProtocol;
        private TextBox txtSrcMAC, txtDstMAC, txtSrcIP, txtDstIP, txtSrcPort, txtDstPort;
        private Button btnOK, btnCancel;

        public ACE NewRule { get; private set; }

        public RuleEditorForm(List<LibPcapLiveDevice> interfaces)
        {
            InitializeComponents(interfaces);
        }

        private void InitializeComponents(List<LibPcapLiveDevice> interfaces)
        {
            this.Text = "Add Rule";
            this.Size = new Size(400, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
           


            var labels = new[]
            {
                "Interface", "Direction", "Action", "Protocol",
                "Source MAC", "Destination MAC",
                "Source IP", "Destination IP",
                "Source Port", "Destination Port"
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label
                {
                    Text = labels[i] + ":",
                    Left = 20,
                    Top = 20 + i * 30,
                    Width = 120
                };
                this.Controls.Add(lbl);
            }
            comboInterface = new ComboBox { Left = 150, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            comboDirection = new ComboBox { Left = 150, Top = 50, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            comboAction = new ComboBox { Left = 150, Top = 80, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            comboProtocol = new ComboBox { Left = 150, Top = 110, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            txtSrcMAC = new TextBox { Left = 150, Top = 140, Width = 200 };
            txtDstMAC = new TextBox { Left = 150, Top = 170, Width = 200 };
            txtSrcIP = new TextBox { Left = 150, Top = 200, Width = 200 };
            txtDstIP = new TextBox { Left = 150, Top = 230, Width = 200 };
            txtSrcPort = new TextBox { Left = 150, Top = 260, Width = 200 };
            txtDstPort = new TextBox { Left = 150, Top = 290, Width = 200 };

            btnOK = new Button { Text = "OK", Left = 80, Top = 340, Width = 100 };
            btnCancel = new Button { Text = "Cancel", Left = 200, Top = 340, Width = 100 };

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            for (int i = 0; i < interfaces.Count; i++)
            {
                comboInterface.Items.Add($"Interface {i + 1} ({interfaces[i].Interface.FriendlyName})");
            }
            comboDirection.Items.Add("In"); comboDirection.Items.Add("Out");
            comboAction.Items.Add("Allow"); comboAction.Items.Add("Deny");
            comboProtocol.Items.AddRange(new[] { "Any", "Ethernet", "IP", "ARP", "ICMP", "TCP", "UDP" });

            comboProtocol.SelectedIndex = 0;
            comboInterface.SelectedIndex = 0;
            comboDirection.SelectedIndex = 0;
            comboAction.SelectedIndex = 0;

            this.Controls.AddRange(new Control[]
            {
                comboInterface, comboDirection, comboAction, comboProtocol,
                txtSrcMAC, txtDstMAC, txtSrcIP, txtDstIP, txtSrcPort, txtDstPort,
                btnOK, btnCancel
            });
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                NewRule = new ACE
                {
                    RuleAction = comboAction.SelectedItem?.ToString() ?? "Allow",
                    RuleProtocol = comboProtocol.SelectedItem?.ToString() ?? "Any",
                    SourceMAC = string.IsNullOrWhiteSpace(txtSrcMAC.Text) ? null : PhysicalAddress.Parse(txtSrcMAC.Text.Replace(":", "-").ToUpper()),
                    DestinationMAC = string.IsNullOrWhiteSpace(txtDstMAC.Text) ? null : PhysicalAddress.Parse(txtDstMAC.Text.Replace(":", "-").ToUpper()),
                    SourceIP = string.IsNullOrWhiteSpace(txtSrcIP.Text) ? null : IPAddress.Parse(txtSrcIP.Text),
                    DestinationIP = string.IsNullOrWhiteSpace(txtDstIP.Text) ? null : IPAddress.Parse(txtDstIP.Text),
                    SourcePort = ushort.TryParse(txtSrcPort.Text, out var srcPort) ? (ushort?)srcPort : null,
                    DestinationPort = ushort.TryParse(txtDstPort.Text, out var dstPort) ? (ushort?)dstPort : null

                };

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
