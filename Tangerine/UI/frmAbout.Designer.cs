namespace Tangerine.UI
{
    partial class frmAbout
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
            this.btnOK = new System.Windows.Forms.Button();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblContributors = new System.Windows.Forms.Label();
            this.lblAndrey = new System.Windows.Forms.Label();
            this.lblDmitriy = new System.Windows.Forms.Label();
            this.lblEvgeny = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblBasedOn = new System.Windows.Forms.Label();
            this.lnkXAPSpy = new System.Windows.Forms.LinkLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(150, 122);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAppName.Location = new System.Drawing.Point(80, 12);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(64, 13);
            this.lblAppName.TabIndex = 1;
            this.lblAppName.Text = "Tangerine";
            // 
            // lblContributors
            // 
            this.lblContributors.AutoSize = true;
            this.lblContributors.Location = new System.Drawing.Point(80, 59);
            this.lblContributors.Name = "lblContributors";
            this.lblContributors.Size = new System.Drawing.Size(66, 13);
            this.lblContributors.TabIndex = 1;
            this.lblContributors.Text = "Contributors:";
            // 
            // lblAndrey
            // 
            this.lblAndrey.AutoSize = true;
            this.lblAndrey.Location = new System.Drawing.Point(97, 72);
            this.lblAndrey.Name = "lblAndrey";
            this.lblAndrey.Size = new System.Drawing.Size(104, 13);
            this.lblAndrey.TabIndex = 1;
            this.lblAndrey.Text = "Andrey Chasovskikh";
            // 
            // lblDmitriy
            // 
            this.lblDmitriy.AutoSize = true;
            this.lblDmitriy.Location = new System.Drawing.Point(97, 85);
            this.lblDmitriy.Name = "lblDmitriy";
            this.lblDmitriy.Size = new System.Drawing.Size(94, 13);
            this.lblDmitriy.TabIndex = 1;
            this.lblDmitriy.Text = "Dmitriy Evdokimov";
            // 
            // lblEvgeny
            // 
            this.lblEvgeny.AutoSize = true;
            this.lblEvgeny.Location = new System.Drawing.Point(97, 98);
            this.lblEvgeny.Name = "lblEvgeny";
            this.lblEvgeny.Size = new System.Drawing.Size(91, 13);
            this.lblEvgeny.TabIndex = 1;
            this.lblEvgeny.Text = "Evgeny Bechkalo";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Tangerine.Properties.Resources._1352152419_ball_1x1;
            this.pictureBox1.Location = new System.Drawing.Point(25, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblBasedOn
            // 
            this.lblBasedOn.AutoSize = true;
            this.lblBasedOn.Location = new System.Drawing.Point(80, 31);
            this.lblBasedOn.Name = "lblBasedOn";
            this.lblBasedOn.Size = new System.Drawing.Size(55, 13);
            this.lblBasedOn.TabIndex = 1;
            this.lblBasedOn.Text = "Based on ";
            // 
            // lnkXAPSpy
            // 
            this.lnkXAPSpy.AutoSize = true;
            this.lnkXAPSpy.Location = new System.Drawing.Point(130, 31);
            this.lnkXAPSpy.Name = "lnkXAPSpy";
            this.lnkXAPSpy.Size = new System.Drawing.Size(46, 13);
            this.lnkXAPSpy.TabIndex = 3;
            this.lnkXAPSpy.TabStop = true;
            this.lnkXAPSpy.Text = "XAPSpy";
            this.lnkXAPSpy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkXAPSpy_LinkClicked);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblVersion.Location = new System.Drawing.Point(141, 12);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(25, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "0.0";
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 157);
            this.Controls.Add(this.lnkXAPSpy);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblEvgeny);
            this.Controls.Add(this.lblDmitriy);
            this.Controls.Add(this.lblAndrey);
            this.Controls.Add(this.lblContributors);
            this.Controls.Add(this.lblBasedOn);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblContributors;
        private System.Windows.Forms.Label lblAndrey;
        private System.Windows.Forms.Label lblDmitriy;
        private System.Windows.Forms.Label lblEvgeny;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblBasedOn;
        private System.Windows.Forms.LinkLabel lnkXAPSpy;
        private System.Windows.Forms.Label lblVersion;
    }
}