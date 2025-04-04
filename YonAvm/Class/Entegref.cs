using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YonAvm.Class
{
    class Entegref
    {
        private readonly HttpClient httpClient;
        public class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }
        public class EntegrefAPI
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string results { get; set; }
            public string message { get; set; }
            public string internalMessage { get; set; }
            public List<Validation> validations { get; set; }
        }
        public class Validation
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }
        public class Sonuc
        {
            public bool status { get; set; }
            public string message { get; set; }
        }
        public class ReturnModel
        {
            public bool status { get; set; }
            public string message { get; set; }
        }
        public class ConnectionList
        {
            public string ServerName { get; set; }
            public string Conncetion { get; set; }
        }
        public class Lisans
        {
            public DateTime CreationDate { get; }
            public int DaysLeft { get; }
            public DateTime ExpireDate { get; }
            public bool IsValid { get; }
        }
        public static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            Dictionary<string, string> connectionProperties = new Dictionary<string, string>();

            string[] properties = connectionString.Split(';');
            foreach (string property in properties)
            {
                string[] keyValue = property.Split('=');
                if (keyValue.Length == 2)
                {
                    connectionProperties[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            return connectionProperties;
        }
        private static readonly HttpClient client = new HttpClient();
        public async Task<string> SetVersion(string VKN, string Name, string version, string appname)
        {
            string baseUrl = "http://lisans.entegref.com/"; // API URL'nizi buraya ekleyin
            //string baseUrl = "https://localhost:44371/"; // API URL'nizi buraya ekleyin

            string requestUrl = $"{baseUrl}/api/data/Versiyoninsert?VKN={VKN}&Name={Name}&version={version}&AppName={appname}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                Console.WriteLine($"Hata kodu: {response.StatusCode}");
                return null;
            }
        }
        public async Task<string> Versiyon(string VKN, string Name, string appname)
        {
            string baseUrl = "http://lisans.entegref.com/"; // API URL'nizi buraya ekleyin
            //string baseUrl = "http://localhost:24853/"; // API URL'nizi buraya ekleyin

            string requestUrl = $"{baseUrl}/api/data/VersiyonCheck?VKN={VKN}&Name={Name}&AppName={appname}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                Console.WriteLine($"Hata kodu: {response.StatusCode}");
                return null;
            }
        }

        public async Task<string> UpdateLicensingUser(string VKN, string Name, string ID, string version, string appname)
        {
            string baseUrl = "http://lisans.entegref.com/"; // API URL'nizi buraya ekleyin
            //string baseUrl = "https://localhost:44371/"; // API URL'nizi buraya ekleyin

            string requestUrl = $"{baseUrl}/api/data/UpdateLisansingUser?VKN={VKN}&Name={Name}&ID={ID}&version={version}&AppName={appname}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                Console.WriteLine($"Hata kodu: {response.StatusCode}");
                return null;
            }
        }

        public async Task<string> UpdateLicensing(string VKN, string lisansID, string cpuid, string mdid, string appname)
        {
            string baseUrl = "http://lisans.entegref.com/"; // API URL'nizi buraya ekleyin
            //string baseUrl = "http://localhost:24853/"; // API URL'nizi buraya ekleyin

            string requestUrl = $"{baseUrl}/api/data/UpdateLisansing?VKN={VKN}&lisansID={lisansID}&cpuid={cpuid}&mdid={mdid}&AppName={appname}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                Console.WriteLine($"Hata kodu: {response.StatusCode}");
                return null;
            }
        }

        public async Task<string> Newkey(string VKN, string appname, string lisansID, string computerid)
        {
            string Key = "";
            string baseUrl = "http://lisans.entegref.com/"; // API URL'nizi buraya ekleyin
            //string baseUrl = "http://localhost:24853/"; // API URL'nizi buraya ekleyin

            string requestUrl = $"{baseUrl}/api/data/Programskey?VKN={VKN}&APPName={appname}&lisansingID={lisansID}&computerid={computerid}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(result);
                foreach (var item in myDeserializedClass)
                {
                    Key = item.message;
                }
                return Key;
            }
            else
            {
                Console.WriteLine($"Hata kodu: {response.StatusCode}");
                return null;
            }
        }
    }
}
