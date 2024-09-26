namespace YonAvm
{
    partial class SatisSec
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SatisSec));
            this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
            this.tileBarGroup2 = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.btnSec = new DevExpress.XtraBars.Navigation.TileBarItem();
            this.gridListe = new DevExpress.XtraGrid.GridControl();
            this.ViewListe = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridListe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewListe)).BeginInit();
            this.SuspendLayout();
            // 
            // tileBar1
            // 
            this.tileBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            this.tileBar1.Groups.Add(this.tileBarGroup2);
            this.tileBar1.ItemPadding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.tileBar1.Location = new System.Drawing.Point(0, 302);
            this.tileBar1.MaxId = 2;
            this.tileBar1.Name = "tileBar1";
            this.tileBar1.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
            this.tileBar1.Size = new System.Drawing.Size(519, 95);
            this.tileBar1.TabIndex = 0;
            this.tileBar1.Text = "tileBar1";
            // 
            // tileBarGroup2
            // 
            this.tileBarGroup2.Items.Add(this.btnSec);
            this.tileBarGroup2.Name = "tileBarGroup2";
            // 
            // btnSec
            // 
            this.btnSec.AppearanceItem.Normal.BackColor = System.Drawing.Color.Black;
            this.btnSec.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.Blue;
            this.btnSec.AppearanceItem.Normal.Options.UseBackColor = true;
            this.btnSec.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            tileItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            tileItemElement2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "İşaretli Olanı Seç";
            tileItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            this.btnSec.Elements.Add(tileItemElement2);
            this.btnSec.Id = 0;
            this.btnSec.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.btnSec.Name = "btnSec";
            this.btnSec.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.btnSec_ItemClick);
            // 
            // gridListe
            // 
            this.gridListe.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridListe.Location = new System.Drawing.Point(0, 35);
            this.gridListe.MainView = this.ViewListe;
            this.gridListe.Name = "gridListe";
            this.gridListe.Size = new System.Drawing.Size(519, 267);
            this.gridListe.TabIndex = 1;
            this.gridListe.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ViewListe});
            // 
            // ViewListe
            // 
            this.ViewListe.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.ViewListe.GridControl = this.gridListe;
            this.ViewListe.Name = "ViewListe";
            this.ViewListe.OptionsSelection.MultiSelect = true;
            this.ViewListe.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.ViewListe.OptionsView.ColumnAutoWidth = false;
            this.ViewListe.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(63, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "labelControl1";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(203, 16);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(63, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "labelControl2";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(324, 2);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(63, 13);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "labelControl3";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "CURID";
            this.gridColumn1.FieldName = "CURID";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Müşteri No";
            this.gridColumn2.FieldName = "CURVAL";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Müşteri Adı";
            this.gridColumn3.FieldName = "CURNAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "SALID";
            this.gridColumn4.FieldName = "SALID";
            this.gridColumn4.Name = "gridColumn4";
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "İşlem Giriş Tarihi";
            this.gridColumn5.FieldName = "FTRDATE";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Satış Tutarı";
            this.gridColumn6.FieldName = "SALAMOUNT";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            // 
            // SatisSec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 397);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.gridListe);
            this.Controls.Add(this.tileBar1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(352, 224);
            this.Name = "SatisSec";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SatisSec";
            this.Load += new System.EventHandler(this.SatisSec_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridListe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewListe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Navigation.TileBar tileBar1;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroup2;
        private DevExpress.XtraBars.Navigation.TileBarItem btnSec;
        private DevExpress.XtraGrid.GridControl gridListe;
        private DevExpress.XtraGrid.Views.Grid.GridView ViewListe;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}