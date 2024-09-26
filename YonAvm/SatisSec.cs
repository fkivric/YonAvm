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
using System.Data.SqlClient;

namespace YonAvm
{
    public partial class SatisSec : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        public string MUSTERIADI { get; set; }
        public string SATISTARİHİ { get; set; }
        public string SATISTUTARI { get; set; }
        public string CURID{ get; set; }
        public string SALID { get; set; }
        string CURVAL;
        public SatisSec(string _Val)
        {
            InitializeComponent();
            CURVAL = _Val;
        }

        private void SatisSec_Load(object sender, EventArgs e)
        {
            labelControl1.Text = MUSTERIADI;
            labelControl2.Text = SATISTARİHİ;
            labelControl3.Text = SATISTUTARI;
            SORGU(CURVAL);
        }
        void SORGU(string VAL)
        {
            string q = String.Format(@"select distinct d.CURID,CURVAL,CURNAME,d.SALID,Convert(char(10),IslemTarihi,121) as FTRDATE,SALAMOUNT from MDE_GENEL.dbo.DivaEklenenler d
            left outer join VDB_YON01.dbo.SALES vs on vs.SALID = d.SALID
            left outer join VDB_YON01.dbo.CURRENTS vc on vc.CURID = d.CURID
            left outer join VDB_YON01.dbo.SALESINVOICE i on i.SALINVSALID = d.SALID
            where not exists (select * from MDE_GENEL.dbo.DivaFaturaListesi where VOL_CURID = d.CURID and VOL_SALID = d.SALID)
            --and not exists (select * from VDB_YON01.dbo.SALESINVOICE where SALINVSALID = d.SALID)
            and SALAMOUNT is not NULL
            and CURVAL = '{0}'", VAL);
            gridListe.DataSource = conn.GetDataTableConnectionSql(q, sql);
            ViewListe.OptionsView.BestFitMaxRowCount = -1;
            ViewListe.BestFitColumns(true);
        }

        private void btnSec_ItemClick(object sender, TileItemEventArgs e)
        {
            var selected = ViewListe.GetSelectedRows();
            if (selected.Length > -1)
            {
                SALID = ViewListe.GetRowCellValue(selected[0], "SALID").ToString();
                CURID = ViewListe.GetRowCellValue(selected[0], "CURID").ToString();
                this.Close();
                this.Dispose();
            }
        }
    }
}