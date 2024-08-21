using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YonAvm
{
    public partial class logodeneme : Form
    {
        public logodeneme()
        {
            InitializeComponent();
        }

        private void logodeneme_Load(object sender, EventArgs e)
        {
            Login();
        }
        [ServiceContract]
        public interface IPostBoxService
        {
            [OperationContract]
            bool Login(Logo.LoginType login, out string sessionID);
        }
        public void Login()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            BasicHttpBinding binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 655360, // 640 KB
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxDepth = 32,
                    MaxStringContentLength = 8192,
                    MaxArrayLength = 16384,
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 16384
                }
            };
            try
            {
                Logo.PostBoxServiceClient client = new Logo.PostBoxServiceClient("PostBoxServiceEndpoint");
                Logo.LoginType user = new Logo.LoginType
                {
                    userName = "6200080458",
                    passWord = "MagicUser2024!",
                };
                string sesionid = "";
                bool ss = client.Login(user, out sesionid); //logoservis.serviceClient.Login(user, out sesionid);
                if (ss)
                {
                    Logo.GetDocumentType document = Logo.GetDocumentType.EARCHIVE;
                    Logo.DocumentDataType dataType = Logo.DocumentDataType.UBL;
                    string UUID = "69ead0d2-3ede-4b49-bd0b-fcfdc5db3fee".ToUpper();
                    var sonuc = client.getDocumentData(sesionid,UUID, document,dataType);

                    byte[] zipData = sonuc.binaryData.Value;

                    //var base64Data = sonuc.binaryData.Value;

                    //byte[] zipData = Convert.FromBase64String(base64Data);

                    // ZIP dosyasının adını belirleyin
                    string fileName = sonuc.fileName;

                    // ZIP dosyasını kaydedin
                    File.WriteAllBytes(fileName, zipData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
