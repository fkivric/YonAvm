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
using System.Text.RegularExpressions;
using System.Diagnostics;
using DevExpress.XtraPrinting;
using System.IO;
using YonAvm.EFaturaIntegrationService;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Spreadsheet;
using Worksheet = DevExpress.Spreadsheet.Worksheet;
using System.Threading;
using YonAvm.Class;
using static YonAvm.Class.FaturaVeri;
using System.Net;
using System.ServiceModel;
using System.IO.Compression;

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
        private BackgroundWorker _backgroundWorker;
        private ManualResetEvent _workerCompletedEvent = new ManualResetEvent(false);
        private void executeBackground(Action doWorkAction, Action progressAction = null, Action completedAction = null)
        {
            try
            {
                if (_backgroundWorker != null)
                {
                    if (_backgroundWorker.IsBusy)
                    {
                        return;
                    }
                }
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += (x, y) =>
                {
                    try
                    {
                        doWorkAction.Invoke();
                    }
                    catch (Exception ex)
                    {
                        y.Cancel = true;
                        XtraMessageBox.Show("Bilinmeyen Hata. Detay : " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // throw;
                    }
                };
                if (progressAction != null)
                {
                    _backgroundWorker.ProgressChanged += (x, y) =>
                    {
                        progressAction.Invoke();
                    };
                }
                if (completedAction != null)
                {
                    _backgroundWorker.RunWorkerCompleted += (x, y) =>
                    {
                        completedAction.Invoke();
                    };
                }
                this.Enabled = false;
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception)
            {

            }

        }
        private void completeProgress()
        {
            try
            {
                _backgroundWorker.Dispose();
                _backgroundWorker = null;
                if (!this.Enabled)
                {
                    this.Enabled = true;
                }

            }
            finally
            {
                //this.Cursor = Cursors.Default;
                _workerCompletedEvent.Set();

            }
        }
        private void tileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            if (tileBarGroupTables.Items.IndexOf(e.Item) >= 0)
            {
                navigationFrame.SelectedPageIndex = tileBarGroupTables.Items.IndexOf(e.Item);
                lblSayfaIsmi.Text = e.Item.Name;
            }
        }
        private void tileBar1_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = tileBarGroup1.Items.IndexOf(e.Item);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            srcVestelMagaza.Properties.DataSource = conn.GetDataTableConnectionSql("select DIVVAL, DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql);
            srcVestelMagaza.Properties.ValueMember = "DIVVAL";
            srcVestelMagaza.Properties.DisplayMember = "DIVNAME";
            dteVestelBasTarih.EditValue = DateTime.Now;
            dteVestelBitTarih.EditValue = DateTime.Now;
            dteVestelBasTarih.Properties.MaxValue = DateTime.Now;
            dteVestelBitTarih.Properties.MaxValue = DateTime.Now;

            srcVestelMagaza2.Properties.DataSource = conn.GetDataTableConnectionSql("select DIVVAL, DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql);
            srcVestelMagaza2.Properties.ValueMember = "DIVVAL";
            srcVestelMagaza2.Properties.DisplayMember = "DIVNAME";
            dteVestelBasTarih2.EditValue = DateTime.Now;
            dteVestelBitTarih2.EditValue = DateTime.Now;
            dteVestelBasTarih2.Properties.MaxValue = DateTime.Now;
            dteVestelBitTarih2.Properties.MaxValue = DateTime.Now;

            srcProfiloMagaza.Properties.DataSource = conn.GetDataTableConnectionSql("select DIVVAL, DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql);
            srcProfiloMagaza.Properties.ValueMember = "DIVVAL";
            srcProfiloMagaza.Properties.DisplayMember = "DIVNAME";
            dteProfiloBasTarih.EditValue = DateTime.Now;
            dteProfiloBitTarih.EditValue = DateTime.Now;
            dteProfiloBasTarih.Properties.MaxValue = DateTime.Now;
            dteProfiloBitTarih.Properties.MaxValue = DateTime.Now;

            srcProfiloMagaza2.Properties.DataSource = conn.GetDataTableConnectionSql("select DIVVAL, DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql);
            srcProfiloMagaza2.Properties.ValueMember = "DIVVAL";
            srcProfiloMagaza2.Properties.DisplayMember = "DIVNAME";
            dteProfiloBasTarih2.EditValue = DateTime.Now;
            dteProfiloBitTarih2.EditValue = DateTime.Now;
            dteProfiloBasTarih2.Properties.MaxValue = DateTime.Now;
            dteProfiloBitTarih2.Properties.MaxValue = DateTime.Now;

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                this.Text = "Version : " + ad.CurrentVersion.Major + "." + ad.CurrentVersion.Minor + "." + ad.CurrentVersion.Build + "." + ad.CurrentVersion.Revision;
                //version = ad.CurrentVersion.Revision.ToString();
            }
            else
            {
                string _s1 = Application.ProductVersion; // versiyon
                this.Text = "Version : " + _s1;
            }
        }
        EFaturaIntegration Clint = new EFaturaIntegration();
        void ParkToken()
        {
            var token = Clint.OturumAc("yonavm", "123456");
            if (token.IsSuccessLogin)
            {
                Properties.Settings.Default.ParkToken = token.SessionId;
            }
        }
        private void btnSatisListele_Click(object sender, EventArgs e)
        {
            string q;
            string div;
            if (srcVestelMagaza.EditValue != null)
            {
                div = "= '" + srcVestelMagaza.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            if (Properties.Settings.Default.Company == "VDB_KAMALAR01")
            {
                q = String.Format(@"SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				s.SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN ORDERSCHILD WITH (NOLOCK) ON  ORDCHID=PROBHORDCHID  
                LEFT OUTER JOIN ORDERS WITH (NOLOCK) ON ORDID=ORDCHORDID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=ORDSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600,-600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE between '{0}' and '{1}'
                AND PRONAME like 'VESTEL%'
                AND SALDIVISON {2}
                AND s.SALID > 0
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
                union
                SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				s.SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID 
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=INVSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600,-600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
                AND s.SALID > 0
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)                
                AND PRONAME like 'VESTEL%'", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            else
            {
                q = String.Format(@"SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				s.SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN ORDERSCHILD WITH (NOLOCK) ON  ORDCHID=PROBHORDCHID  
                LEFT OUTER JOIN ORDERS WITH (NOLOCK) ON ORDID=ORDCHORDID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=ORDSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600,-600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE between '{0}' and '{1}'
                AND PROSUPPLIER.PROSUPCURID IN (3408506)
                AND SALDIVISON {2}
                AND s.SALID > 0
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
                union
                SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				s.SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID 
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=INVSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600,-600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
                AND s.SALID > 0
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)
                AND PROSUPPLIER.PROSUPCURID IN (3408506)", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridVestelSatis.DataSource = dt;
                ViewVestelSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewVestelSatis.BestFitColumns(true);
                srcVestelMagaza.Enabled = false;
                dteVestelBasTarih.Enabled = false;
                dteVestelBitTarih.Enabled = false;
                btnVestelSatisListele.Enabled = false;
                btnVestelIadeListele.Enabled = false;
            }
            else
            {
                srcVestelMagaza.Enabled = true;
                dteVestelBasTarih.Enabled = true;
                dteVestelBitTarih.Enabled = true;
                btnVestelSatisListele.Enabled = true;
                btnVestelIadeListele.Enabled = true;

            }
            satis = true;
        }

        private void btnIadeListele_Click(object sender, EventArgs e)
        {
            string q;
            string div;
            if (srcVestelMagaza.EditValue != null)
            {
                div = "= '" + srcVestelMagaza.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            if (Properties.Settings.Default.Company == "VDB_KAMALAR01")
            {
                q = String.Format(@"SELECT distinct SALID,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
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
                AND SALID < 0
                AND DEEDDATE between '{0}' and '{1}'
                AND PRONAME like 'VESTEL%'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
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
                AND SALID < 0
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)
                AND PRONAME like 'VESTEL%'", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            else
            {
                q = String.Format(@"SELECT distinct SALID,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
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
                AND SALID < 0
                AND DEEDDATE between '{0}' and '{1}'
                AND PROSUPPLIER.PROSUPCURID IN (3408506)
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
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
                AND SALID < 0
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)
                AND PROSUPPLIER.PROSUPCURID IN (3408506)", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridVestelSatis.DataSource = dt;
                ViewVestelSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewVestelSatis.BestFitColumns(true);
                srcVestelMagaza.Enabled = false;
                dteVestelBasTarih.Enabled = false;
                dteVestelBitTarih.Enabled = false;
                btnVestelSatisListele.Enabled = false;
                btnVestelIadeListele.Enabled = false;
            }
            else
            {
                srcVestelMagaza.Enabled = true;
                dteVestelBasTarih.Enabled = true;
                dteVestelBitTarih.Enabled = true;
                btnVestelSatisListele.Enabled = true;
                btnVestelIadeListele.Enabled = true;

            }
            satis = false;
        }

        private void Yeni_Click(object sender, EventArgs e)
        {
            navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            gridVestelUrunler.DataSource = null;
            gridVestelSatis.DataSource = null;
            txtMadi_V.Text = "";
            txtSAdi_V.Text = "";
            txtCode_V.Text = "";
            txtIdnty_V.Text = "";
            txtVdate_V.Text = "";
            txtSex_V.Text = "";
            txtMerred_V.Text = "";
            txtCity_V.Text = "";
            txtDefrent_V.Text = "";
            txtGsm1_V.Text = "";
            txtGsm2_V.Text = "";
            txtMail_V.Text = "";
            txtAdr_V.Text = "";
            srcVestelMagaza.Enabled = true;
            dteVestelBasTarih.Enabled = true;
            dteVestelBitTarih.Enabled = true;
            btnVestelSatisListele.Enabled = true;
            btnVestelIadeListele.Enabled = true;
        }

        private void ViewVestelSatis_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks >= 2 && e.Button == MouseButtons.Left && e.RowHandle >= 0)
            {
                navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                var selectdrow = ViewVestelSatis.GetSelectedRows();

                txtMadi_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDNAME").ToString();
                txtSAdi_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDSIRNAME").ToString();
                txtCode_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CURVAL").ToString();
                txtIdnty_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDENTY").ToString();
                txtVdate_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSIDBIRTHDAY").ToString();
                txtSex_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSSEX").ToString();
                txtMerred_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSMERRIED").ToString();
                txtCity_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CITY").ToString();
                txtDefrent_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "DEFRENT").ToString();
                txtGsm1_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSGSM1").ToString();
                txtGsm2_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSGSM2").ToString();
                txtMail_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSMAIL").ToString();
                txtAdr_V.Text = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CUSADR").ToString();

                var SALID = ViewVestelSatis.GetRowCellValue(selectdrow[0], "SALID").ToString();
                string q = String.Format(@"select PROVAL,
                PRONAME,
                case when INVCHID is not NULL then PROBHQUAN else ORDCHQUAN end as QUAN,
				isnull(INVCHID,ORDCHID) as Detayid
                from SALES 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) on INVSALID = SALID
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID -- ON PROBHDEEDID=DEEDID 
                left outer join ORDERS on ORDSALID = SALID
                left outer join ORDERSCHILD on ORDCHORDID = ORDID
                left outer join PRODUCTS on (PROID = PROBHPROID or PROID = ORDCHPROID)
                where SALID = {0}", SALID);
                var sonuc = conn.GetDataTableConnectionSql(q, sql);
                List<Urunler> urunlers = new List<Urunler>();
                for (int i = 0; i < sonuc.Rows.Count; i++)
                {
                    var PRONAME = sonuc.Rows[i]["PRONAME"].ToString();
                    string pattern = @"VESTEL.*";
                    Match match = Regex.Match(PRONAME, pattern);
                    if (match.Success)
                    {
                        Urunler s = new Urunler();
                        s.PROVAL = sonuc.Rows[i]["PROVAL"].ToString();
                        s.PRONAME = PRONAME;
                        s.QUAN = sonuc.Rows[i]["QUAN"].ToString();
                        s.Detayid =sonuc.Rows[i]["Detayid"].ToString();
                        urunlers.Add(s);
                    }                    
                }
                gridVestelUrunler.DataSource = urunlers;
            }
        }
        internal class Urunler
        {
            public string PROVAL { get; set; }
            public string PRONAME { get; set; }
            public string QUAN { get; set; }
            public string Detayid { get; set; }
        }
        bool satis;
        private void btnVestelTamamlandi_Click(object sender, EventArgs e)
        {
            var selectdrow = ViewVestelSatis.GetSelectedRows();
            var SALID = ViewVestelSatis.GetRowCellValue(selectdrow[0], "SALID").ToString();
            var CURID = ViewVestelSatis.GetRowCellValue(selectdrow[0], "CURID").ToString();
            for (int i = 0; i < ViewVestelUrunler.RowCount; i++)
            {
                string input = ViewVestelUrunler.GetRowCellValue(i, "PRONAME").ToString();
                string pattern = @"VESTEL.*";
                Match match = Regex.Match(input, pattern);
                if (match.Success)
                {
                    string result = match.Value;
                    var Detayid = ViewVestelUrunler.GetRowCellValue(i, "Detayid").ToString();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = sql;
                        cmd.CommandText = "insert into MDE_GENEL.dbo.DivaEklenenler values (@CURID,@SALID,@Detayid,@Islmetarihi)";
                        cmd.Parameters.AddWithValue("@CURID", CURID);
                        cmd.Parameters.AddWithValue("@SALID", SALID);
                        cmd.Parameters.AddWithValue("@Detayid", Detayid);
                        cmd.Parameters.AddWithValue("@Islmetarihi", DateTime.Now);
                        if (sql.State == ConnectionState.Closed)
                        {
                            sql.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }
                    // Ayıklanan değeri işleme
                }
            }
            sql.Close();
            if (satis)
            {
                Yeni_Click(null, null);
                btnSatisListele_Click(null, null);
            }
            else
            {
                Yeni_Click(null, null);
                btnIadeListele_Click(null, null);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            OpenTabForm(new PhoneForm(""));
            //PhoneForm phone = new PhoneForm("");
            //if (phone.Visible)
            //{
            //    XtraMessageBox.Show("Telefon Açık");
            //    phone.Focus();
            //}
            //else
            //{
            //    phone.Show();
            //}
        }

        public void OpenTabForm(Form form)
        {
            bool isFormOpen = false;
            // İlgili formun açık olup olmadığını kontrol etmek için bir bayrak kullanıyoruz.

            for (int i = 0; i < xtraTabControl1.TabPages.Count; i++)
            {

                if (xtraTabControl1.TabPages[i].Text == form.Text)
                {
                    isFormOpen = true;
                    // Aynı isme sahip bir formun açık olduğunu belirledik.
                    break;
                }
            }
            if (isFormOpen)
            {
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AutoCloseOptions.Delay = 2000;
                args.Caption = "Uyarı";
                args.Text = form.Text + "Açık.";
                args.Buttons = new DialogResult[] { DialogResult.OK };
                args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                form.Focus();
                XtraMessageBox.Show(args).ToString();
                form.Close();
                form.Dispose();
                return;
                // Form zaten açıksa kullanıcıyı uyar ve işlemi sonlandır.
            }
            else
            {
                xtraTabControl1.TabPages.Add(form.Text);
                xtraTabControl1.SelectedTabPageIndex = xtraTabControl1.TabPages.Count - 1;
                //form.MdiParent = this;
                navBarControl3.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.Parent = xtraTabControl1.TabPages[xtraTabControl1.TabPages.Count - 1];
                form.Show();
            }

        }
        private void btnGsm1Ara_Click(object sender, EventArgs e)
        {
            if (txtGsm1_V.Text.Substring(1,1)=="0")
            {
                PhoneForm.number = txtGsm1_V.Text;
            }
            else
            {
                PhoneForm.number = "0"+txtGsm1_V.Text;
            }
        }

        private void btnGsm2Ara_Click(object sender, EventArgs e)
        {
            if (txtGsm2_V.Text.Substring(1, 1) == "0")
            {
                PhoneForm.number = txtGsm2_V.Text;
            }
            else
            {
                PhoneForm.number = "0" + txtGsm2_V.Text;
            }
        }

        private void tileBar2_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame2.SelectedPageIndex = tileBarGroup2.Items.IndexOf(e.Item);            
        }

        private void btnVestelGirlien_Click(object sender, EventArgs e)
        {
            string div;
            if (srcVestelMagaza2.EditValue != null)
            {
                div = "= '" + srcVestelMagaza2.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            string q = String.Format(@"select 
            distinct s.SALID,
            c.CURID ,'' as SPEONAME,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
			i.IslemTarihi as SALDATE,
            s.SALDATE as DEEDDATE,
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
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS DEFRENT ,

            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CUSADR ,
			SALAMOUNT
            from MDE_GENEL.dbo.DivaEklenenler i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where SALDATE between '{0}' and '{1}' 
			AND SALDIVISON {2}
            AND PRONAME like 'VESTEL%'
            AND SALDATE is not NULL", Convert.ToDateTime(dteVestelBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridVestelTamamlanan.DataSource = dt;
            btnVestelIadeEt.Visible = false;
        }

        private void btnVestelGirisIade_Click(object sender, EventArgs e)
        {
            string div;
            if (srcVestelMagaza2.EditValue != null)
            {
                div = "= '" + srcVestelMagaza2.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            string q = String.Format(@"select 
            distinct s.SALID,
            c.CURID ,'' as SPEONAME,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
            s.SALDATE,
            '' as DEEDDATE,
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
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS DEFRENT ,
            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CUSADR,
			SALAMOUNT
            from MDE_GENEL.dbo.DivaEklenenler i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALCANSALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where SALDATE between '{0}' and '{1}' 
			AND SALDIVISON {2}
            AND PRONAME like 'VESTEL%'
            AND not exists (select * from MDE_GENEL.dbo.DivaIadeler d where d.SALID = s.SALID)
            AND SALDATE is not NULL", Convert.ToDateTime(dteVestelBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridVestelTamamlanan.DataSource = dt;
            btnVestelIadeEt.Visible = true;
        }

        private void btnProfiloSatis_Click(object sender, EventArgs e)
        {
            string q;
            string div;
            if (srcProfiloMagaza.EditValue != null)
            {
                div = "= '" + srcProfiloMagaza.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            if (Properties.Settings.Default.Company == "VDB_KAMALAR01")
            {
                q = String.Format(@"SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE t WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN ORDERSCHILD WITH (NOLOCK) ON  ORDCHID=PROBHORDCHID  
                LEFT OUTER JOIN ORDERS WITH (NOLOCK) ON ORDID=ORDCHORDID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=ORDSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE between '{0}' and '{1}'
                AND PRONAME like 'PROFI%'
                AND SALDIVISON {2}
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
                union
                SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
				SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE t WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID 
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=INVSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)                
                AND PRONAME like 'PROFI%'", Convert.ToDateTime(dteProfiloBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            else
            {
                q = String.Format(@"SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR ,
                SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE t WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN ORDERSCHILD WITH (NOLOCK) ON  ORDCHID=PROBHORDCHID  
                LEFT OUTER JOIN ORDERS WITH (NOLOCK) ON ORDID=ORDCHORDID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=ORDSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE between '{0}' and '{1}'
                AND PROSUPPLIER.PROSUPCURID IN (3412264)
                AND SALDIVISON {2}
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
                union
                SELECT distinct SALID,
                CURID ,SPEONAME,
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
                (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = CURID) END) AS CUSADR  ,
                SALAMOUNT
                FROM DEEDS WITH (NOLOCK)  
                LEFT OUTER JOIN PRODUCTSBEHAVE t WITH (NOLOCK) ON PROBHDEEDID=DEEDID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID 
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN SALES s WITH (NOLOCK) ON SALID=INVSALID 
                LEFT OUTER JOIN PRODUCTS WITH (NOLOCK) ON PROID=PROBHPROID   
                LEFT OUTER JOIN PRODUCTSCHILD WITH (NOLOCK) ON PROCHID=PROBHPROCHID   
                LEFT OUTER JOIN PROUNIT WITH (NOLOCK) ON PUNIPROID=PROID AND PUNISORT=0 
                LEFT OUTER JOIN DELIVERFEEDBACK WITH (NOLOCK) ON DFBDEEDID=DEEDID   
                LEFT OUTER JOIN DEFPRODUCTDEED WITH (NOLOCK) ON DPDEEDBHVAL=DEEDDPBHEVAL AND DPDEEDDEEDVAL=DEEDDEEDVAL    
                LEFT OUTER JOIN CURRENTS WITH (NOLOCK) ON CURID=DEEDCURID
                LEFT OUTER JOIN CUSIDENTITY WITH (NOLOCK) ON CUSIDCURID = CURID
                LEFT OUTER JOIN PROSUPPLIER WITH (NOLOCK) ON PROSUPPLIER.PROSUPPROID = PROID AND PROSUPPLIER.PROSUPSORT = 1 
			    left outer join SPECIALOFFERSVALID on SPEOVSALID = s.SALID
			    LEFT OUTER JOIN SPECIALOFFERS on SPEOID = SPEOVSPEOID
                WHERE DEEDDPBHEVAL IN (600)
                AND DEEDDEEDVAL IN (3,4,1)
                AND SALID IS NOT NULL 
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)
                AND PROSUPPLIER.PROSUPCURID IN (3412264)", Convert.ToDateTime(dteProfiloBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
                }
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridProfiloSatis.DataSource = dt;
                ViewProfiloSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewProfiloSatis.BestFitColumns(true);
                srcProfiloMagaza.Enabled = false;
                dteProfiloBasTarih.Enabled = false;
                dteProfiloBitTarih.Enabled = false;
                btnProfiloSatis.Enabled = false;
                btnProfiloIade.Enabled = false;
            }
            else
            {
                srcProfiloMagaza.Enabled = true;
                dteProfiloBasTarih.Enabled = true;
                dteProfiloBitTarih.Enabled = true;
                btnProfiloSatis.Enabled = true;
                btnProfiloIade.Enabled = true;

            }
            satis = true;
        }

        private void btnProfiloIade_Click(object sender, EventArgs e)
        {
            string q;
            string div;
            if (srcProfiloMagaza2.EditValue != null)
            {
                div = "= '" + srcProfiloMagaza.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            if (Properties.Settings.Default.Company == "VDB_KAMALAR01")
            {
                q = String.Format(@"SELECT distinct SALID,
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
                AND SALID < 0
                AND DEEDDATE between '{0}' and '{1}'
                AND PRONAME like 'PROFI%'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
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
                AND SALID < 0
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)                
                AND PRONAME like 'PROFI%'", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            else
            {
                q = String.Format(@"SELECT distinct SALID,
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
                AND SALID < 0
                AND DEEDDATE between '{0}' and '{1}'
                AND PROSUPPLIER.PROSUPCURID IN (3412264)
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = ORDCHID)
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
                AND SALID < 0
                AND DEEDDATE  between '{0}' and '{1}'
                AND SALDIVISON {2}
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler where IslemdetayID = INVCHID)
                AND PROSUPPLIER.PROSUPCURID IN (3412264)", Convert.ToDateTime(dteVestelBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteVestelBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            }
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridProfiloSatis.DataSource = dt;
                ViewProfiloSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewProfiloSatis.BestFitColumns(true);
                srcProfiloMagaza.Enabled = false;
                dteProfiloBasTarih.Enabled = false;
                dteProfiloBitTarih.Enabled = false;
                btnProfiloSatis.Enabled = false;
                btnProfiloIade.Enabled = false;
            }
            else
            {
                srcProfiloMagaza.Enabled = true;
                dteProfiloBasTarih.Enabled = true;
                dteProfiloBitTarih.Enabled = true;
                btnProfiloSatis.Enabled = true;
                btnProfiloIade.Enabled = true;

            }
            satis = true;
        }

        private void btnProfiloYeni_Click(object sender, EventArgs e)
        {
            navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            gridProfiloUrunler.DataSource = null;
            gridProfiloSatis.DataSource = null;
            txtMadi_P.Text = "";
            txtSAdi_P.Text = "";
            txtCode_P.Text = "";
            txtIdnty_P.Text = "";
            txtVdate_P.Text = "";
            txtSex_P.Text = "";
            txtMerred_P.Text = "";
            txtCity_P.Text = "";
            txtDefrent_P.Text = "";
            txtGsm1_P.Text = "";
            txtGsm2_P.Text = "";
            txtMail_P.Text = "";
            txtAdr_P.Text = "";
            srcProfiloMagaza.Enabled = true;
            dteProfiloBasTarih.Enabled = true;
            dteProfiloBitTarih.Enabled = true;
            btnProfiloSatis.Enabled = true;
            btnProfiloIade.Enabled = true;
        }

        private void ViewProfiloSatis_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks >= 2 && e.Button == MouseButtons.Left && e.RowHandle >= 0)
            {
                navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                var selectdrow = ViewProfiloSatis.GetSelectedRows();

                txtMadi_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSIDNAME").ToString();
                txtSAdi_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSIDSIRNAME").ToString();
                txtCode_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CURVAL").ToString();
                txtIdnty_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSIDENTY").ToString();
                txtVdate_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSIDBIRTHDAY").ToString();
                txtSex_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSSEX").ToString();
                txtMerred_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSMERRIED").ToString();
                txtCity_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CITY").ToString();
                txtDefrent_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "DEFRENT").ToString();
                txtGsm1_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSGSM1").ToString();
                txtGsm2_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSGSM2").ToString();
                txtMail_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSMAIL").ToString();
                txtAdr_P.Text = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CUSADR").ToString();

                var SALID = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "SALID").ToString();
                string q = String.Format(@"select PROVAL,
                PRONAME,
                case when INVCHID is not NULL then PROBHQUAN else ORDCHQUAN end as QUAN,
				isnull(INVCHID,ORDCHID) as Detayid
                from SALES 
                LEFT OUTER JOIN INVOICE WITH (NOLOCK) on INVSALID = SALID
                LEFT OUTER JOIN INVOICECHILD WITH (NOLOCK) ON INVID=INVCHINVID 
                LEFT OUTER JOIN INVOICECHILDPROBH  WITH (NOLOCK) ON  INVOICECHILDPROBH.INVCHPBHID = INVOICECHILD.INVCHID 
                LEFT OUTER JOIN PRODUCTSBEHAVE WITH (NOLOCK) ON PROBHID = INVCHPBHPROBHID -- ON PROBHDEEDID=DEEDID 
                left outer join ORDERS on ORDSALID = SALID
                left outer join ORDERSCHILD on ORDCHORDID = ORDID
                left outer join PRODUCTS on (PROID = PROBHPROID or PROID = ORDCHPROID)
                where SALID = {0}", SALID);
                var sonuc = conn.GetDataTableConnectionSql(q, sql);
                List<Urunler> urunlers = new List<Urunler>();
                for (int i = 0; i < sonuc.Rows.Count; i++)
                {
                    var PRONAME = sonuc.Rows[i]["PRONAME"].ToString();
                    string pattern = @"PROFILO.*";
                    Match match = Regex.Match(PRONAME, pattern);
                    if (match.Success)
                    {
                        Urunler s = new Urunler();
                        s.PROVAL = sonuc.Rows[i]["PROVAL"].ToString();
                        s.PRONAME = PRONAME;
                        s.QUAN = sonuc.Rows[i]["QUAN"].ToString();
                        s.Detayid = sonuc.Rows[i]["Detayid"].ToString();
                        urunlers.Add(s);
                    }
                }
                gridProfiloUrunler.DataSource = urunlers;
            }
        }

        private void btnGsm1Ara_P_Click(object sender, EventArgs e)
        {
            if (txtGsm1_P.Text.Substring(1, 1) == "0")
            {
                PhoneForm.number = txtGsm1_P.Text;
            }
            else
            {
                PhoneForm.number = "0" + txtGsm1_P.Text;
            }
        }

        private void btnGsm2Ara_P_Click(object sender, EventArgs e)
        {
            if (txtGsm2_P.Text.Substring(1, 1) == "0")
            {
                PhoneForm.number = txtGsm2_P.Text;
            }
            else
            {
                PhoneForm.number = "0" + txtGsm2_P.Text;
            }
        }

        private void btnProfilolTamamlandi_Click(object sender, EventArgs e)
        {
            var selectdrow = ViewProfiloSatis.GetSelectedRows();
            var SALID = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "SALID").ToString();
            var CURID = ViewProfiloSatis.GetRowCellValue(selectdrow[0], "CURID").ToString();
            for (int i = 0; i < ViewProfiloUrunler.RowCount; i++)
            {
                string input = ViewProfiloUrunler.GetRowCellValue(i, "PRONAME").ToString();
                string pattern = @"PROFILO.*";
                Match match = Regex.Match(input, pattern);
                if (match.Success)
                {
                    string result = match.Value;

                    var Detayid = ViewProfiloUrunler.GetRowCellValue(i, "Detayid").ToString();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = sql;
                        cmd.CommandText = "insert into MDE_GENEL.dbo.DivaEklenenler values (@CURID,@SALID,@Detayid,@Islmetarihi)";
                        cmd.Parameters.AddWithValue("@CURID", CURID);
                        cmd.Parameters.AddWithValue("@SALID", SALID);
                        cmd.Parameters.AddWithValue("@Detayid", Detayid);
                        cmd.Parameters.AddWithValue("@Islmetarihi", DateTime.Now);
                        if (sql.State == ConnectionState.Closed)
                        {
                            sql.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }
                    // Ayıklanan değeri işleme
                }
            }
            sql.Close();
            if (satis)
            {
                btnProfiloYeni_Click(null, null);
                btnProfiloSatis_Click(null, null);
            }
            else
            {
                btnProfiloYeni_Click(null, null);
                btnProfiloSatis_Click(null, null);
            }
        }
        private void btnVestelIadeEt_Click(object sender, EventArgs e)
        {
            var selectdrow = ViewVestelTamamlanan.GetSelectedRows();
            var SALID = ViewVestelTamamlanan.GetRowCellValue(selectdrow[0], "SALID").ToString();
            var CURID = ViewVestelTamamlanan.GetRowCellValue(selectdrow[0], "CURID").ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sql;
                cmd.CommandText = "insert into MDE_GENEL.dbo.DivaIadeler values (@CURID,@SALID,@Detayid,@Islmetarihi)";
                cmd.Parameters.AddWithValue("@CURID", CURID);
                cmd.Parameters.AddWithValue("@SALID", SALID);
                cmd.Parameters.AddWithValue("@Detayid", "");
                cmd.Parameters.AddWithValue("@Islmetarihi", DateTime.Now);
                if (sql.State == ConnectionState.Closed)
                {
                    sql.Open();
                }
                cmd.ExecuteNonQuery();
            }
            sql.Close();
            if (satis)
            {
                Yeni_Click(null, null);
                btnSatisListele_Click(null, null);
            }
            else
            {
                Yeni_Click(null, null);
                btnIadeListele_Click(null, null);
            }
        }
        private void btnProfiloIadeEt_Click(object sender, EventArgs e)
        {
            var selectdrow = ViewProfloIade.GetSelectedRows();
            var SALID = ViewProfloIade.GetRowCellValue(selectdrow[0], "SALID").ToString();
            var CURID = ViewProfloIade.GetRowCellValue(selectdrow[0], "CURID").ToString();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sql;
                cmd.CommandText = "insert into MDE_GENEL.dbo.DivaIadeler values (@CURID,@SALID,@Detayid,@Islmetarihi)";
                cmd.Parameters.AddWithValue("@CURID", CURID);
                cmd.Parameters.AddWithValue("@SALID", SALID);
                cmd.Parameters.AddWithValue("@Detayid", "");
                cmd.Parameters.AddWithValue("@Islmetarihi", DateTime.Now);
                if (sql.State == ConnectionState.Closed)
                {
                    sql.Open();
                }
                cmd.ExecuteNonQuery();
            }
            sql.Close();
            if (satis)
            {
                Yeni_Click(null, null);
                btnSatisListele_Click(null, null);
            }
            else
            {
                Yeni_Click(null, null);
                btnIadeListele_Click(null, null);
            }
        }

        private void btnProfiloGiris_Click(object sender, EventArgs e)
        {
            string div;
            if (srcVestelMagaza2.EditValue != null)
            {
                div = "= '" + srcVestelMagaza2.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            string q = String.Format(@"select 
            distinct s.SALID,
            c.CURID ,'' as SPEONAME,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
			i.IslemTarihi as DEEDDATE,
            s.SALDATE as SALDATE,
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
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS DEFRENT ,

            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CUSADR ,
			SALAMOUNT
            from MDE_GENEL.dbo.DivaEklenenler i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where SALDATE between '{0}' and '{1}' 
            AND PRONAME like 'PROFILO%'
			AND SALDIVISON {2}
            AND SALDATE is not NULL", Convert.ToDateTime(dteProfiloBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridProfloIade.DataSource = dt;
            btnVestelIadeEt.Visible = false;
            btnProfiloIadeEt.Visible = true;
        }

        private void btnProfiloIadeListe_Click(object sender, EventArgs e)
        {
            gridProfloIade.DataSource = null;
            string div;
            if (srcVestelMagaza2.EditValue != null)
            {
                div = "= '" + srcVestelMagaza2.EditValue.ToString() + "'";
            }
            else
            {
                div = "!='00'";
            }
            string q = String.Format(@"select 
            distinct s.SALID,
            c.CURID ,'' as SPEONAME,
            case when 
            (select CURCHEINVOICE from CURRENTSCHILD where CURCHID = SALCURID) = 1 then 'KURUMSAL Müşteri' else 'Bireysel Müşteri' end as CURTYPE,
            CURNAME,
            (select DIVNAME from DIVISON where DIVVAL = SALDIVISON) as DIVISON,
			i.IslemTarihi as DEEDDATE,
            s.SALDATE as SALDATE,
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
            (SELECT TOP 1 CURCHCITY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CITY ,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSADISTRICT  FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHCOUNTY FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS DEFRENT ,

            (select CURCHGSM1 from CURRENTSCHILD where CURCHID = SALCURID) as CUSGSM1,
            (select CURCHGSM2 from CURRENTSCHILD where CURCHID = SALCURID and CURCHGSM1 != CURCHGSM2)  as CUSGSM2,
            (select CURCHEMAIL from CURRENTSCHILD where CURCHID = SALCURID)  as CUSMAIL,
            (CASE     WHEN(SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID) >= 0 THEN
            (SELECT CSAADR1 + ' ' + CSAADR2 FROM CURSHIPADR WITH (NOLOCK) WHERE CSAID = (SELECT TOP 1 C1.CDRCSAID FROM CUSDELIVER C1 WITH (NOLOCK) WHERE  C1.CDRORDCHID = ORDCHID))
            ELSE
            (SELECT TOP 1 CURCHADR1 + ' ' + CURCHADR2 FROM CURRENTSCHILD WITH (NOLOCK) WHERE CURRENTSCHILD.CURCHID = c.CURID) END) AS CUSADR ,
			SALAMOUNT
            from MDE_GENEL.dbo.DivaEklenenler i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALCANSALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where SALDATE between '{0}' and '{1}' 
            AND PRONAME like 'PROFILO%'
			AND SALDIVISON {2}
            AND SALDATE is not NULL", Convert.ToDateTime(dteProfiloBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridProfloIade.DataSource = dt;
            btnVestelIadeEt.Visible = false;
            btnProfiloIadeEt.Visible = true;
        }

        private void btnPorifloExcel_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lblSayfaIsmi.Text, DateTime.Now.ToString("yyyy-MM-dd")+ ".xls");
            CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lblSayfaIsmi.Text));


            gridProfloIade.ExportToXls(filePath, new XlsExportOptions
            {
                ExportMode = XlsExportMode.SingleFile,
                TextExportMode = TextExportMode.Value,
                ShowGridLines = true,
                FitToPrintedPageWidth = true,
                FitToPrintedPageHeight = true,
            });
            Process.Start("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lblSayfaIsmi.Text));
        }
        private void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                    //MessageBox.Show($"'{directoryPath}' dizini oluşturuldu Dosya buraya kaydedilecek", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void btnDosyaSec_ItemClick(object sender, TileItemEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Dosyaları (*.xlsx;*.xls)|*.xlsx;*.xls|Tüm Dosyalar (*.*)|*.*";

            // Kullanıcıdan dosyayı seçmesini iste
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //txtDosyaYolu.Text = openFileDialog.FileName;
                Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;

                // Çalışma sayfasının içeriğini temizle
                worksheet.Clear(worksheet.GetDataRange());
                spreadsheetControl.Document.BeginUpdate();
                spreadsheetControl.Document.LoadDocument(openFileDialog.FileName, DocumentFormat.Xlsx);
                spreadsheetControl.Document.EndUpdate();

                Worksheet worksheet2 = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
                CellRange usedRange = worksheet2.GetUsedRange();

                // Dolu sütunları dolaşarak sıralı harf isimlerini elde edin
                for (int columnIndex = usedRange.LeftColumnIndex; columnIndex <= usedRange.RightColumnIndex + 1; columnIndex++)
                {
                    // Sütunun harf karşılığını hesaplayın
                    string columnName = GetColumnName(columnIndex);
                    // Sütun ismini listeye ekleyin
                    cmbMusterino.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbFatid.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbEETN.Items.Add(new ComboBoxItem(columnIndex, columnName));
                }
                spreadsheetControl.Document.Worksheets.ActiveWorksheet.Cells.AutoFitColumns();
            }
        }
        public class ComboBoxItem
        {
            public int Value { get; set; }
            public string DisplayText { get; set; }

            public ComboBoxItem(int value, string displayText)
            {
                Value = value;
                DisplayText = displayText;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }
        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string columnName = "";

            while (index > 0)
            {
                int remainder = (index - 1) % 26;
                columnName = letters[remainder] + columnName;
                index = (index - 1) / 26;
            }

            return columnName;
        }
        public static int HarfinSirasi(string harf)
        {
            if (harf == "I")
            {
                harf = "i";
            }
            // Küçük harfleri kontrol etmek için harfin ASCII değerini 97'ye çıkarırız.
            // Büyük harfler için bu gerekli değildir.
            harf = harf.ToLower();

            // Harfin alfabedeki sırasını bulmak için ASCII tablosundaki indeksi kullanırız.
            // Küçük harfler için indeksler 0'dan 25'e kadar, büyük harfler için 0'dan 25'e kadar.
            int sira = harf[0] - 'a' + 1;

            return sira;
        }

        private void btnKaydet_ItemClick(object sender, TileItemEventArgs e)
        {
            int index = 0;
            if (!toggleSwitch1.IsOn)
            {
                index = 1;
            }
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet.GetUsedRange();

            // Sıralama yapılacak sütunu al
            //string siraSutunu = cmbSehir.Text;

            //CellRange range = worksheet.Range[cmbCinsiyet.Text+ (index+1)+":"+cmbTarih.Text+"69999"];
            //worksheet.Sort(range, HarfinSirasi(cmbSehir.Text)-1);

            //worksheet.Sort(usedRange,HarfinSirasi(cmbSehir.Text)-1, true);

            List<EArsiv> faturlar = new List<EArsiv>();
            HashSet<string> uniquePlakaSet = new HashSet<string>();

            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = usedRange.RowCount,
                Position = 0,
                ToplamAdet = usedRange.RowCount.ToString(),
            };
            var a1 = HarfinSirasi(cmbMusterino.Text);
            var a2 = HarfinSirasi(cmbFatid.Text);
            var a3 = HarfinSirasi(cmbEETN.Text);
            int success = 0;
            int error = 0;
            this.Enabled = false;
            if (Properties.Settings.Default.LogoToken == "")
            {
                LogoLogin();
            }
            if (Properties.Settings.Default.ParkToken == "")
            {
                ParkToken();
            }
            Logo.PostBoxServiceClient client = new Logo.PostBoxServiceClient("PostBoxServiceEndpoint");
            executeBackground(
       () =>
       {
           progressForm.Show(this);
           for (int rowIndex = index; rowIndex < usedRange.RowCount; rowIndex++)
           {
               string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lblSayfaIsmi.Text, usedRange[rowIndex, a1-1].Value.ToString());
               CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), lblSayfaIsmi.Text, usedRange[rowIndex, a1-1].Value.ToString()));
               try
               {
                   Logo.GetDocumentType document = Logo.GetDocumentType.EARCHIVE;
                   Logo.DocumentDataType dataType = Logo.DocumentDataType.UBL;
                   string UUID = usedRange[rowIndex, a3 - 1].Value.ToString().ToUpper();
                   var sonuc = client.getDocumentData(Properties.Settings.Default.LogoToken, UUID, document, dataType);

                   byte[] zipData = sonuc.binaryData.Value;

                   //var base64Data = sonuc.binaryData.Value;

                   //byte[] zipData = Convert.FromBase64String(base64Data);

                   // ZIP dosyasının adını belirleyin
                   string fileName = filePath+"/"+sonuc.fileName;

                   // ZIP dosyasını kaydedin
                   //File.WriteAllBytes(fileName, zipData);
                   string zipFilePath = fileName;

                   // Çıkartılacak klasörün yolu
                   string extractPath = filePath;

                   using (FileStream zipFileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
                   using (ZipArchive archive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
                   {
                       foreach (ZipArchiveEntry entry in archive.Entries)
                       {
                           // Klasörse, devam et
                           if (string.IsNullOrEmpty(entry.Name))
                           {
                               // Klasör oluştur
                               string directoryPath = Path.Combine(extractPath, entry.FullName);
                               Directory.CreateDirectory(directoryPath);
                           }
                           else
                           {
                               // Dosya yolunu oluştur
                               string filePath2 = Path.Combine(extractPath, entry.FullName);

                               // Klasör olup olmadığını kontrol et
                               string directoryPath = Path.GetDirectoryName(filePath2);
                               if (!Directory.Exists(directoryPath))
                               {
                                   Directory.CreateDirectory(directoryPath);
                               }

                               // Dosyayı çıkar
                               using (Stream entryStream = entry.Open())
                               using (FileStream fileStream = new FileStream(filePath2, FileMode.Create))
                               {
                                   entryStream.CopyTo(fileStream);
                               }
                           }
                       }
                   }


                   EArsiv fat = new EArsiv();
                   fat.sira = rowIndex;
                   fat.CURVAL = usedRange[rowIndex, a1-1].Value.ToString();
                   fat.FaturaID = usedRange[rowIndex, a2-1].Value.ToString();
                   fat.ETTN = usedRange[rowIndex, a3-1].Value.ToString();
                   faturlar.Add(fat);
                   progressForm.PerformStep(this);
                   success++;
               }
               catch (Exception mesaj)
               {
                   progressForm.PerformStep(this);
                   error++;
                   XtraMessageBox.Show(mesaj.Message);
               }
           }
       },
                              null,
                              () =>
                              {
                                  client.Logout(Properties.Settings.Default.LogoToken);
                                  completeProgress();
                                  progressForm.Hide(this);
                                  Properties.Settings.Default.LogoToken = "";
                                  Properties.Settings.Default.ParkToken = "";
                                  var sonuc = converter.ToDataTable(faturlar);
                              });
        }
        public void LogoLogin()
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
                    passWord = "MagicUser20!",
                };
                string sesionid = "";
                bool ss = client.Login(user, out sesionid); //logoservis.serviceClient.Login(user, out sesionid);
                if (ss)
                {
                    Properties.Settings.Default.LogoToken = sesionid;
                    //Logo.GetDocumentType document = Logo.GetDocumentType.EARCHIVE;
                    //Logo.DocumentDataType dataType = Logo.DocumentDataType.UBL;
                    //string UUID = "69ead0d2-3ede-4b49-bd0b-fcfdc5db3fee".ToUpper();
                    //var sonuc = client.getDocumentData(sesionid, UUID, document, dataType);

                    //byte[] zipData = sonuc.binaryData.Value;

                    ////var base64Data = sonuc.binaryData.Value;

                    ////byte[] zipData = Convert.FromBase64String(base64Data);

                    //// ZIP dosyasının adını belirleyin
                    //string fileName = sonuc.fileName;

                    //// ZIP dosyasını kaydedin
                    //File.WriteAllBytes(fileName, zipData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
    }    
}
