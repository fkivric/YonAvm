using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using FoxLearn.License;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YonAvm.Class;

namespace YonAvm
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
            var sonuc = key2.GetValue("ApplicationGUID");
            if (sonuc == null)
            {
                RunAsync().Wait();
            }
            bool acikmi = false;
            Mutex mtex = new Mutex(true, "Program", out acikmi);
            if (acikmi)
            {
                string _s4 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                key.SetValue("ApplicationVersion", _s4);
                key.Close();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("TR-tr");
                Application.Run(new frmLogin());
            }
            else
            {
                MessageBox.Show("Program Çalışıyor", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static async Task RunAsync()
        {
            // Asenkron işlemleri burada gerçekleştirin
            //await Task.Delay(1000); // Örnek bir asenkron işlem

            string Cpuid = "";
            string Motherboardid = "";
            //string fileName = "Abto\\Register_SIP_SDK_ActiveX.bat"; // .bat dosyanızın adı

            //// Proje dizini içindeki .bat dosyanızın tam yolunu oluşturun
            //string pathToBatFile = System.IO.Path.Combine(Application.StartupPath, fileName);


            //Process process = new Process();
            //process.StartInfo.FileName = "cmd.exe"; // Komutları çalıştırmak için komut istemcisini (cmd.exe) kullanıyoruz.
            //process.StartInfo.Arguments = "/c " + pathToBatFile; // /c parametresi, komut istemcisini kapatır (cmd.exe'yi çalıştırdıktan sonra kapatır).
            //process.Start();

            string _s4 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); // versiyon
            string _s5 = System.Reflection.Assembly.GetExecutingAssembly().GetName().CultureInfo.ToString(); // kültür bilgisi
            string _s6 = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı
            string _s7 = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company; // şirket
            string _s8 = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; // Copyright
            ManagementObjectSearcher query = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            ManagementObjectSearcher CPU = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectSearcher MOTHERBOARD = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (ManagementObject obj in CPU.Get())
            {
                Cpuid = obj["ProcessorID"].ToString();
            }
            foreach (ManagementObject obj in MOTHERBOARD.Get())
            {
                Motherboardid = obj["SerialNumber"].ToString();
            }
            ManagementObjectCollection queryCollection = query.Get();
            string pcModeli = "";
            string pcİsmi = "";
            foreach (var item in queryCollection)
            {
                pcModeli = item["model"].ToString();
                pcİsmi = item["name"].ToString();
            }
            var assembly = typeof(Program).Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var id = attribute.Value;
            //Entegref client = new Entegref();
            //string response = await client.UpdateLicensingUser("6200080458", pcİsmi.ToString(), pcModeli.ToString(), _s4, _s6);
            //List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
            //var ConnectionLisansingID = myDeserializedClass[0].message;
            //string response2 = await client.UpdateLicensing("6200080458", ConnectionLisansingID.ToString(), Cpuid.ToString(), Motherboardid.ToString(), _s6, ComputerInfo.GetComputerId());
            //if (response2 != null)
            //{
            //    List<Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Sonuc>>(response2);
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegreFYonSatışDestek");
                key.SetValue("ApplicationGUID", id);
                key.SetValue("ApplicationVersion", _s4);
                key.SetValue("ApplicationVKN", "6200080458");
                key.SetValue("ApplicationPhoneLisans", true);
                key.SetValue("CPU", Cpuid);
                key.SetValue("motherboardid", Motherboardid);
                key.SetValue("ComputerName", pcİsmi.ToString());
                key.SetValue("ComputerID", pcModeli);
                key.Close();
            //}
            //else
            //{
            //    XtraMessageBox.Show("Lisan Hatası");
            //}

        }
    }
}
