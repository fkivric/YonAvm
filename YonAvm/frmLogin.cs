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
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using FoxLearn.License;

namespace YonAvm
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private string clientName;
        public static string Username;
        public static string SOCODE = "";
        public static string DIVVAL = "";
        public frmLogin()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://lisans.entegref.com/");
            if (VKN == null)
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                if (!string.IsNullOrEmpty(key.GetValue("ApplicationVKN").ToString()))
                {
                    VKN = key.GetValue("ApplicationVKN").ToString();
                }
                else
                {
                    VKN = Properties.Settings.Default.VKN;
                }
            }
        }
        private List<eDatabase> lDatabase;
        List<Firma> firmas = new List<Firma>();
        string version;
        public static string ProductName = "";
        public static string VKN = null;
        public static int lisansKalan = 0;
        private readonly HttpClient httpClient;
        private async void frmLogin_Load(object sender, EventArgs e)
        {
            Entegref client = new Entegref();
            try
            {
                ProductName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı
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
                    version = ad.CurrentVersion.Revision.ToString();
                }
                else
                {
                    string _s1 = Application.ProductVersion; // versiyon
                    lblversion.Text = "Version : " + _s1;
                    version = _s1;
                }
                RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                string[] valueNames = key2.GetValueNames();
                if (!valueNames.Contains("ApplicationSetupComplate"))
                {
                    if (!valueNames.Contains("ComputerLisansingID"))
                    {
                        CustomMessageBox.ShowMessage("Lütfen Bekleyin: ", "Propgram Lisanslanıyor", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        var pcİsmi = key2.GetValue("ComputerName");
                        var pcModeli = key2.GetValue("ComputerID");
                        var Cpuid = key2.GetValue("CPU");
                        var Motherboardid = key2.GetValue("motherboardid");
                        string response = await client.UpdateLicensingUser(VKN, pcİsmi.ToString(), pcModeli.ToString(), version, ProductName);
                        //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                        List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                        var ConnectionLisansingID = myDeserializedClass[0].message;
                        string response2 = await client.UpdateLicensing(VKN, ConnectionLisansingID.ToString(), Cpuid.ToString(), Motherboardid.ToString(), ProductName);
                        List<Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Sonuc>>(response2);
                        if (!valueNames.Contains("ApplicationSecretPhase"))
                        {
                            //string SecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();

                            //if (string.IsNullOrEmpty(SecretPhase))
                            //{
                            await Lisansing(VKN);
                            //await Task.Delay(1000);
                            RegistryKey lisans = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                            SKGL.Validate validate = new SKGL.Validate();
                            validate.secretPhase = VKN;
                            validate.Key = lisans.GetValue("ApplicationSecretPhase").ToString();
                            txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate.CreationDate.ToShortDateString();
                            txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate.ExpireDate.ToShortDateString();
                            txtLisansing1.Text = "Kalan Gün : \r\n " + validate.DaysLeft;
                            lisansKalan = validate.DaysLeft;

                            if (Properties.Settings.Default.EntegrefSecretPhase == "")
                            {
                                key2.SetValue("ApplicationSecretPhase", Properties.Settings.Default.EntegrefSecretPhase);// Properties.Settings.Default.EntegrefSecretPhase);
                            }
                            if (validate.DaysLeft > 20)
                            {
                                pnlLisans.Visible = false;
                                this.Size = new Size(718, 325);
                            }
                            //}
                        }
                        key2.SetValue("ComputerLisansingID", myDeserializedClass2[0].message);
                        key2.SetValue("ApplicationSetupComplate", true);
                        key2.SetValue("ApplicationComputerID", ComputerInfo.GetComputerId());
                        key2.Close();


                    }
                }
                else
                {
                    var vknvar = key2.GetValue("ApplicationVKN");
                    var pcİsmi = key2.GetValue("ComputerName");
                    string response = await client.Versiyon(vknvar.ToString(), pcİsmi.ToString(), ProductName);
                    //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                    List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                    if (myDeserializedClass[0].message == "Yok")
                    {
                        string response2 = await client.Versiyon(vknvar.ToString(), pcİsmi.ToString(), ProductName);
                    }
                    else
                    {
                        int new_version = int.Parse(myDeserializedClass[0].message.Replace(".", ""));
                        int last_version = int.Parse(version.Replace(".", ""));
                        if (new_version > last_version)
                        {
                            this.Enabled = false;
                            try
                            {
                                key2.DeleteSubKey("ApplicationSecretPhase");
                                key2.DeleteSubKey("ApplicationSetupComplate");
                                key2.DeleteSubKey("ComputerLisansingID");

                                string pathToUpdater = @"updater.exe"; // updater.exe dosyasının adını belirtin

                                ProcessStartInfo startInfo = new ProcessStartInfo
                                {
                                    FileName = pathToUpdater,
                                    WindowStyle = ProcessWindowStyle.Normal // İsteğe bağlı: Pencere stili
                                };

                                Process.Start(startInfo);
                                Application.Exit();
                            }
                            catch (Exception ex)
                            {
                                CustomMessageBox.ShowMessage("Güncelleme Var Hata = " + ex.Message, "Güncelleme var Oto güncelleme Çalışmadı! Elle güncelleyiniz.\r\n Seçenekler = \r\n 1-) C:\\Program Files (x86)\\Entegref Yazılım Tic. Ltd.Şti\\EntegreF Connector\\updater.exe dosya yoluna gidip güncelleme programını çalıştırınız.\r\n 2-) Başlat butonundan Updater aratarak çıkanı çalıştırınız", this, "Güncelleme Kontrol", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Enabled = true;
                            }
                        }
                    }
                    SKGL.Validate validate = new SKGL.Validate();
                    validate.secretPhase = VKN;
                    validate.Key = key2.GetValue("ApplicationSecretPhase").ToString();
                    txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate.CreationDate.ToShortDateString();
                    txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate.ExpireDate.ToShortDateString();
                    txtLisansing1.Text = "Kalan Gün : \r\n" + validate.DaysLeft;
                    lisansKalan = validate.DaysLeft;
                    if (validate.DaysLeft > 0)
                    {
                        pnlLisans.Visible = false;
                        this.Size = new Size(718, 325);
                    }
                    else
                    {
                        CustomMessageBox.ShowMessage("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız", "", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (lisansKalan <= 0)
                {
                    var ComputerLisansingID = key2.GetValue("ComputerLisansingID").ToString();
                    var ApplicationSecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();
                    var newkey = await client.Newkey(VKN,ProductName, ComputerLisansingID, ComputerInfo.GetComputerId());
                    if (ApplicationSecretPhase != newkey)
                    {
                        key2.SetValue("ApplicationSecretPhase", newkey);
                        Properties.Settings.Default.EntegrefSecretPhase = newkey;
                        Properties.Settings.Default.Save();
                        key2.Close();
                        Application.Restart();
                        CustomMessageBox.ShowMessage("Entegref Lisans Anahtarınız Güncellendi...!", "Kullanım süreniz dolan lisans anahtarı otomatik güncellendi.", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        CustomMessageBox.ShowMessage("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız", "Kullanım süreniz dolmuştur. Programnı Kullanmaya devam etmek için lütfen Yeni Lisans Anahtarı Satın Alın", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //XtraMessageBox.Show("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız");
                        Properties.Settings.Default.EntegrefSecretPhase = "";
                        Properties.Settings.Default.Save();
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowMessage("Program Hatası", ex.Message, this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        DateTime now = DateTime.Now;
        string bugun = "";
        bool yenigun = false;
        private async Task Lisansing(string vknid)
        {
            try
            {
                if (string.IsNullOrEmpty((Properties.Settings.Default.EntegrefAIPToken)))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, "http://lisans.entegref.com/token");
                        //var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44371/token");
                        request.Content = new StringContent("grant_type=password&username=admin&password=Madam3169");

                        // İstek başlığına gerekli kimlik doğrulama bilgilerini ekleyin
                        //string clientId = "your_client_id";
                        //string clientSecret = "your_client_secret";
                        //string credentials = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(clientId + ":" + clientSecret));
                        //request.Headers.Add("Authorization", "Basic " + credentials);

                        // İsteği gönderin ve yanıtı alın
                        HttpResponseMessage responses = await client.SendAsync(request);

                        // Yanıtın başarılı olup olmadığını kontrol edin
                        if (responses.IsSuccessStatusCode)
                        {
                            string responseData = await responses.Content.ReadAsStringAsync();
                            Token myDeserializedClass = JsonConvert.DeserializeObject<Token>(responseData);
                            Properties.Settings.Default.EntegrefAIPToken = myDeserializedClass.access_token;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            CustomMessageBox.ShowMessage("API isteği başarısız",responses.RequestMessage.ToString(),this, "Servis Uyarısı",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }
                    }
                }
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.EntegrefAIPToken);
                HttpResponseMessage response = await httpClient.GetAsync($"api/data/Lisans?VKN={vknid}&AppName={ProductName}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                    List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(responseData);
                    foreach (var item in myDeserializedClass)
                    {
                        if (item.status)
                        {
                            SKGL.Validate validate = new SKGL.Validate();
                            validate.secretPhase = VKN;
                            validate.Key = item.message;
                            Properties.Settings.Default.EntegrefSecretPhase = item.message;
                            Properties.Settings.Default.Save();
                            var assembly = typeof(Program).Assembly;
                            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                            var id = attribute.Value;
                            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                            key.SetValue("ApplicationSetupComplate", "true");
                            key.SetValue("ApplicationSecretPhase", item.message);// Properties.Settings.Default.EntegrefSecretPhase);
                            key.Close();
                        }
                        else
                        {
                            CustomMessageBox.ShowMessage("",item.message,this, "Dikkat", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    CustomMessageBox.ShowMessage("API isteği başarısız: ", response.Content.ToString(), this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowMessage("",ex.Message,this,"",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
                DialogResult result = CustomMessageBox.ShowMessage("",exp.Message + "\r\nHarici Baglantı Kullanılsın mı",this, "Seçim", MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
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
                        CustomMessageBox.ShowMessage("DiDTelekom Token alınırken Hata oldu",ex.Message,this,"",MessageBoxButtons.OK,MessageBoxIcon.Information);

                    }
                }
            }
            catch (Exception ex2)
            {
                CustomMessageBox.ShowMessage("",ex2.Message,this,"",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
            txtVolantUser_TextChanged(null, null);
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
            var yetki = Sorgu(string.Format(@"select * from SOCIAL left outer join CASHIER on CHSOCODE = SOCODE where  SOSTS = 1 and SOCODE = '{0}' and SOENTERKEY = '{1}'", txtVolantUser.Text, txtVolantPassword.Text), Settings.Default.connectionstring);

            if (yetki.Rows.Count > 0)
            {
                //DidToken();
                SOCODE = yetki.Rows[0]["SOCODE"].ToString();
                if (cmbVolantMagaza.EditValue.ToString() == "00")
                {
                    DIVVAL = yetki.Rows[0]["CHDIVISON"].ToString();
                }
                else
                {
                    DIVVAL = cmbVolantMagaza.EditValue.ToString();
                }
                this.Hide();
                if (Properties.Settings.Default.Company == "VDB_KAMALAR01")
                {
                    frmMain2 main2 = new frmMain2();
                    main2.ShowDialog();
                }
                else
                {
                    frmMain main = new frmMain();
                    main.ShowDialog();
                }
                this.Show();
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
            var sonuc = Sorgu("select SONAME +SPACE(1)+SOSURNAME as Name, CHDIVISON,SODEPART from SOCIAL left outer join CASHIER on CHSOCODE = SOCODE where SOSTS = 1 and SOCODE = '" + txtVolantUser.Text + "'", Settings.Default.connectionstring);
            if (sonuc.Rows.Count > 0)
            {
                if (sonuc.Rows[0]["SODEPART"].ToString() != "ADMIN")
                {
                    cmbVolantMagaza.EditValue = sonuc.Rows[0]["CHDIVISON"].ToString();
                    cmbVolantMagaza.Enabled = false;
                }
                else
                {
                    cmbVolantMagaza.Enabled = true;
                }
                togsKullanici.IsOn = true;
                togsKullanici.Properties.OnText = sonuc.Rows[0]["Name"].ToString();
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