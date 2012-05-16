namespace ImagePerf
{
    partial class ImgShrink
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
            this.dgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.BtnProcess = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.BtnGenerateReport = new System.Windows.Forms.Button();
            this.BtnGenReport = new System.Windows.Forms.Button();
            this.txtDest = new System.Windows.Forms.TextBox();
            this.lblSrc = new System.Windows.Forms.Label();
            this.lblDest = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dgFileOpen
            // 
            this.dgFileOpen.FileName = "dgFileOpen";
            // 
            // BtnProcess
            // 
            this.BtnProcess.Location = new System.Drawing.Point(34, 273);
            this.BtnProcess.Name = "BtnProcess";
            this.BtnProcess.Size = new System.Drawing.Size(122, 23);
            this.BtnProcess.TabIndex = 1;
            this.BtnProcess.Text = "Process";
            this.BtnProcess.UseVisualStyleBackColor = true;
            this.BtnProcess.Click += new System.EventHandler(this.BtnProcessClick);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(147, 30);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(245, 20);
            this.txtPath.TabIndex = 2;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(13, 179);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 3;
            // 
            // BtnGenerateReport
            // 
            this.BtnGenerateReport.Location = new System.Drawing.Point(179, 273);
            this.BtnGenerateReport.Name = "BtnGenerateReport";
            this.BtnGenerateReport.Size = new System.Drawing.Size(122, 23);
            this.BtnGenerateReport.TabIndex = 4;
            this.BtnGenerateReport.Text = "Generate Report";
            this.BtnGenerateReport.UseVisualStyleBackColor = true;
            this.BtnGenerateReport.Click += new System.EventHandler(this.BtnGenerateReportClick);
            // 
            // BtnGenReport
            // 
            this.BtnGenReport.Location = new System.Drawing.Point(324, 273);
            this.BtnGenReport.Name = "BtnGenReport";
            this.BtnGenReport.Size = new System.Drawing.Size(122, 23);
            this.BtnGenReport.TabIndex = 5;
            this.BtnGenReport.Text = "Generate Byte Report";
            this.BtnGenReport.UseVisualStyleBackColor = true;
            this.BtnGenReport.Click += new System.EventHandler(this.BtnGenReportClick);
            // 
            // txtDest
            // 
            this.txtDest.Location = new System.Drawing.Point(147, 76);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(245, 20);
            this.txtDest.TabIndex = 6;
            // 
            // lblSrc
            // 
            this.lblSrc.AutoSize = true;
            this.lblSrc.Location = new System.Drawing.Point(31, 37);
            this.lblSrc.Name = "lblSrc";
            this.lblSrc.Size = new System.Drawing.Size(86, 13);
            this.lblSrc.TabIndex = 7;
            this.lblSrc.Text = "Source Directory";
            // 
            // lblDest
            // 
            this.lblDest.AutoSize = true;
            this.lblDest.Location = new System.Drawing.Point(31, 78);
            this.lblDest.Name = "lblDest";
            this.lblDest.Size = new System.Drawing.Size(105, 13);
            this.lblDest.TabIndex = 8;
            this.lblDest.Text = "Destination Directory";
            // 
            // ImgShrink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 346);
            this.Controls.Add(this.lblDest);
            this.Controls.Add(this.lblSrc);
            this.Controls.Add(this.txtDest);
            this.Controls.Add(this.BtnGenReport);
            this.Controls.Add(this.BtnGenerateReport);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.BtnProcess);
            this.Name = "ImgShrink";
            this.Text = "ImgShrink";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dgFileOpen;
        private System.Windows.Forms.Button BtnProcess;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button BtnGenerateReport;
        private System.Windows.Forms.Button BtnGenReport;
        private System.Windows.Forms.TextBox txtDest;
        private System.Windows.Forms.Label lblSrc;
        private System.Windows.Forms.Label lblDest;
    }
}

