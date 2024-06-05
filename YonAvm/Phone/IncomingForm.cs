using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace YonAvm
{
	public partial class IncomingForm : Form
	{
		public IncomingForm()
		{
			InitializeComponent();
		}
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
		private void buttonYes_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Yes;
			this.Close();
		}

		private void buttonNo_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.No;
			this.Close();
		}

        private void IncomingForm_Load(object sender, EventArgs e)
        {
            string q = string.Format("select WebSiparis.ID as [Sipariþ No], convert(char(10), SiparisTarihi, 103) as [Sipariþ Tarihi], AdiSoyadi as [Müþteri] " +
                "from WebSiparis " +
                "inner join Uye on Uye.ID = UyeID " +
                "where CepTelefonu like '%{0}%' " +
                "order by 3, 2 desc", textBoxCaller.Text);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            sql.Close();
            gridControl1.DataSource = dt;
        }
    }
}