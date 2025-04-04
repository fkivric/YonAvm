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
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using DevExpress.XtraPrinting;
using System.IO;
using DevExpress.Spreadsheet;
using System.IO.Compression;
using YonAvm.EFaturaIntegrationService;
using static YonAvm.Class.FaturaVeri;
using System.Net;
using System.ServiceModel;
using YonAvm.Class;

namespace YonAvm
{
    public partial class frmMain2 : DevExpress.XtraEditors.XtraForm
    {
        public frmMain2()
        {
            InitializeComponent();
            DBName = Properties.Settings.Default.Company;
        }
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnection sql2 = new SqlConnection(Properties.Settings.Default.connectionstring2);
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

        private void frmMain2_Load(object sender, EventArgs e)
        {
            dteProfiloBasTarih.EditValue = DateTime.Now;
            dteProfiloBitTarih.EditValue = DateTime.Now;
            dteProfiloBasTarih.Properties.MaxValue = DateTime.Now;
            dteProfiloBitTarih.Properties.MaxValue = DateTime.Now;

            dteProfiloBasTarih2.EditValue = DateTime.Now;
            dteProfiloBitTarih2.EditValue = DateTime.Now;
            dteProfiloBasTarih2.Properties.MaxValue = DateTime.Now;
            dteProfiloBitTarih2.Properties.MaxValue = DateTime.Now;
            //navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
        }
        private void tileBar2_SelectedItemChanged(object sender, TileItemEventArgs e)
        {
            navigationFrame2.SelectedPageIndex = tileBarGroup2.Items.IndexOf(e.Item);
        }
        internal class Urunler
        {
            public string PROVAL { get; set; }
            public string PRONAME { get; set; }
            public string QUAN { get; set; }
            public string Detayid { get; set; }
        }
        bool satis;
        private static string DBName = "";
        private void btnProfiloSatis_Click(object sender, EventArgs e)
        {
            string q = String.Format(@"SELECT distinct SALID,
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
                AND SALDIVISON = '03'
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler2 where IslemdetayID = ORDCHID)
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
                AND SALDIVISON= '03'
                AND s.SALID > 0
				AND not exists (select * from PRODUCTSBEHAVE ti where ti.PROBHCANCELID = t.PROBHID)
			    AND not exists (select * from SALES i where i.SALCANSALID = s.SALID)
			    AND NOT EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler2 where IslemdetayID = INVCHID)", Convert.ToDateTime(dteProfiloBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih.EditValue).ToString("yyyy-MM-dd"));

            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = 1,
                Position = 0,
                ToplamAdet = 1.ToString(),
            };
            int success = 0;
            int error = 0;
            DataTable dt = new DataTable();
            executeBackground(
       () =>
       {
           progressForm.Show(this);
           try
           {
               progressForm.UpdateDetails("Veriler Çekiliyor");
               dt = conn.GetDataTableConnectionSql(q, sql);
               progressForm.PerformStep(this);
               success = dt.Rows.Count;
           }
           catch (Exception)
           {
               progressForm.PerformStep(this);
               error++;
           }
       },
                              null,
                              () =>
                              {
                                  completeProgress();
                                  progressForm.Hide(this);

                                  if (dt.Rows.Count > 0)
                                  {
                                      gridProfiloSatis.DataSource = dt;
                                      ViewProfiloSatis.OptionsView.BestFitMaxRowCount = -1;
                                      ViewProfiloSatis.BestFitColumns(true);
                                      dteProfiloBasTarih.Enabled = false;
                                      dteProfiloBitTarih.Enabled = false;
                                      btnProfiloSatis.Enabled = false;
                                      btnProfiloIade.Enabled = false;
                                  }
                                  else
                                  {
                                      dteProfiloBasTarih.Enabled = true;
                                      dteProfiloBitTarih.Enabled = true;
                                      btnProfiloSatis.Enabled = true;
                                      btnProfiloIade.Enabled = true;

                                  }
                                  satis = true;
                                  XtraMessageBox.Show("Başarılı :" + success + " adet işlem listelendi.", "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
                              });
        }

