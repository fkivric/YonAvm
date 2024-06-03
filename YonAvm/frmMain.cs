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
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        public frmMain()
        {
            InitializeComponent();
        }
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnectionObject conn = new SqlConnectionObject();
        private void tileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame.SelectedPageIndex = tileBarGroupTables.Items.IndexOf(e.Item);
            lblSayfaIsmi.Text = e.Item.Name;
        }
        private void tileBar1_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = tileBarGroup1.Items.IndexOf(e.Item);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            srcMagaza.Properties.DataSource = conn.GetDataTableConnectionSql("select DIVVAL, DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql);
            srcMagaza.Properties.ValueMember = "DIVVAL";
            srcMagaza.Properties.DisplayMember = "DIVNAME";
            dteBasTarih.EditValue = DateTime.Now.AddDays(-1);
            dteBitTarih.EditValue = DateTime.Now;
        }

        private void btnSatisListele_Click(object sender, EventArgs e)
        {
            string div;
            if (srcMagaza.EditValue != null)
            {
                div = "= '"+srcMagaza.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            string q = String.Format(@"SELECT distinct SALID,
            CURID ,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
            SALDATE,
            DEEDDATE,
            CUSIDNAME,
            CUSIDSIRNAME,
            CURVAL,
            isnull(CUSIDTCNO,(select CURCHWATNO from CURRENTSCHILD where CURCHID = SALCURID)) as CUSIDENTY,
            (select CURCHWATP from CURRENTSCHILD where CURCHID = SALCURID) as CUSIDWP,
            CUSIDBIRTHDAY,
            case when CUSIDSEX = 'E' then 'ERKEK' 
	            when CUSIDSEX = 'K' then 'KADIN'
            else 'BELİRSİZ' end CUSSEX,
            case when CUSIDMARRIED = 'B' then 'BEKAR' 
	            when CUSIDMARRIED = 'E' then 'EVLI'
            else 'BELİRSİZ' end CUSMERRIED,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSACITY FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS DEFRENT ,

            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR 
            FROM DEEDS WITH (NOLOCK)  
            LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
            LEFT OUTER JOIN ORDERSCHILD WITH (NOLOCK) ON  ORDCHID=PROBHORDCHID  
            LEFT OUTER JOIN ORDERS WITH (NOLOCK) ON ORDID=ORDCHORDID 
            LEFT OUTER JOIN SALES WITH (NOLOCK) ON SALID=ORDSALID 
            LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
            LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
            LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
            LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
            LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
            LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
            LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
            LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
            WHERE DEEDDPBHEVAL IN (600,-600)
            AND DEEDDEEDVAL IN (3,4,1)
            AND SALID IS NOT NULL 
            AND DEEDDATE between '{0}' and '{1}'
            AND PROSUPPLIER.PROSUPCURID IN (3408506)
            AND SALDIVISON {2}
            union
            SELECT distinct SALID,
            CURID ,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
            SALDATE,
            DEEDDATE,
            CUSIDNAME,
            CUSIDSIRNAME,
            CURVAL,
            isnull(CUSIDTCNO,(select CURCHWATNO from CURRENTSCHILD where CURCHID = SALCURID)) as CUSIDENTY,
            (select CURCHWATP from CURRENTSCHILD where CURCHID = SALCURID) as CUSIDWP,
            CUSIDBIRTHDAY,
            case when CUSIDSEX = 'E' then 'ERKEK' 
	            when CUSIDSEX = 'K' then 'KADIN'
            else 'BELİRSİZ' end CUSSEX,
            case when CUSIDMARRIED = 'B' then 'BEKAR' 
	            when CUSIDMARRIED = 'E' then 'EVLI'
            else 'BELİRSİZ' end CUSMERRIED,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID) >= 0 THEN
            (SELECT CSACITY FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID))
            ELSE
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS DEFRENT ,

            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = INVCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR 
            FROM DEEDS WITH (NOLOCK)  
            LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
            LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID 
            LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
            LEFT OUTER JOIN INVOICE WITH (NOLOCK) ON INVID=INVCHINVID 
            LEFT OUTER JOIN SALES WITH (NOLOCK) ON SALID=INVSALID 
            LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
            LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
            LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
            LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
            LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
            LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
            LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
            LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
            WHERE DEEDDPBHEVAL IN (600,-600)
            AND DEEDDEEDVAL IN (3,4,1)
            AND SALID IS NOT NULL 
            AND DEEDDATE  between '{0}' and '{1}'
            AND SALDIVISON {2}
            AND PROSUPPLIER.PROSUPCURID IN (3408506)", Convert.ToDateTime(dteBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteBitTarih.EditValue).ToString("yyyy-MM-dd"),div);
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridVestelSatis.DataSource = dt;
                ViewVestelSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewVestelSatis.BestFitColumns(true);
                srcMagaza.Enabled = false;
                dteBasTarih.Enabled = false;
                dteBitTarih.Enabled = false;
                btnSatisListele.Enabled = false;
                btnIadeListele.Enabled = false;
            }
            else
            {
                srcMagaza.Enabled = true;
                dteBasTarih.Enabled = true;
                dteBitTarih.Enabled = true;
                btnSatisListele.Enabled = true;
                btnIadeListele.Enabled = true;

            }

        }

        private void btnIadeListele_Click(object sender, EventArgs e)
        {
            //denemerrrrrrr
        }

        private void Yeni_Click(object sender, EventArgs e)
        {
            navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            gridUrunler.DataSource = null;
            gridVestelSatis.DataSource = null;
            txtMadi.Text = "";
            txtSAdi.Text = "";
            txtCode.Text = "";
            txtIdnty.Text = "";
            txtVdate.Text = "";
            txtsex.Text = "";
            txtMerred.Text = "";
            txtXity.Text = "";
            txtDefrent.Text = "";
            txtgsm1.Text = "";
            txtgsm2.Text = "";
            txtmail.Text = "";
            txtadr.Text = "";
            srcMagaza.Enabled = true;
            dteBasTarih.Enabled = true;
            dteBitTarih.Enabled = true;
            btnSatisListele.Enabled = true;
            btnIadeListele.Enabled = true;
        }

        private void ViewVestelSatis_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks >= 2 && e.Button == MouseButtons.Left)
            {
                navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                var selectdrow = ViewVestelSatis.GetSelectedRows();

                txtMadi.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDNAME").ToString();
                txtSAdi.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDNAME").ToString();
                txtCode.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CURVAL").ToString();
                txtIdnty.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDENTY").ToString();
                txtVdate.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDBIRTHDAY").ToString();
                txtsex.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSSEX").ToString();
                txtMerred.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSMERRIED").ToString();
                txtXity.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CITY").ToString();
                txtDefrent.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "DEFRENT").ToString();
                txtgsm1.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSGSM1").ToString();
                txtgsm2.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSGSM2").ToString();
                txtmail.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSMAIL").ToString();
                txtadr.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSADR").ToString();
                
                var SALID = ViewVestelSatis.GetRowCellValue(selectdrow[0], "SALID").ToString();
                string q = String.Format(@"select PROVAL,PROBARID,
                PRONAME,
                case when INVCHID is not NULL then PROBHQUAN else ORDCHQUAN end as QUAN
                from SALES 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) on INVSALID = SALID
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID -- ON PROBHDEEDID=DEEDID 
                left outer join ORDERS on ORDSALID = SALID
                left outer join ORDERSCHILD on ORDCHORDID = ORDID
                left outer join PRODUCTS on (PROID = PROBHPROID or PROID = ORDCHPROID)
                left outer join PRODUCTSBAR on PROBARPROID = PROID
                where SALID = {0}", SALID);
                gridUrunler.DataSource = conn.GetDataTableConnectionSql(q, sql);

            }
        }
    }
}
 