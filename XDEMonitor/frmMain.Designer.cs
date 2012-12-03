namespace XDEMonitor
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.labelMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textSearchStr = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(665, 417);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(58, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // labelMsg
            // 
            this.labelMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMsg.AutoSize = true;
            this.labelMsg.Location = new System.Drawing.Point(9, 422);
            this.labelMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(59, 13);
            this.labelMsg.TabIndex = 2;
            this.labelMsg.Text = "API Hook: ";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(499, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Find text:";
            // 
            // textSearchStr
            // 
            this.textSearchStr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchStr.Location = new System.Drawing.Point(552, 7);
            this.textSearchStr.Margin = new System.Windows.Forms.Padding(2);
            this.textSearchStr.Name = "textSearchStr";
            this.textSearchStr.Size = new System.Drawing.Size(110, 20);
            this.textSearchStr.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(666, 7);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 19);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menu.Size = new System.Drawing.Size(731, 24);
            this.menu.TabIndex = 6;
            this.menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.onSaveAs);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.OnExit);
            // 
            // rtbConsole
            // 
            this.rtbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbConsole.BackColor = System.Drawing.SystemColors.Window;
            this.rtbConsole.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbConsole.Location = new System.Drawing.Point(12, 27);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(711, 385);
            this.rtbConsole.TabIndex = 7;
            this.rtbConsole.Text = "";
            this.rtbConsole.WordWrap = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 451);
            this.Controls.Add(this.rtbConsole);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.textSearchStr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.menu);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMain";
            this.Text = "XDE Monitor";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSearchStr;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtbConsole;
    }
}