        private void ViewProfiloSatis_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.Clicks >= 2 && e.Button == MouseButtons.Left && e.RowHandle >= 0)
            {
                ProgressBarFrm progressForm = new ProgressBarFrm()
                {
                    Start = 0,
                    Finish = 1,
                    Position = 0,
                    ToplamAdet = 1.ToString(),
                };
                int success = 0;
                int error = 0;
                List<Urunler> urunlers = new List<Urunler>();
                executeBackground(
           () =>
           {
               progressForm.Show(this);
               Invoke((MethodInvoker)delegate
               {
                   var selectdrow = ViewProfiloSatis.GetSelectedRows();
                   progressForm.UpdateDetails("Veriler işleniyor");
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
                   progressForm.UpdateDetails("Satış işleniyor");
                   progressForm.UpdateDetails("Ürünler işleniyor");
                   for (int i = 0; i < sonuc.Rows.Count; i++)
                   {
                       var PRONAME = sonuc.Rows[i]["PRONAME"].ToString();
                       //string pattern = @"PROFILO.*";
                       //Match match = Regex.Match(PRONAME, pattern);
                       //if (match.Success)
                       //{
                       Urunler s = new Urunler();
                       s.PROVAL = sonuc.Rows[i]["PROVAL"].ToString();
                       s.PRONAME = PRONAME;
                       s.QUAN = sonuc.Rows[i]["QUAN"].ToString();
                       s.Detayid = sonuc.Rows[i]["Detayid"].ToString();
                       urunlers.Add(s);
                       //}
                   }
               });
               progressForm.PerformStep(this);
               success++;
            },
                              null,
                              () =>
                              {
                                  completeProgress();
                                  progressForm.Hide(this);
                                  gridProfiloUrunler.DataSource = urunlers;
                                  navBarControl2.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                                  XtraMessageBox.Show("Başarılı :" + success + " adet işlem listelendi.", "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
                              });
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
                        cmd.CommandText = "insert into MDE_GENEL.dbo.DivaEklenenler2 values (@CURID,@SALID,@Detayid,@Islmetarihi)";
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
            dteProfiloBasTarih.Enabled = true;
            dteProfiloBitTarih.Enabled = true;
            btnProfiloSatis.Enabled = true;
            btnProfiloIade.Enabled = true;
        }

        private void btnProfiloIade_Click(object sender, EventArgs e)
        {
            string q;
            string div = frmLogin.DIVVAL;
            

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
                AND SALDIVISON = '{2}'
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler2 where IslemdetayID = ORDCHID)
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
                AND SALDIVISON = '{2}'
			    AND EXISTS (select * from MDE_GENEL.dbo.DivaEklenenler2 where IslemdetayID = INVCHID)", Convert.ToDateTime(dteProfiloBasTarih.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih.EditValue).ToString("yyyy-MM-dd"), div);
            var dt = conn.GetDataTableConnectionSql(q, sql);
            if (dt.Rows.Count > 0)
            {
                gridProfiloSatis.DataSource = dt;
                ViewProfiloSatis.OptionsView.BestFitMaxRowCount = -1;
                ViewProfiloSatis.BestFitColumns(true);
                dteProfiloBasTarih.Enabled = false;
                dteProfiloBitTarih.Enabled = false;
                btnProfiloSatis.Enabled = false;
                btnProfiloIade.Enabled = false;
            }
            else
            {
                dteProfiloBasTarih.Enabled = true;
                dteProfiloBitTarih.Enabled = true;
                btnProfiloSatis.Enabled = true;
                btnProfiloIade.Enabled = true;

            }
            satis = false;
        }

        private void btnProfiloGiris_Click(object sender, EventArgs e)
        {
            string div = frmLogin.DIVVAL;
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
            from MDE_GENEL.dbo.DivaEklenenler2 i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where i.IslemTarihi between '{0}' and '{1}' 
			AND SALDIVISON = '{2}'
            AND SALDATE is not NULL", Convert.ToDateTime(dteProfiloBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridProfloIade.DataSource = dt;
            btnProfiloIadeEt.Visible = false;
        }

        private void btnProfiloIadeListe_Click(object sender, EventArgs e)
        {
            gridProfloIade.DataSource = null;
            string div = frmLogin.DIVVAL;
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
            from MDE_GENEL.dbo.DivaEklenenler2 i
            left outer join CURRENTS c on c.CURID = i.CURID
			left outer join CUSIDENTITY on CUSIDCURID=c.CURID
            left outer join SALES s on s.SALCANSALID = i.SALID
            left outer join DIVISON on DIVVAL = SALDIVISON
            left outer join ORDERSCHILD on ORDCHID = i.IslemdetayID
            left outer join PRODUCTS on PROID = ORDCHPROID
			where i.IslemTarihi between '{0}' and '{1}'
			AND SALDIVISON = '{2}'
            AND SALDATE is not NULL", Convert.ToDateTime(dteProfiloBasTarih2.EditValue).ToString("yyyy-MM-dd"), Convert.ToDateTime(dteProfiloBitTarih2.EditValue).ToString("yyyy-MM-dd"), div);
            SqlDataAdapter da = new SqlDataAdapter(q, sql);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gridProfloIade.DataSource = dt;
            btnProfiloIadeEt.Visible = true;
        }

        private void btnPorifloExcel_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), this.Text, DateTime.Now.ToString("yyyy-MM-dd") + ".xls");
            CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), this.Text));


            gridProfloIade.ExportToXls(filePath, new XlsExportOptions
            {
                ExportMode = XlsExportMode.SingleFile,
                TextExportMode = TextExportMode.Value,
                ShowGridLines = true,
                FitToPrintedPageWidth = true,
                FitToPrintedPageHeight = true,
            });
            Process.Start("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), this.Text));
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

                }
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
                cmd.CommandText = "insert into MDE_GENEL.dbo.DivaIadeler2 values (@CURID,@SALID,@Detayid,@Islmetarihi)";
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
        }

        private void btnDosyaSec_ItemClick(object sender, TileItemEventArgs e)
        {
            cmbMusterino.Items.Clear();
            cmbMusteriAdi.Items.Clear();
            cmbDivaID.Items.Clear();
            cmbFaturaTarihi.Items.Clear();
            cmbFaturaTipi.Items.Clear();
            cmbFatid.Items.Clear();
            cmbETTN.Items.Clear();
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
                    cmbMusteriAdi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbDivaID.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbFaturaTarihi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbFaturaTipi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbFatid.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbETTN.Items.Add(new ComboBoxItem(columnIndex, columnName));
                }
                spreadsheetControl.Document.Worksheets.ActiveWorksheet.Cells.AutoFitColumns();
                navBarControl4.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
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
            var Musterino = HarfinSirasi(cmbMusterino.Text);
            var MusteriAdi = HarfinSirasi(cmbMusteriAdi.Text);
            var DivaID = HarfinSirasi(cmbDivaID.Text);
            var FaturaTarihi = HarfinSirasi(cmbFaturaTarihi.Text);
            var FaturaTipi = HarfinSirasi(cmbFaturaTipi.Text);
            var Fatid = HarfinSirasi(cmbFatid.Text);
            var ETTN = HarfinSirasi(cmbETTN.Text);
            int success = 0;
            int error = 0;
            this.Enabled = false;
            if (Properties.Settings.Default.LogoToken == "")
            {
                LogoToken();
            }
            if (Properties.Settings.Default.ParkToken == "")
            {
                ParkToken();
            }
            Logo.PostBoxServiceClient client = new Logo.PostBoxServiceClient("PostBoxServiceEndpoint");
            EFaturaIntegration parkclient = new EFaturaIntegration();
            executeBackground(
       () =>
       {
           progressForm.Show(this);
           for (int rowIndex = index; rowIndex < usedRange.RowCount; rowIndex++)
           {
               if (rowIndex == 600)
               {

               }
               DateTime gidistarihi = DateTime.Now.AddDays(-30);
               EArsiv fat = new EArsiv();
               fat.sira = rowIndex;
               string q = String.Format(@"select d.CURID,d.SALID,Convert(char(10),IslemTarihi,121) as FTRDATE,SALAMOUNT from MDE_GENEL.dbo.DivaEklenenler2 d
                left outer join {1}.dbo.SALES vs on vs.SALID = d.SALID
                left outer join {1}.dbo.CURRENTS vc on vc.CURID = d.CURID
                where CURVAL = '{0}'
                and not exists (select * from MDE_GENEL.dbo.DivaFaturaListesi2 where VOL_CURID = d.CURID and VOL_SALID = d.SALID)
                --and not exists (select * from {1}.dbo.SALESINVOICE where SALINVSALID = d.SALID)", usedRange[rowIndex, Musterino - 1].Value.ToString(), DBName);
               var satisverisi = conn.GetDataTableConnectionSql(q, sql);
               if (satisverisi.Rows.Count > 1)
               {
                   SatisSec sec = new SatisSec(usedRange[rowIndex, Musterino - 1].Value.ToString());
                   sec.MUSTERIADI = usedRange[rowIndex, MusteriAdi - 1].Value.ToString();
                   sec.SATISTARİHİ = usedRange[rowIndex, FaturaTarihi - 1].Value.ToString();
                   sec.ShowDialog();
                   if (sec.CURID != null)
                   {
                       fat.VOL_CURID = sec.CURID;
                   }
                   else
                   {
                       fat.VOL_CURID = satisverisi.Rows[0]["CURID"].ToString();
                   }
                   fat.VOL_SALID = sec.SALID;
               }
               else if (satisverisi.Rows.Count == 1)
               {
                   fat.VOL_CURID = satisverisi.Rows[0]["CURID"].ToString();
                   fat.VOL_SALID = satisverisi.Rows[0]["SALID"].ToString();
               }
               else
               {

                   var sadeceveri = conn.GetDataTableConnectionSql(String.Format(@"select distinct d.CURID,d.SALID,l.DIV_FTRDATE from CURRENTS c
                                                                                   left outer join MDE_GENEL.dbo.DivaEklenenler2 d on c.CURID = d.CURID
                                                                                   left outer join MDE_GENEL.dbo.DivaFaturaListesi2 l on l.VOL_CURID = d.CURID and l.VOL_SALID = d.SALID
                                                                                   where CURVAL = '{0}'", usedRange[rowIndex, Musterino - 1].Value.ToString()), sql);
                   if (sadeceveri.Rows[0]["DIV_FTRDATE"].ToString() != "")
                   {
                       fat.VOL_CURID = sadeceveri.Rows[0]["CURID"].ToString();
                       fat.VOL_SALID = sadeceveri.Rows[0]["SALID"].ToString();
                       gidistarihi = Convert.ToDateTime(usedRange[rowIndex, FaturaTarihi - 1].Value.ToString());
                   }
                   else
                   {

                   }
               }
               fat.DIV_SALID = usedRange[rowIndex, DivaID - 1].Value.ToString();
               fat.DIV_FTRDATE = usedRange[rowIndex, FaturaTarihi - 1].Value.ToString();
               fat.DIV_FTRNO = usedRange[rowIndex, Fatid - 1].Value.ToString();
               fat.DIV_ETTN = usedRange[rowIndex, ETTN - 1].Value.ToString().ToUpper();
               string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), this.Text, usedRange[rowIndex, MusteriAdi - 1].Value.ToString());
               CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), this.Text, usedRange[rowIndex, MusteriAdi - 1].Value.ToString()));
               try
               {
                   Logo.GetDocumentType document;
                   if (usedRange[rowIndex, FaturaTipi - 1].Value.ToString() == "e-Arşivlendi")
                   {
                       document = Logo.GetDocumentType.EARCHIVE;
                   }
                   else
                   {
                       document = Logo.GetDocumentType.EINVOICE;
                   }
                   Logo.DocumentDataType dataType = Logo.DocumentDataType.UBL;
                   string UUID = usedRange[rowIndex, ETTN - 1].Value.ToString().ToUpper();
                   var sonuc = client.getDocumentData(Properties.Settings.Default.LogoToken, UUID, document, dataType);

                   byte[] zipData = sonuc.binaryData.Value;

                   //var base64Data = sonuc.binaryData.Value;

                   //byte[] zipData = Convert.FromBase64String(base64Data);

                   // ZIP dosyasının adını belirleyin
                   string fileName = filePath + "/" + sonuc.fileName;

                   // ZIP dosyasını kaydedin
                   File.WriteAllBytes(fileName, zipData);

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
                               var ssss = File.ReadAllBytes(filePath2);
                               var data = parkclient.ESaklamaFaturaYukle(Properties.Settings.Default.ParkToken, usedRange[rowIndex, Fatid - 1].Value.ToString(), File.ReadAllBytes(filePath2));
                               if (data.data != null)
                               {
                                   fat.PARK_NEWID = data.data.contents.First().id.ToString();
                               }
                               else
                               {
                                   var gidenfatura = parkclient.ESaklamaGidenFaturaListele(Properties.Settings.Default.ParkToken, gidistarihi, gidistarihi.AddDays(1));
                                   if (gidenfatura.error == null && gidenfatura.warning == null)
                                   {
                                       if (gidenfatura.data != null)
                                       {
                                           foreach (var item in gidenfatura.data.contents)
                                           {
                                               if (item.documentNo == usedRange[rowIndex, Fatid - 1].Value.ToString())
                                               {
                                                   var idsor = conn.GetValueConnectionSql(String.Format(@"select PARK_NEWID from MDE_GENEL.dbo.DivaFaturaListesi2 where DIV_FTRNO = '{0}' and DIV_SALID != 0", usedRange[rowIndex, Fatid - 1].Value.ToString()), sql);
                                                   if (idsor == "1" && idsor != "")
                                                   {
                                                       conn.insertData(String.Format("update MDE_GENEL.dbo.DivaFaturaListesi2 set PARK_NEWID = {1} where  DIV_FTRNO = '{0}'", usedRange[rowIndex, Fatid - 1].Value.ToString(), item.id), sql);
                                                   }
                                                   var pdf = parkclient.ESaklamaGidenFaturaPdfAlById(Properties.Settings.Default.ParkToken, item.id);
                                                   File.WriteAllBytes(Path.Combine(extractPath, usedRange[rowIndex, Fatid - 1].Value.ToString() + ".pdf"), pdf);
                                                   fat.PARK_NEWID = item.id.ToString();
                                                   string insertq = String.Format(@"insert into MDE_GENEL.dbo.DivaFaturaListesi2 values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", fat.VOL_CURID, fat.VOL_SALID, fat.DIV_SALID, Convert.ToDateTime(fat.DIV_FTRDATE).ToString("yyyy-MM-dd"), fat.DIV_FTRNO, fat.DIV_ETTN, fat.PARK_NEWID);
                                                   int insert = conn.insertData(insertq, sql);
                                               }
                                               //Console.WriteLine(item.documentNo + " numaralı belge");
                                           }
                                       }

                                   }
                                   fat.PARK_NEWID = 0000001.ToString();
                               }
                               //var gidenfatura = parkclient.ESaklamaGidenFaturaListele(Properties.Settings.Default.ParkToken, DateTime.Now.AddDays(-30), DateTime.Now);
                               if (data.data != null)
                               {
                                   string insertq = String.Format(@"insert into MDE_GENEL.dbo.DivaFaturaListesi2 values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", fat.VOL_CURID, fat.VOL_SALID, fat.DIV_SALID, Convert.ToDateTime(fat.DIV_FTRDATE).ToString("yyyy-MM-dd"), fat.DIV_FTRNO, fat.DIV_ETTN, fat.PARK_NEWID);
                                   int insert = conn.insertData(insertq, sql);

                               }
                               //using (SqlCommand cmd = new SqlCommand())
                               //{
                               //    cmd.CommandType = CommandType.Text;
                               //    cmd.Connection = sql;
                               //    cmd.CommandTimeout = 300;
                               //    cmd.CommandText = "insert into MDE_GENEL.dbo.DivaFaturaListesi values ('@VOL_CURID','@VOL_SALID','@DIV_SALID','@DIV_FTRDATE','@DIV_FTRNO','@DIV_ETTN','@PARK_NEWID')";
                               //    cmd.Parameters.AddWithValue("@VOL_CURID", fat.VOL_CURID);
                               //    cmd.Parameters.AddWithValue("@VOL_SALID", fat.VOL_SALID);
                               //    cmd.Parameters.AddWithValue("@DIV_SALID", fat.DIV_SALID);
                               //    cmd.Parameters.AddWithValue("@DIV_FTRDATE", fat.DIV_FTRDATE);
                               //    cmd.Parameters.AddWithValue("@DIV_FTRNO", fat.DIV_FTRNO);
                               //    cmd.Parameters.AddWithValue("@DIV_ETTN", fat.DIV_ETTN);
                               //    cmd.Parameters.AddWithValue("@PARK_NEWID", fat.PARK_NEWID);
                               //    if (sql.State == ConnectionState.Closed)
                               //    {
                               //        sql.Open();
                               //    }
                               //    cmd.ExecuteNonQuery();
                               //}
                           }
                       }
                   }
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
                                  CustomMessageBox.ShowMessage("","Başarılı :" + success + " adet işlem kaydetildi. Hatalı işlem Toplamı :" + error,this, "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                  worksheet.Clear(worksheet.GetDataRange());
                              });
        }
        EFaturaIntegration Clint = new EFaturaIntegration();
        void ParkToken()
        {
            var token = Clint.OturumAc("kamalar", "Park2019.");
            if (token.IsSuccessLogin)
            {
                Properties.Settings.Default.ParkToken = token.SessionId;
            }
        }
        public void LogoToken()
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
                    userName = "7610537931",
                    passWord = "Y5043618*+yy",
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