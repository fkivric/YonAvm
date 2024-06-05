using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static YonAvm.Class.Volant;
using YonAvm.Properties;
using System.Data.SqlClient;
using System.Reflection.Emit;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors.Design;

namespace YonAvm
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private string clientName;
        public static string Username;
        public static string SOCODE = "";
        public frmLogin()
        {
            InitializeComponent();
        }
        private List<eDatabase> lDatabase;
        List<Firma> firmas = new List<Firma>();
        private void frmLogin_Load(object sender, EventArgs e)
        {
            VolXml();
            DataCek();
            DataTable Compny = Sorgu("select distinct CATALOG_NAME from INFORMATION_SCHEMA.SCHEMATA", Settings.Default.connectionstring);
            cmbVolantSirket.Properties.DataSource = firmas;// Compny;
            cmbVolantSirket.Properties.DisplayMember = "COMPANYNAME";
            cmbVolantSirket.Properties.ValueMember = "COMPANYDB";
            cmbVolantSirket.EditValue = Compny.Rows[0]["CATALOG_NAME"];            
        }
        private void VolXml()
        {
            string ConStrg = "";
            try
            {
                clientName = SystemInformation.ComputerName;
                if (!File.Exists("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnection.xml"))
                {
                    throw new Exception("VolErpConnection Dosyası Eksik!");
                }
                XmlTextReader reader = new XmlTextReader("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnection.xml");
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element && reader.Name == "PARAMS") || reader.NodeType != XmlNodeType.Element || !(reader.Name == "DB"))
                    {
                        continue;
                    }
                    eDatabase rDatabase = new eDatabase();
                    try
                    {
                        ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};",
                        reader.GetAttribute("SERVERNAME").TextSifreCoz(),
                        reader.GetAttribute("DATABASE").TextSifreCoz(),
                        reader.GetAttribute("LOGIN").TextSifreCoz(),
                        reader.GetAttribute("PASSWORD").TextSifreCoz());
                    }
                    catch
                    {
                        ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};",
                        reader.GetAttribute("SERVERNAME").ToString(),
                        reader.GetAttribute("DATABASE").ToString(),
                        reader.GetAttribute("LOGIN").ToString(),
                        reader.GetAttribute("PASSWORD").ToString());
                    }

                    Settings.Default.Company = reader.GetAttribute("DATABASE").ToString();
                }
                Settings.Default.connectionstring = ConStrg;
                Settings.Default.Save();

                reader.Close();
            }
            catch (Exception exp)
            {
                XtraMessageBox.Show(exp.Message);
            }
        }
        void DataCek()
        {
            List<AllDataBase> allDatas = new List<AllDataBase>() ;
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connectionstring))
            {
                connection.Open();

                DataTable databases = connection.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    AllDataBase dts = new AllDataBase { DbNAme = database["database_name"].ToString() };
                    allDatas.Add(dts);
                    string databaseName = database["database_name"].ToString();                   
                    var dd = Sorgu(string.Format("select COMPANYVAL,COMPANYNAME from {0}.dbo.COMPANY", database["database_name"]), Settings.Default.connectionstring);
                    if (dd != null)
                    {
                        var ff = new Firma();
                        ff.COMPANYNAME = dd.Rows[0]["COMPANYNAME"].ToString();
                        ff.COMPANYDB = database["database_name"].ToString();
                        if (!firmas.Any(f => f.COMPANYNAME == ff.COMPANYNAME))
                        {
                            firmas.Add(ff);
                        }
                    }
                }
                connection.Close();
            }
        }
        void FirmaBilgileri()
        {
            cmbVolantMagaza.Properties.DataSource = null;
            DataTable Divison = Sorgu("select DIVVAL,DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", Settings.Default.connectionstring);
            cmbVolantMagaza.Properties.DataSource = Divison;
            cmbVolantMagaza.Properties.DisplayMember = "DIVNAME";
            cmbVolantMagaza.Properties.ValueMember = "DIVVAL";
            cmbVolantMagaza.EditValue = Divison.Rows[0]["DIVVAL"];
            var ftp = Sorgu("select MTFTPIP,MTFTPUSER,MTFTPPASSWORD from MANAGEMENT", Settings.Default.connectionstring);
        }
        private void cmbVolantSirket_EditValueChanged(object sender, EventArgs e)
        {
            Settings.Default.connectionstring = Settings.Default.connectionstring.Replace(Settings.Default.Company.ToString(), cmbVolantSirket.EditValue.ToString());
            Settings.Default.Company = cmbVolantSirket.EditValue.ToString();
            Settings.Default.Save();
            FirmaBilgileri();
        }
        public DataTable Sorgu(string sorgu, string connection)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sorgu, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            var yetki = Sorgu(string.Format(@"select * from SOCIAL where SOCODE = '{0}' and SOENTERKEY = '{1}'", txtVolantUser.Text, txtVolantPassword.Text), Settings.Default.connectionstring);

            if (yetki.Rows.Count > 0)
            {
                SOCODE = yetki.Rows[0]["SOCODE"].ToString();
                this.Hide();
                frmMain main = new frmMain();
                main.ShowDialog();
                this.Close();
            }
            else
            {
                XtraMessageBox.Show("Giriş Bilgilerinizi Kontrol Ediniz");
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtVolantPassword_Enter(object sender, EventArgs e)
        {
            if (txtVolantPassword.Text == "Parola")
            {
                txtVolantPassword.Properties.PasswordChar = '*';
                txtVolantPassword.ForeColor = SystemColors.WindowText;
                txtVolantPassword.Text = "";
            }
        }

        private void txtVolantPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                simpleButton2_Click(null, null);
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                this.Dispose();
                Application.Exit();
            }

        }

        private void txtVolantPassword_Leave(object sender, EventArgs e)
        {
            if (txtVolantPassword.Text.Length == 0)
            {
                txtVolantPassword.Properties.PasswordChar = '\0';
                txtVolantPassword.Text = "Parola";
                txtVolantPassword.ForeColor = SystemColors.GrayText;
            }
        }
        private void txtVolantUser_TextChanged(object sender, EventArgs e)
        {
            var sonuc = Sorgu("select SONAME +SPACE(1)+SOSURNAME from SOCIAL left outer join DEPARTMENT on DEPVAL = SODEPART where SOCODE = '" + txtVolantUser.Text + "'", Settings.Default.connectionstring);
            if (sonuc.Rows.Count > 0)
            {
                togsKullanici.IsOn = true; 
                togsKullanici.Properties.OnText = sonuc.Rows[0][0].ToString();
            }
            else
            {
                togsKullanici.IsOn = false;
            }
        }
    }
}