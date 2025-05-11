
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;

namespace BasicFormApplication1
{


    #region File: Form.Desinger.cs
    public partial class Form1 : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Khai báo các control
        internal Label lblMessage;
        internal Button btnOn;
        internal Button btnOff;

        /// <summary>
        /// Required method for Designer support - không sửa đổi nội dung bên trong phương thức này bằng trình soạn thảo code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOn = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(5, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(200, 23);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Đang khởi tạo Session...";
            // 
            // btnOn
            // 
            this.btnOn.Location = new System.Drawing.Point(60, 24);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(60, 30);
            this.btnOn.TabIndex = 1;
            this.btnOn.Text = "On";
            this.btnOn.UseVisualStyleBackColor = true;
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnOff
            // 
            this.btnOff.Location = new System.Drawing.Point(145, 24);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(60, 30);
            this.btnOff.TabIndex = 2;
            this.btnOff.Text = "Off";
            this.btnOff.UseVisualStyleBackColor = true;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(250, 66);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.btnOff);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    #endregion




}