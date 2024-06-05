using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YonAvm
{
	public partial class ActivationForm : Form
	{
		public string m_licUserId;
		public string m_licKey;
		
		public ActivationForm()
		{
			InitializeComponent();
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			m_licUserId = licUserIdTextBox.Text;
			m_licKey    = licKeyTextBox.Text;
			if((m_licUserId.Length == 0) || (m_licKey.Length == 0)) return;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
        private void ActivationForm_Load(object sender, EventArgs e)
        {
            licUserIdTextBox.Text = Properties.Settings.Default.DidlicUserId;
            licKeyTextBox.Text = Properties.Settings.Default.DidlicKey;
        }
    }
}