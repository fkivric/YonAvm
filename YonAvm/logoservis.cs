using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YonAvm
{
    public class LogoUser
    {
        public string appStr { get; set; }
        public string passWord { get; set; }
        public int source { get; set; }
        public string userName { get; set; }
        public string version { get; set; }
    }
    public class ParamList
    {
        public string DOCUMENTTYPE { get; set; }
        public string DATAFORMAT { get; set; }
        public string ISCANCEL { get; set; }
    }
    public static class logoservis
    {
        public static Logo.PostBoxServiceClient serviceClient; 
    }
}
