namespace MusicForm
{
    partial class Form1
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnInject = new System.Windows.Forms.Button();
            this.webControl = new Awesomium.Windows.Forms.WebControl(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(302, 680);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(75, 23);
            this.btnInject.TabIndex = 1;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // webControl
            // 
            this.webControl.Location = new System.Drawing.Point(13, 13);
            this.webControl.Size = new System.Drawing.Size(646, 641);
            this.webControl.Source = new System.Uri("http://www.100fm.co.il/jwplayer/jwplayer.aspx", System.UriKind.Absolute);
            this.webControl.TabIndex = 2;
            this.webControl.LoadingFrame += new Awesomium.Core.LoadingFrameEventHandler(this.Awesomium_Windows_Forms_WebControl_LoadingFrame);
            this.webControl.LoadingFrameComplete += new Awesomium.Core.FrameEventHandler(this.Awesomium_Windows_Forms_WebControl_LoadingFrameComplete);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 719);
            this.Controls.Add(this.webControl);
            this.Controls.Add(this.btnInject);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Music";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnInject;
        private Awesomium.Windows.Forms.WebControl webControl;

    }
}

