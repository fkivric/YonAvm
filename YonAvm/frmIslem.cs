using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace YonAvm
{
    public partial class frmIslem : DevExpress.XtraEditors.XtraForm
    {
        string CURID;
        string SALID;
        public frmIslem(string _curid, string _salid)
        {
            InitializeComponent();
            CURID = _curid;
            SALID = _salid;
        }

        private void frmIslem_Load(object sender, EventArgs e)
        {

        }
    }
}