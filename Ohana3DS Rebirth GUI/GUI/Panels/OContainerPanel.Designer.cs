namespace Ohana3DS_Rebirth.GUI
{
    partial class OContainerPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TopControls = new System.Windows.Forms.TableLayoutPanel();
            this.BtnExport = new Ohana3DS_Rebirth.GUI.OButton();
            this.BtnExportAll = new Ohana3DS_Rebirth.GUI.OButton();
            this.FileList = new Ohana3DS_Rebirth.GUI.OList();
            this.OpenButton = new Ohana3DS_Rebirth.GUI.OButton();
            this.TopControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopControls
            // 
            this.TopControls.ColumnCount = 3;
            this.TopControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TopControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TopControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TopControls.Controls.Add(this.OpenButton, 2, 0);
            this.TopControls.Controls.Add(this.BtnExport, 0, 0);
            this.TopControls.Controls.Add(this.BtnExportAll, 0, 0);
            this.TopControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopControls.Location = new System.Drawing.Point(0, 0);
            this.TopControls.Name = "TopControls";
            this.TopControls.RowCount = 1;
            this.TopControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TopControls.Size = new System.Drawing.Size(447, 30);
            this.TopControls.TabIndex = 7;
            // 
            // BtnExport
            // 
            this.BtnExport.BackColor = System.Drawing.Color.Transparent;
            this.BtnExport.Centered = true;
            this.BtnExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnExport.Image = global::Ohana3DS_Rebirth_GUI.Properties.Resources.ui_icon_upload;
            this.BtnExport.Location = new System.Drawing.Point(150, 2);
            this.BtnExport.Margin = new System.Windows.Forms.Padding(2);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(144, 26);
            this.BtnExport.TabIndex = 7;
            this.BtnExport.Text = "Export";
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // BtnExportAll
            // 
            this.BtnExportAll.BackColor = System.Drawing.Color.Transparent;
            this.BtnExportAll.Centered = true;
            this.BtnExportAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnExportAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnExportAll.Image = global::Ohana3DS_Rebirth_GUI.Properties.Resources.ui_icon_arrowup;
            this.BtnExportAll.Location = new System.Drawing.Point(2, 2);
            this.BtnExportAll.Margin = new System.Windows.Forms.Padding(2);
            this.BtnExportAll.Name = "BtnExportAll";
            this.BtnExportAll.Size = new System.Drawing.Size(144, 26);
            this.BtnExportAll.TabIndex = 6;
            this.BtnExportAll.Text = "Export all";
            this.BtnExportAll.Click += new System.EventHandler(this.BtnExportAll_Click);
            // 
            // FileList
            // 
            this.FileList.BackColor = System.Drawing.Color.Transparent;
            this.FileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileList.HeaderHeight = 24;
            this.FileList.ItemHeight = 24;
            this.FileList.Location = new System.Drawing.Point(0, 30);
            this.FileList.Name = "FileList";
            this.FileList.SelectedIndex = -1;
            this.FileList.Size = new System.Drawing.Size(447, 226);
            this.FileList.TabIndex = 8;
            // 
            // OpenButton
            // 
            this.OpenButton.BackColor = System.Drawing.Color.Transparent;
            this.OpenButton.Centered = true;
            this.OpenButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Image = global::Ohana3DS_Rebirth_GUI.Properties.Resources.ui_icon_folder;
            this.OpenButton.Location = new System.Drawing.Point(298, 2);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(147, 26);
            this.OpenButton.TabIndex = 8;
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new System.EventHandler(this.Open);
            // 
            // OContainerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.Controls.Add(this.FileList);
            this.Controls.Add(this.TopControls);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "OContainerPanel";
            this.Size = new System.Drawing.Size(447, 256);
            this.TopControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TopControls;
        private OButton BtnExport;
        private OButton BtnExportAll;
        private OList FileList;
        private OButton OpenButton;
    }
}
