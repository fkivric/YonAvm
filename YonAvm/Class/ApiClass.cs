using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonAvm.Class
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class Sonuc
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class Lisanslar
    {
        public int id { get; set; }

        public int CallNumber { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public decimal? CallerId { get; set; }
    }
    public class DidKey
    {
        public string username { get; set; }
        public string password { get; set; }
        public string centralcode { get; set; }
        public string module { get; set; }
        public string function { get; set; }
    }
    // DidKeyreturn myDeserializedClass = JsonConvert.DeserializeObject<DidKeyreturn>(myJsonResponse);
    public class DidKeyreturn
    {
        public string result { get; set; }
        public string description { get; set; }
        public string apikey { get; set; }
        public string centraluuid { get; set; }
        public string centralcode { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DidEXTLIST
    {
        public string apikey { get; set; }
        public string centraluuid { get; set; }
        public string centralcode { get; set; }
        public string module { get; set; }
        public string function { get; set; }
    }
    // DidEXTLISTreturn myDeserializedClass = JsonConvert.DeserializeObject<DidEXTLISTreturn>(myJsonResponse);
    public class Datum
    {
        public string santraldahili_kullaniciadi { get; set; }
        public string santraldahili_ekranadi { get; set; }
        public string santraldahili_sifre { get; set; }
        public string santraldahili_numara90e164 { get; set; }
        public string santraldahili_domain { get; set; }
        public string santraldahili_durum { get; set; }
    }

    public class DidEXTLISTreturn
    {
        public string result { get; set; }
        public string description { get; set; }
        public List<Datum> data { get; set; }
    }
}
