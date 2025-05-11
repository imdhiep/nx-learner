namespace FormWriteTabular
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
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.cbDesignNo = new System.Windows.Forms.ComboBox();
            this.cbProjectName = new System.Windows.Forms.ComboBox();
            this.cbTitleName = new System.Windows.Forms.ComboBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbDesignNo
            // 
            this.cbDesignNo.FormattingEnabled = true;
            this.cbDesignNo.Items.AddRange(new object[] {
            "JS KHAC NIEM",
            "JS QUE VO",
            "JS KOREA",
            "SAMSUNG",
            "SCONNECT",
            "M&C",
            "SJ",
            "ALUKO",
            "ACTRO",
            "HALLA"});
            this.cbDesignNo.Location = new System.Drawing.Point(93, 12);
            this.cbDesignNo.Name = "cbDesignNo";
            this.cbDesignNo.Size = new System.Drawing.Size(139, 21);
            this.cbDesignNo.TabIndex = 0;
            // 
            // cbProjectName
            // 
            this.cbProjectName.FormattingEnabled = true;
            this.cbProjectName.Location = new System.Drawing.Point(93, 48);
            this.cbProjectName.Name = "cbProjectName";
            this.cbProjectName.Size = new System.Drawing.Size(139, 21);
            this.cbProjectName.TabIndex = 1;
            // 
            // cbTitleName
            // 
            this.cbTitleName.FormattingEnabled = true;
            this.cbTitleName.Location = new System.Drawing.Point(93, 86);
            this.cbTitleName.Name = "cbTitleName";
            this.cbTitleName.Size = new System.Drawing.Size(139, 21);
            this.cbTitleName.TabIndex = 2;
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnWrite.Location = new System.Drawing.Point(93, 121);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(139, 32);
            this.btnWrite.TabIndex = 3;
            this.btnWrite.Text = "Write All";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Design No.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Project Name.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tittle Name.";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(244, 165);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.cbTitleName);
            this.Controls.Add(this.cbProjectName);
            this.Controls.Add(this.cbDesignNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copyright Dll0202           Write Title";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
