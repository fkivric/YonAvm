using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YonAvm.Class
{
    public static class Volant
    {
        public class AllDataBase
        {
            public string DbNAme { get; set; }
        }
        public class Firma
        {
            public string COMPANYDB { get; set; }
            public string COMPANYNAME { get; set; }

        }

        public class Magazalar
        {
            public string DIVVAL {get;set;}
            public string DIVNAME { get;set;}

        }
        private static string password = "310894";
        public class eDatabase
        {
            public string sqlServerName { get; set; }

            public string databaseName { get; set; }

            public string login { get; set; }

            public string password { get; set; }

            public string integratedSecurity { get; set; }

            public string company { get; set; }

            public string divison { get; set; }

            public string year { get; set; }

            public string pathOfPrints { get; set; }

            public string pathOfArchive { get; set; }

            public string serverPort { get; set; }

            public string serverDbName => sqlServerName + databaseName;

            public string language { get; set; }
        }
        public static string TextSifreCoz(this string text)
        {
            byte[] SifrelenmisByteDizisi = Convert.FromBase64String(text);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[13]
            {
            73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
            100, 101, 118
            });
            byte[] SifresiCozulmusVeri = SifreCoz(SifrelenmisByteDizisi, pdb.GetBytes(32), pdb.GetBytes(16));
            return Encoding.Unicode.GetString(SifresiCozulmusVeri);
        }
        private static byte[] SifreCoz(byte[] SifreliVeri, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(SifreliVeri, 0, SifreliVeri.Length);
            cs.Close();
            return ms.ToArray();
        }
        public static Image byteArrayToImage(this byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            return Image.FromStream(ms);
        }
    }
}
