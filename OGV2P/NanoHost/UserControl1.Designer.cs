namespace NanoHost
{
    partial class UserControl1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl1));
            this.rtmpActiveX = new AxRTMPActiveX.AxRTMPActiveX();
            ((System.ComponentModel.ISupportInitialize)(this.rtmpActiveX)).BeginInit();
            this.SuspendLayout();
            // 
            // rtmpActiveX
            // 
            this.rtmpActiveX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtmpActiveX.Enabled = true;
            this.rtmpActiveX.Location = new System.Drawing.Point(0, 0);
            this.rtmpActiveX.Name = "rtmpActiveX";
            this.rtmpActiveX.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rtmpActiveX.OcxState")));
            this.rtmpActiveX.Size = new System.Drawing.Size(450, 450);
            this.rtmpActiveX.TabIndex = 0;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtmpActiveX);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(450, 450);
            ((System.ComponentModel.ISupportInitialize)(this.rtmpActiveX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxRTMPActiveX.AxRTMPActiveX rtmpActiveX;
    }
}
