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
    public class ACLViewerForm : Form
    {
        private TextBox txtRules;

        public ACLViewerForm()
        {
            InitializeComponents();
        }
        private void InitializeComponents()
        {
            this.Text = "ACL Rules Viewer";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            txtRules = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                WordWrap = true
            };

            this.Controls.Add(txtRules);
        }

        public void SetRulesText(string rulesText)
        {
            txtRules.Text = rulesText;
        }
    }
}
