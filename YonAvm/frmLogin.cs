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
using System.Net.Http;
using Newtonsoft.Json;
using YonAvm.Class;
using System.Text.RegularExpressions;

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

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                lblversion.Text = "Version : " + ad.CurrentVersion.Major + "." + ad.CurrentVersion.Minor + "." + ad.CurrentVersion.Build + "." + ad.CurrentVersion.Revision;
                //version = ad.CurrentVersion.Revision.ToString();
            }
            else
            {
                string _s1 = Application.ProductVersion; // versiyon
                lblversion.Text = "Version : " + _s1;
            }
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
                //XtraMessageBox.Show(exp.Message);
                DialogResult result = XtraMessageBox.Show(exp.Message + "\r\nHarici Baglantı Kullanılsın mı", "Seçim", MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Properties.Settings.Default.connectionstring = "Server=212.174.235.106,1436;Database=VDB_YON01;User Id=sa;Password=MagicUser2023!;";
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Application.Exit();
                }
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
        public async void DidToken()
        {
            try
            {
                var sorgu = new DidKey
                {
                    username = "entegref",
                    password = "M@gicUs€r2023!H",
                    centralcode = "yon.didtelekom.com.tr",
                    module = "ENTEGREF",
                    function = "GETKEY"
                };
                var json = JsonConvert.SerializeObject(sorgu);
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // İstek oluştur
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Properties.Settings.Default.DidApiUrl);
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                        // İsteği gönder ve yanıtı al
                        HttpResponseMessage response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        // Yanıtı oku
                        string responseContent = await response.Content.ReadAsStringAsync();
                        DidKeyreturn myDeserializedClass = JsonConvert.DeserializeObject<DidKeyreturn>(responseContent);
                        if (myDeserializedClass.result == "SUCCESS")
                        {
                            Properties.Settings.Default.DidApikey = myDeserializedClass.apikey;
                            Properties.Settings.Default.DidCentraluuid = myDeserializedClass.centraluuid;
                            Properties.Settings.Default.DidCentralcode = myDeserializedClass.centralcode;
                            Properties.Settings.Default.Save();
                            //MessageBox.Show(myDeserializedClass.description);
                        }
                    }
                    catch (Exception ex)
                    {

                        // Hata durumunda gerekli işlemleri yapabilirsiniz
                        XtraMessageBox.Show("DiDTelekom Token alınırken Hata: " + ex.Message);

                    }
                }
            }
            catch (Exception ex2)
            {
                XtraMessageBox.Show(ex2.Message);
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
            var data2last = GetDatabaseFromConnectionString(Settings.Default.connectionstring2.ToString());
            var data2new = cmbVolantSirket.EditValue.ToString().Replace("1", "2");

            Settings.Default.connectionstring = Settings.Default.connectionstring.Replace(Settings.Default.Company.ToString(), cmbVolantSirket.EditValue.ToString());
            Settings.Default.connectionstring2 = Settings.Default.connectionstring2.Replace(data2last, data2new);
            Settings.Default.Company = cmbVolantSirket.EditValue.ToString();
            Settings.Default.Save();
            FirmaBilgileri();

            string company = cmbVolantSirket.EditValue.ToString();
            if (company.Contains("YON"))
            {
                pictureEdit1.Image = Properties.Resources.YON_AVM_400;
            }
            else if (company.Contains("KAMALAR"))
            {
                pictureEdit1.Image = Properties.Resources.Kamalar_logo;
            }
        }
        private string GetDatabaseFromConnectionString(string connectionString)
        {
            var match = Regex.Match(connectionString, @"Database = ([^;]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
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
                DidToken();
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

        private void togsKullanici_Toggled(object sender, EventArgs e)
        {
            //if (togsKullanici.IsOn)
            //{
            //    Properties.Settings.Default.connectionstring = "Server=212.174.235.106,1436;Database=VDB_YON01;User Id=sa;Password=MagicUser2023!;";
            //    Properties.Settings.Default.Save();
            //    //DataCek();
            //    FirmaBilgileri();
            //    DataTable Compny = Sorgu("select distinct CATALOG_NAME from INFORMATION_SCHEMA.SCHEMATA", Settings.Default.connectionstring);
            //    cmbVolantSirket.Properties.DataSource = firmas;// Compny;
            //    cmbVolantSirket.Properties.DisplayMember = "COMPANYNAME";
            //    cmbVolantSirket.Properties.ValueMember = "COMPANYDB";
            //    cmbVolantSirket.EditValue = Compny.Rows[0]["CATALOG_NAME"];
            //}
            //else
            //{
            //    Properties.Settings.Default.connectionstring = "Server=192.168.4.24;Database=VDB_YON01;User Id=sa;Password=MagicUser2023!;";
            //    Properties.Settings.Default.Save();
            //    //DataCek();
            //    FirmaBilgileri();

            //    DataTable Compny = Sorgu("select distinct CATALOG_NAME from INFORMATION_SCHEMA.SCHEMATA", Settings.Default.connectionstring);
            //    cmbVolantSirket.Properties.DataSource = firmas;// Compny;
            //    cmbVolantSirket.Properties.DisplayMember = "COMPANYNAME";
            //    cmbVolantSirket.Properties.ValueMember = "COMPANYDB";
            //    cmbVolantSirket.EditValue = Compny.Rows[0]["CATALOG_NAME"];
            //}
        }
    }
}