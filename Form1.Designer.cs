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
        private System.Windows.Forms.Timer timerStatistics;
        private System.Windows.Forms.Label lblInterface1Incoming;
        private System.Windows.Forms.Label lblInterface1Outgoing;
        private System.Windows.Forms.Label lblInterface2Incoming;
        private System.Windows.Forms.Label lblInterface2Outgoing;

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
            this.timerStatistics = new System.Windows.Forms.Timer(this.components);
            this.lblInterface1Incoming = new System.Windows.Forms.Label();
            this.lblInterface1Outgoing = new System.Windows.Forms.Label();
            this.lblInterface2Incoming = new System.Windows.Forms.Label();
            this.lblInterface2Outgoing = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblInterface1
            this.lblInterface1.AutoSize = true;
            this.lblInterface1.Location = new System.Drawing.Point(20, 20);
            this.lblInterface1.Name = "lblInterface1";
            this.lblInterface1.Size = new System.Drawing.Size(60, 13);
            this.lblInterface1.TabIndex = 0;
            this.lblInterface1.Text = "Interface 1:";

            // lblInterface2
            this.lblInterface2.AutoSize = true;
            this.lblInterface2.Location = new System.Drawing.Point(20, 60);
            this.lblInterface2.Name = "lblInterface2";
            this.lblInterface2.Size = new System.Drawing.Size(60, 13);
            this.lblInterface2.TabIndex = 1;
            this.lblInterface2.Text = "Interface 2:";

            // comboBoxInterface1
            this.comboBoxInterface1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInterface1.FormattingEnabled = true;
            this.comboBoxInterface1.Location = new System.Drawing.Point(130, 17);
            this.comboBoxInterface1.Name = "comboBoxInterface1";
            this.comboBoxInterface1.Size = new System.Drawing.Size(1100, 21);
            this.comboBoxInterface1.TabIndex = 2;

            // comboBoxInterface2
            this.comboBoxInterface2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInterface2.FormattingEnabled = true;
            this.comboBoxInterface2.Location = new System.Drawing.Point(130, 57);
            this.comboBoxInterface2.Name = "comboBoxInterface2";
            this.comboBoxInterface2.Size = new System.Drawing.Size(1100, 21);
            this.comboBoxInterface2.TabIndex = 3;

            // btnStart
            this.btnStart.Location = new System.Drawing.Point(20, 100);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start Hub";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // btnStop
            this.btnStop.Location = new System.Drawing.Point(120, 100);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop Hub";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // timerStatistics
            this.timerStatistics.Interval = 1000; // 1 second
            this.timerStatistics.Tick += new System.EventHandler(this.timerStatistics_Tick);

            // lblInterface1Incoming
            this.lblInterface1Incoming.AutoSize = true;
            this.lblInterface1Incoming.Location = new System.Drawing.Point(20, 140);
            this.lblInterface1Incoming.Name = "lblInterface1Incoming";
            this.lblInterface1Incoming.Size = new System.Drawing.Size(120, 13);
            this.lblInterface1Incoming.TabIndex = 6;
            this.lblInterface1Incoming.Text = "Interface 1 Incoming: 0";

            // lblInterface1Outgoing
            this.lblInterface1Outgoing.AutoSize = true;
            this.lblInterface1Outgoing.Location = new System.Drawing.Point(20, 160);
            this.lblInterface1Outgoing.Name = "lblInterface1Outgoing";
            this.lblInterface1Outgoing.Size = new System.Drawing.Size(120, 13);
            this.lblInterface1Outgoing.TabIndex = 7;
            this.lblInterface1Outgoing.Text = "Interface 1 Outgoing: 0";

            // lblInterface2Incoming
            this.lblInterface2Incoming.AutoSize = true;
            this.lblInterface2Incoming.Location = new System.Drawing.Point(20, 180);
            this.lblInterface2Incoming.Name = "lblInterface2Incoming";
            this.lblInterface2Incoming.Size = new System.Drawing.Size(120, 13);
            this.lblInterface2Incoming.TabIndex = 8;
            this.lblInterface2Incoming.Text = "Interface 2 Incoming: 0";

            // lblInterface2Outgoing
            this.lblInterface2Outgoing.AutoSize = true;
            this.lblInterface2Outgoing.Location = new System.Drawing.Point(20, 200);
            this.lblInterface2Outgoing.Name = "lblInterface2Outgoing";
            this.lblInterface2Outgoing.Size = new System.Drawing.Size(120, 13);
            this.lblInterface2Outgoing.TabIndex = 9;
            this.lblInterface2Outgoing.Text = "Interface 2 Outgoing: 0";

            // Form1
            this.ClientSize = new System.Drawing.Size(450, 250); // Increase height to accommodate new labels
            this.Controls.Add(this.lblInterface2Outgoing);
            this.Controls.Add(this.lblInterface2Incoming);
            this.Controls.Add(this.lblInterface1Outgoing);
            this.Controls.Add(this.lblInterface1Incoming);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.comboBoxInterface2);
            this.Controls.Add(this.comboBoxInterface1);
            this.Controls.Add(this.lblInterface2);
            this.Controls.Add(this.lblInterface1);
            this.Name = "Form1";
            this.Text = "Ethernet Hub";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}