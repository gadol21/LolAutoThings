namespace LolThingies
{
    partial class frmGUI
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
            this.btnInject = new System.Windows.Forms.Button();
            this.panelInject = new System.Windows.Forms.Panel();
            this.btnStart = new System.Windows.Forms.Button();
            this.rdbInjectManually = new System.Windows.Forms.RadioButton();
            this.rdbInjcetAuto = new System.Windows.Forms.RadioButton();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSmite = new System.Windows.Forms.Label();
            this.rdbSmiteD = new System.Windows.Forms.RadioButton();
            this.rdbSmiteF = new System.Windows.Forms.RadioButton();
            this.panelInject.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(91, 43);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(75, 23);
            this.btnInject.TabIndex = 0;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // panelInject
            // 
            this.panelInject.Controls.Add(this.btnStart);
            this.panelInject.Controls.Add(this.rdbInjectManually);
            this.panelInject.Controls.Add(this.btnInject);
            this.panelInject.Controls.Add(this.rdbInjcetAuto);
            this.panelInject.Location = new System.Drawing.Point(44, 79);
            this.panelInject.Name = "panelInject";
            this.panelInject.Size = new System.Drawing.Size(200, 100);
            this.panelInject.TabIndex = 1;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(91, 1);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // rdbInjectManually
            // 
            this.rdbInjectManually.AutoSize = true;
            this.rdbInjectManually.Location = new System.Drawing.Point(4, 46);
            this.rdbInjectManually.Name = "rdbInjectManually";
            this.rdbInjectManually.Size = new System.Drawing.Size(67, 17);
            this.rdbInjectManually.TabIndex = 1;
            this.rdbInjectManually.TabStop = true;
            this.rdbInjectManually.Text = "Manually";
            this.rdbInjectManually.UseVisualStyleBackColor = false;
            this.rdbInjectManually.CheckedChanged += new System.EventHandler(this.rdbInjectManually_CheckedChanged);
            // 
            // rdbInjcetAuto
            // 
            this.rdbInjcetAuto.AutoSize = true;
            this.rdbInjcetAuto.Checked = true;
            this.rdbInjcetAuto.Location = new System.Drawing.Point(4, 4);
            this.rdbInjcetAuto.Name = "rdbInjcetAuto";
            this.rdbInjcetAuto.Size = new System.Drawing.Size(72, 17);
            this.rdbInjcetAuto.TabIndex = 0;
            this.rdbInjcetAuto.TabStop = true;
            this.rdbInjcetAuto.Text = "Automatic";
            this.rdbInjcetAuto.UseVisualStyleBackColor = true;
            this.rdbInjcetAuto.CheckedChanged += new System.EventHandler(this.rdbInjcetAuto_CheckedChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(253, 31);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status: Not Injected";
            // 
            // lblSmite
            // 
            this.lblSmite.AutoSize = true;
            this.lblSmite.Location = new System.Drawing.Point(48, 206);
            this.lblSmite.Name = "lblSmite";
            this.lblSmite.Size = new System.Drawing.Size(57, 13);
            this.lblSmite.TabIndex = 3;
            this.lblSmite.Text = "Smite Key:";
            // 
            // rdbSmiteD
            // 
            this.rdbSmiteD.AutoSize = true;
            this.rdbSmiteD.Location = new System.Drawing.Point(112, 206);
            this.rdbSmiteD.Name = "rdbSmiteD";
            this.rdbSmiteD.Size = new System.Drawing.Size(33, 17);
            this.rdbSmiteD.TabIndex = 4;
            this.rdbSmiteD.Text = "D";
            this.rdbSmiteD.UseVisualStyleBackColor = true;
            this.rdbSmiteD.CheckedChanged += new System.EventHandler(this.rdbSmiteD_CheckedChanged);
            // 
            // rdbSmiteF
            // 
            this.rdbSmiteF.AutoSize = true;
            this.rdbSmiteF.Checked = true;
            this.rdbSmiteF.Location = new System.Drawing.Point(151, 206);
            this.rdbSmiteF.Name = "rdbSmiteF";
            this.rdbSmiteF.Size = new System.Drawing.Size(31, 17);
            this.rdbSmiteF.TabIndex = 5;
            this.rdbSmiteF.TabStop = true;
            this.rdbSmiteF.Text = "F";
            this.rdbSmiteF.UseVisualStyleBackColor = true;
            this.rdbSmiteF.CheckedChanged += new System.EventHandler(this.rdbSmiteF_CheckedChanged);
            // 
            // frmGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.rdbSmiteF);
            this.Controls.Add(this.rdbSmiteD);
            this.Controls.Add(this.lblSmite);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.panelInject);
            this.Name = "frmGUI";
            this.Text = "LOL Thingies";
            this.panelInject.ResumeLayout(false);
            this.panelInject.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.Panel panelInject;
        private System.Windows.Forms.RadioButton rdbInjectManually;
        private System.Windows.Forms.RadioButton rdbInjcetAuto;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSmite;
        private System.Windows.Forms.RadioButton rdbSmiteD;
        private System.Windows.Forms.RadioButton rdbSmiteF;
    }
}

