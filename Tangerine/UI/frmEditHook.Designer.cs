namespace Tangerine.UI
{
    partial class frmEditHook
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditHook));
            this.chbRunCode = new System.Windows.Forms.CheckBox();
            this.rbReplace = new System.Windows.Forms.RadioButton();
            this.rbMethodEnter = new System.Windows.Forms.RadioButton();
            this.chbLogParameters = new System.Windows.Forms.CheckBox();
            this.chbLogMethods = new System.Windows.Forms.CheckBox();
            this.lblGlobalHooks = new System.Windows.Forms.Label();
            this.rtbCode = new System.Windows.Forms.RichTextBox();
            this.lblCustomCode = new System.Windows.Forms.Label();
            this.lblMethod = new System.Windows.Forms.Label();
            this.lblBegin = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.chbLogReturnValues = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbMethodExit = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // chbRunCode
            // 
            this.chbRunCode.AutoSize = true;
            this.chbRunCode.Location = new System.Drawing.Point(186, 9);
            this.chbRunCode.Name = "chbRunCode";
            this.chbRunCode.Size = new System.Drawing.Size(110, 17);
            this.chbRunCode.TabIndex = 2;
            this.chbRunCode.Text = "Run custom code";
            this.chbRunCode.UseVisualStyleBackColor = true;
            this.chbRunCode.CheckedChanged += new System.EventHandler(this.chbRunCode_CheckedChanged);
            // 
            // rbReplace
            // 
            this.rbReplace.AutoSize = true;
            this.rbReplace.Enabled = false;
            this.rbReplace.Location = new System.Drawing.Point(207, 36);
            this.rbReplace.Name = "rbReplace";
            this.rbReplace.Size = new System.Drawing.Size(133, 17);
            this.rbReplace.TabIndex = 3;
            this.rbReplace.TabStop = true;
            this.rbReplace.Text = "Replace target method";
            this.rbReplace.UseVisualStyleBackColor = true;
            // 
            // rbMethodEnter
            // 
            this.rbMethodEnter.AutoSize = true;
            this.rbMethodEnter.Enabled = false;
            this.rbMethodEnter.Location = new System.Drawing.Point(207, 59);
            this.rbMethodEnter.Name = "rbMethodEnter";
            this.rbMethodEnter.Size = new System.Drawing.Size(104, 17);
            this.rbMethodEnter.TabIndex = 4;
            this.rbMethodEnter.TabStop = true;
            this.rbMethodEnter.Text = "On method enter";
            this.rbMethodEnter.UseVisualStyleBackColor = true;
            // 
            // chbLogParameters
            // 
            this.chbLogParameters.AutoSize = true;
            this.chbLogParameters.Checked = true;
            this.chbLogParameters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbLogParameters.Location = new System.Drawing.Point(12, 59);
            this.chbLogParameters.Name = "chbLogParameters";
            this.chbLogParameters.Size = new System.Drawing.Size(128, 17);
            this.chbLogParameters.TabIndex = 27;
            this.chbLogParameters.Text = "Log parameter values";
            this.chbLogParameters.UseVisualStyleBackColor = true;
            // 
            // chbLogMethods
            // 
            this.chbLogMethods.AutoSize = true;
            this.chbLogMethods.Checked = true;
            this.chbLogMethods.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbLogMethods.Location = new System.Drawing.Point(12, 36);
            this.chbLogMethods.Name = "chbLogMethods";
            this.chbLogMethods.Size = new System.Drawing.Size(116, 17);
            this.chbLogMethods.TabIndex = 26;
            this.chbLogMethods.Text = "Log method names";
            this.chbLogMethods.UseVisualStyleBackColor = true;
            this.chbLogMethods.CheckedChanged += new System.EventHandler(this.chbLogMethods_CheckedChanged);
            // 
            // lblGlobalHooks
            // 
            this.lblGlobalHooks.AutoSize = true;
            this.lblGlobalHooks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGlobalHooks.Location = new System.Drawing.Point(9, 10);
            this.lblGlobalHooks.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGlobalHooks.Name = "lblGlobalHooks";
            this.lblGlobalHooks.Size = new System.Drawing.Size(110, 13);
            this.lblGlobalHooks.TabIndex = 25;
            this.lblGlobalHooks.Text = "Override global hooks";
            // 
            // rtbCode
            // 
            this.rtbCode.AcceptsTab = true;
            this.rtbCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCode.Enabled = false;
            this.rtbCode.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbCode.Location = new System.Drawing.Point(31, 162);
            this.rtbCode.Margin = new System.Windows.Forms.Padding(3, 30, 3, 3);
            this.rtbCode.Name = "rtbCode";
            this.rtbCode.Size = new System.Drawing.Size(641, 259);
            this.rtbCode.TabIndex = 28;
            this.rtbCode.Text = "";
            this.rtbCode.WordWrap = false;
            // 
            // lblCustomCode
            // 
            this.lblCustomCode.AutoSize = true;
            this.lblCustomCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomCode.Location = new System.Drawing.Point(9, 110);
            this.lblCustomCode.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomCode.Name = "lblCustomCode";
            this.lblCustomCode.Size = new System.Drawing.Size(69, 13);
            this.lblCustomCode.TabIndex = 25;
            this.lblCustomCode.Text = "Custom code";
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMethod.Location = new System.Drawing.Point(9, 132);
            this.lblMethod.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(42, 13);
            this.lblMethod.TabIndex = 25;
            this.lblMethod.Text = "method";
            // 
            // lblBegin
            // 
            this.lblBegin.AutoSize = true;
            this.lblBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBegin.Location = new System.Drawing.Point(9, 147);
            this.lblBegin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBegin.Name = "lblBegin";
            this.lblBegin.Size = new System.Drawing.Size(11, 13);
            this.lblBegin.TabIndex = 25;
            this.lblBegin.Text = "{";
            // 
            // lblEnd
            // 
            this.lblEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEnd.AutoSize = true;
            this.lblEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnd.Location = new System.Drawing.Point(9, 427);
            this.lblEnd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(11, 13);
            this.lblEnd.TabIndex = 25;
            this.lblEnd.Text = "}";
            // 
            // chbLogReturnValues
            // 
            this.chbLogReturnValues.AutoSize = true;
            this.chbLogReturnValues.Checked = true;
            this.chbLogReturnValues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbLogReturnValues.Location = new System.Drawing.Point(12, 82);
            this.chbLogReturnValues.Name = "chbLogReturnValues";
            this.chbLogReturnValues.Size = new System.Drawing.Size(108, 17);
            this.chbLogReturnValues.TabIndex = 27;
            this.chbLogReturnValues.Text = "Log return values";
            this.chbLogReturnValues.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(597, 427);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 29;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(516, 427);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 29;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // rbMethodExit
            // 
            this.rbMethodExit.AutoSize = true;
            this.rbMethodExit.Enabled = false;
            this.rbMethodExit.Location = new System.Drawing.Point(207, 82);
            this.rbMethodExit.Name = "rbMethodExit";
            this.rbMethodExit.Size = new System.Drawing.Size(96, 17);
            this.rbMethodExit.TabIndex = 4;
            this.rbMethodExit.TabStop = true;
            this.rbMethodExit.Text = "On method exit";
            this.rbMethodExit.UseVisualStyleBackColor = true;
            // 
            // frmEditHook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 462);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rtbCode);
            this.Controls.Add(this.chbLogReturnValues);
            this.Controls.Add(this.chbLogParameters);
            this.Controls.Add(this.chbLogMethods);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.lblBegin);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.lblCustomCode);
            this.Controls.Add(this.lblGlobalHooks);
            this.Controls.Add(this.rbMethodExit);
            this.Controls.Add(this.rbMethodEnter);
            this.Controls.Add(this.rbReplace);
            this.Controls.Add(this.chbRunCode);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditHook";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit hook for ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEditHook_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbRunCode;
        private System.Windows.Forms.RadioButton rbReplace;
        private System.Windows.Forms.RadioButton rbMethodEnter;
        private System.Windows.Forms.CheckBox chbLogParameters;
        private System.Windows.Forms.CheckBox chbLogMethods;
        private System.Windows.Forms.Label lblGlobalHooks;
        private System.Windows.Forms.RichTextBox rtbCode;
        private System.Windows.Forms.Label lblCustomCode;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.Label lblBegin;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.CheckBox chbLogReturnValues;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbMethodExit;
    }
}