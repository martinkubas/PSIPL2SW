using System.Drawing;
using System.Windows.Forms;

namespace Projekt
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblInterface1;
        private System.Windows.Forms.Label lblInterface2;

        private System.Windows.Forms.ComboBox comboBoxInterface1;
        private System.Windows.Forms.ComboBox comboBoxInterface2;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnResetInt1;
        private System.Windows.Forms.Button btnResetInt2;
        private System.Windows.Forms.Button btnResetMAC;


        private System.Windows.Forms.Timer timerStatistics;

        private System.Windows.Forms.TableLayoutPanel interface1InOut;
        private System.Windows.Forms.TableLayoutPanel interface2InOut;

        private System.Windows.Forms.DataGridView macAddressTableGrid;
        private System.Windows.Forms.NumericUpDown agingTimerDuration;
        private System.Windows.Forms.Label lblAgingTimerDuration;
        private System.Windows.Forms.Timer agingTimer;

        private System.Windows.Forms.ListView rulesListView;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnRemoveRule;
        private System.Windows.Forms.Button btnCheckRuleForInt;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.lblInterface1 = new System.Windows.Forms.Label();
            this.lblInterface2 = new System.Windows.Forms.Label();

            this.comboBoxInterface1 = new System.Windows.Forms.ComboBox();
            this.comboBoxInterface2 = new System.Windows.Forms.ComboBox();

            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnResetInt1 = new System.Windows.Forms.Button();
            this.btnResetInt2 = new System.Windows.Forms.Button();

            this.timerStatistics = new System.Windows.Forms.Timer(this.components);

            this.interface1InOut = new TableLayoutPanel();
            this.interface2InOut = new TableLayoutPanel();

            this.macAddressTableGrid = new System.Windows.Forms.DataGridView();
            this.agingTimerDuration = new System.Windows.Forms.NumericUpDown();
            this.lblAgingTimerDuration = new System.Windows.Forms.Label();
            this.btnResetMAC = new System.Windows.Forms.Button();
            this.agingTimer = new System.Windows.Forms.Timer(this.components);

            this.rulesListView = new System.Windows.Forms.ListView();
            this.btnAddRule = new System.Windows.Forms.Button();
            this.btnRemoveRule = new System.Windows.Forms.Button();
            this.btnCheckRuleForInt = new System.Windows.Forms.Button();


            this.SuspendLayout();
            // 
            // lblInterface1
            // 
            this.lblInterface1.Location = new System.Drawing.Point(140, 30);
            this.lblInterface1.Size = new System.Drawing.Size(139, 29);
            this.lblInterface1.AutoSize = true;
            this.lblInterface1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblInterface1.Text = "Interface 1:";
            // 
            // lblInterface2
            // 
            this.lblInterface2.Location = new System.Drawing.Point(1020, 30);
            this.lblInterface2.Size = new System.Drawing.Size(139, 29);
            this.lblInterface2.AutoSize = true;
            this.lblInterface2.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblInterface2.Text = "Interface 2:";
            // 
            // comboBoxInterface1
            // 
            this.comboBoxInterface1.Location = new System.Drawing.Point(50, 60);
            this.comboBoxInterface1.Size = new System.Drawing.Size(300, 24);
            this.comboBoxInterface1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInterface1.FormattingEnabled = true;
            // 
            // comboBoxInterface2
            // 
            this.comboBoxInterface2.Location = new System.Drawing.Point(930, 60);
            this.comboBoxInterface2.Size = new System.Drawing.Size(300, 24);
            this.comboBoxInterface2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInterface2.FormattingEnabled = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(500, 60);
            this.btnStart.Size = new System.Drawing.Size(100, 23);
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.TabStop = false;
            //
            // btnStop
            //
            this.btnStop.Location = new System.Drawing.Point(700, 60);
            this.btnStop.Size = new System.Drawing.Size(100, 23);
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.TabStop = false;
            //
            // btnResetInt1
            //
            this.btnResetInt1.Location = new System.Drawing.Point(150, 300);
            this.btnResetInt1.Size = new System.Drawing.Size(100, 23);
            this.btnResetInt1.Name = "btnResetInt1";
            this.btnResetInt1.Text = "Reset";
            this.btnResetInt1.UseVisualStyleBackColor = true;
            this.btnResetInt1.Click += new System.EventHandler(this.btnReset_Click);
            this.btnStop.TabStop = false;
            //
            // btnResetInt2
            //
            this.btnResetInt2.Location = new System.Drawing.Point(1030, 300);
            this.btnResetInt2.Size = new System.Drawing.Size(100, 23);
            this.btnResetInt2.Name = "btnResetInt2";
            this.btnResetInt2.Text = "Reset";
            this.btnResetInt2.UseVisualStyleBackColor = true;
            this.btnResetInt2.Click += new System.EventHandler(this.btnReset_Click);
            this.btnStop.TabStop = false;
            //
            // interface1InOut
            //
            this.interface1InOut.ColumnCount = 2;
            this.interface1InOut.RowCount = 8;
            this.interface1InOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.interface1InOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            this.interface1InOut.Size = new System.Drawing.Size(350, 150);
            this.interface1InOut.Location = new System.Drawing.Point(50, 120);
            this.interface1InOut.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, this.interface1InOut.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };

            this.interface1InOut.Controls.Add(new Label { Text = "IN", AutoSize = true, Font = new Font("Courier New", 12F, System.Drawing.FontStyle.Bold) }, 0, 0);
            this.interface1InOut.Controls.Add(new Label { Text = "OUT", AutoSize = true, Font = new Font("Courier New", 12F, System.Drawing.FontStyle.Bold) }, 1, 0);


            this.interface1InOut.Controls.Add(new Label { Text = "ARP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 1);
            this.interface1InOut.Controls.Add(new Label { Text = "Ethernet2: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 2);
            this.interface1InOut.Controls.Add(new Label { Text = "IP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 3);
            this.interface1InOut.Controls.Add(new Label { Text = "ICMP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 4);
            this.interface1InOut.Controls.Add(new Label { Text = "TCP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 5);
            this.interface1InOut.Controls.Add(new Label { Text = "UDP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 6);
            this.interface1InOut.Controls.Add(new Label { Text = "Total: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 7);


            this.interface1InOut.Controls.Add(new Label { Text = "ARP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 1);
            this.interface1InOut.Controls.Add(new Label { Text = "Ethernet2: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 2);
            this.interface1InOut.Controls.Add(new Label { Text = "IP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 3);
            this.interface1InOut.Controls.Add(new Label { Text = "ICMP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 4);
            this.interface1InOut.Controls.Add(new Label { Text = "TCP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 5);
            this.interface1InOut.Controls.Add(new Label { Text = "UDP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 6);
            this.interface1InOut.Controls.Add(new Label { Text = "Total: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 7);
            //
            // interface2InOut
            //
            this.interface2InOut.ColumnCount = 2;
            this.interface2InOut.RowCount = 8;
            this.interface2InOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.interface2InOut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            this.interface2InOut.Size = new System.Drawing.Size(350, 150);
            this.interface2InOut.Location = new System.Drawing.Point(880, 120);
            this.interface2InOut.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, this.interface2InOut.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
            };

            this.interface2InOut.Controls.Add(new Label { Text = "IN", AutoSize = true, Font = new Font("Courier New", 12F, System.Drawing.FontStyle.Bold) }, 0, 0);
            this.interface2InOut.Controls.Add(new Label { Text = "OUT", AutoSize = true, Font = new Font("Courier New", 12F, System.Drawing.FontStyle.Bold) }, 1, 0);


            this.interface2InOut.Controls.Add(new Label { Text = "ARP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 1);
            this.interface2InOut.Controls.Add(new Label { Text = "Ethernet2: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 2);
            this.interface2InOut.Controls.Add(new Label { Text = "IP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 3);
            this.interface2InOut.Controls.Add(new Label { Text = "ICMP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 4);
            this.interface2InOut.Controls.Add(new Label { Text = "TCP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 5);
            this.interface2InOut.Controls.Add(new Label { Text = "UDP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 6);
            this.interface2InOut.Controls.Add(new Label { Text = "Total: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 0, 7);


            this.interface2InOut.Controls.Add(new Label { Text = "ARP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 1);
            this.interface2InOut.Controls.Add(new Label { Text = "Ethernet2: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 2);
            this.interface2InOut.Controls.Add(new Label { Text = "IP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 3);
            this.interface2InOut.Controls.Add(new Label { Text = "ICMP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 4);
            this.interface2InOut.Controls.Add(new Label { Text = "TCP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 5);
            this.interface2InOut.Controls.Add(new Label { Text = "UDP: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 6);
            this.interface2InOut.Controls.Add(new Label { Text = "Total: 0", AutoSize = true, Font = new Font("Courier New", 12F) }, 1, 7);
            // 
            // timerStatistics
            // 
            this.timerStatistics.Interval = 1000;
            this.timerStatistics.Tick += new System.EventHandler(this.timerStatistics_Tick);
            // 
            // macAddressTableGrid
            // 
            this.macAddressTableGrid.AllowUserToAddRows = false;
            this.macAddressTableGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.macAddressTableGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.macAddressTableGrid.Columns.Add("MACAddress", "MAC Address"); 
            this.macAddressTableGrid.Columns.Add("Interface", "Interface");  
            this.macAddressTableGrid.Columns.Add("LastSeen", "Last Seen");    
            this.macAddressTableGrid.Location = new System.Drawing.Point(420, 120);
            this.macAddressTableGrid.Name = "macAddressTableGrid";
            this.macAddressTableGrid.ReadOnly = true;
            this.macAddressTableGrid.RowHeadersVisible = false;
            this.macAddressTableGrid.RowTemplate.Height = 24;
            this.macAddressTableGrid.Size = new System.Drawing.Size(450, 200);
            // 
            // agingTimerDuration
            // 
            this.agingTimerDuration.Location = new System.Drawing.Point(770, 330);
            this.agingTimerDuration.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.agingTimerDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.agingTimerDuration.Name = "agingTimerDuration";
            this.agingTimerDuration.Size = new System.Drawing.Size(100, 22);
            this.agingTimerDuration.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});

            // 
            // lblAgingTimerDuration
            // 
            this.lblAgingTimerDuration.AutoSize = true;
            this.lblAgingTimerDuration.Location = new System.Drawing.Point(605, 333);
            this.lblAgingTimerDuration.Name = "lblAgingTimerDuration";
            this.lblAgingTimerDuration.Size = new System.Drawing.Size(200, 16);
            this.lblAgingTimerDuration.Text = "Aging Timer Duration (seconds):";

            // 
            // agingTimer
            // 
            this.agingTimer.Interval = 1000;
            this.agingTimer.Tick += new System.EventHandler(this.AgingTimer_Tick);
            //
            //btnResetMAC
            //
            this.btnResetMAC.Location = new System.Drawing.Point(420, 330);
            this.btnResetMAC.Size = new System.Drawing.Size(100, 23);
            this.btnResetMAC.Name = "btnResetMAC";
            this.btnResetMAC.Text = "Reset";
            this.btnResetMAC.UseVisualStyleBackColor = true;
            this.btnResetMAC.Click += new System.EventHandler(this.btnResetMAC_Click);
            // 
            // rulesListView
            // 
            this.rulesListView.Location = new System.Drawing.Point(50, 400);
            this.rulesListView.Size = new System.Drawing.Size(700, 150);
            this.rulesListView.View = System.Windows.Forms.View.Details;
            this.rulesListView.FullRowSelect = true;
            this.rulesListView.GridLines = true;
            this.rulesListView.Columns.Add("Interface", 100);
            this.rulesListView.Columns.Add("Action", 100);
            this.rulesListView.Columns.Add("Direction", 100);
            this.rulesListView.Columns.Add("Value", 400);
            // 
            // btnAddRule
            // 
            this.btnAddRule.Location = new System.Drawing.Point(50, 560);
            this.btnAddRule.Size = new System.Drawing.Size(100, 30);
            this.btnAddRule.Text = "Add Rule";
            this.btnAddRule.UseVisualStyleBackColor = true;
            this.btnAddRule.Click += new System.EventHandler(this.btnAddRule_Click);

            // 
            // btnRemoveRule
            // 
            this.btnRemoveRule.Location = new System.Drawing.Point(650, 560);
            this.btnRemoveRule.Size = new System.Drawing.Size(100, 30);
            this.btnRemoveRule.Text = "Remove Rule";
            this.btnRemoveRule.UseVisualStyleBackColor = true;
            this.btnRemoveRule.Click += new System.EventHandler(this.btnRemoveRule_Click);
            //
            // btnCheckRuleForInt
            //
            this.btnCheckRuleForInt.Location = new System.Drawing.Point(250, 560);
            this.btnCheckRuleForInt.Size = new System.Drawing.Size(200, 30);
            this.btnCheckRuleForInt.Text = "Check rules for whole Interface";
            this.btnCheckRuleForInt.UseVisualStyleBackColor = true;
            this.btnCheckRuleForInt.Click += new System.EventHandler(this.btnCheckRuleForInt_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.BackColor = System.Drawing.Color.LightGray;

            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnResetInt1);
            this.Controls.Add(this.btnResetInt2);

            this.Controls.Add(this.interface1InOut);
            this.Controls.Add(this.interface2InOut);

            this.Controls.Add(this.comboBoxInterface2);
            this.Controls.Add(this.comboBoxInterface1);

            this.Controls.Add(this.lblInterface2);
            this.Controls.Add(this.lblInterface1);

            this.Controls.Add(this.macAddressTableGrid);
            this.Controls.Add(this.agingTimerDuration);
            this.Controls.Add(this.lblAgingTimerDuration);
            this.Controls.Add(this.btnResetMAC);

            this.Controls.Add(this.rulesListView);
            this.Controls.Add(this.btnAddRule);
            this.Controls.Add(this.btnRemoveRule);
            this.Controls.Add(this.btnCheckRuleForInt);

            this.Name = "Form1";
            this.Text = "Ethernet Hub";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}