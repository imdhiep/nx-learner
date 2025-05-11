using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing;
using NXOpen;
using NXOpen.UF;

namespace PartList
{


    #region File: Form.Designer.cs
    public partial class MainForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Khai báo các control UI
        private ComboBox cbFilter;
        private ComboBox cbPartNo;
        private ComboBox cbPartName;
        private ComboBox cbQuantity;
        private ComboBox cbSize;
        private ComboBox cbMaterial;
        private ComboBox cbRemarks;
        private Button btnWrite;
        private PictureBox pictureBox;
        private Label lbStatus;
        private TextBox txtImageLink;
        private RadioButton rbtnBodyAssign;
        private RadioButton rbtnScreenShot;
        private RadioButton rbtnWriteXML;
        private RadioButton rbtnRemoveTag;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true nếu được gọi từ Dispose; false nếu được gọi từ finalizer</param>
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
            this.cbFilter = new System.Windows.Forms.ComboBox();
            this.cbPartNo = new System.Windows.Forms.ComboBox();
            this.cbPartName = new System.Windows.Forms.ComboBox();
            this.cbQuantity = new System.Windows.Forms.ComboBox();
            this.cbSize = new System.Windows.Forms.ComboBox();
            this.cbMaterial = new System.Windows.Forms.ComboBox();
            this.cbRemarks = new System.Windows.Forms.ComboBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.txtImageLink = new System.Windows.Forms.TextBox();
            this.rbtnBodyAssign = new System.Windows.Forms.RadioButton();
            this.rbtnScreenShot = new System.Windows.Forms.RadioButton();
            this.rbtnWriteXML = new System.Windows.Forms.RadioButton();
            this.rbtnRemoveTag = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cbFilter
            // 
            this.cbFilter.Location = new System.Drawing.Point(12, 12);
            this.cbFilter.Name = "cbFilter";
            this.cbFilter.Size = new System.Drawing.Size(121, 21);
            this.cbFilter.TabIndex = 0;
            this.cbFilter.Text = "Select Filter";
            this.cbFilter.SelectedIndexChanged += new System.EventHandler(this.cbFilter_SelectedIndexChanged);
            // 
            // cbPartNo
            // 
            this.cbPartNo.Location = new System.Drawing.Point(12, 39);
            this.cbPartNo.Name = "cbPartNo";
            this.cbPartNo.Size = new System.Drawing.Size(121, 21);
            this.cbPartNo.TabIndex = 1;
            this.cbPartNo.Text = "New Part No";
            this.cbPartNo.SelectedIndexChanged += new System.EventHandler(this.cbPartNo_SelectedIndexChanged);
            // 
            // cbPartName
            // 
            this.cbPartName.Items.AddRange(new object[] {
            "Shaft",
            "Pin",
            "Base",
            "Part"});
            this.cbPartName.Location = new System.Drawing.Point(12, 66);
            this.cbPartName.Name = "cbPartName";
            this.cbPartName.Size = new System.Drawing.Size(121, 21);
            this.cbPartName.TabIndex = 2;
            // 
            // cbQuantity
            // 
            this.cbQuantity.Items.AddRange(new object[] {
            "1EA",
            "2EA",
            "3EA",
            "4EA",
            "5EA",
            "6EA",
            "7EA",
            "8EA",
            "9EA",
            "10EA"});
            this.cbQuantity.Location = new System.Drawing.Point(12, 93);
            this.cbQuantity.Name = "cbQuantity";
            this.cbQuantity.Size = new System.Drawing.Size(121, 21);
            this.cbQuantity.TabIndex = 3;
            // 
            // cbSize
            // 
            this.cbSize.Items.AddRange(new object[] {
            "Size"});
            this.cbSize.Location = new System.Drawing.Point(12, 174);
            this.cbSize.Name = "cbSize";
            this.cbSize.Size = new System.Drawing.Size(121, 21);
            this.cbSize.TabIndex = 4;
            // 
            // cbMaterial
            // 
            this.cbMaterial.Items.AddRange(new object[] {
            "S45C",
            "SKD11",
            "SUS304",
            "Al6061",
            "Brass",
            "POM",
            "Acrylic"});
            this.cbMaterial.Location = new System.Drawing.Point(12, 120);
            this.cbMaterial.Name = "cbMaterial";
            this.cbMaterial.Size = new System.Drawing.Size(121, 21);
            this.cbMaterial.TabIndex = 5;
            // 
            // cbRemarks
            // 
            this.cbRemarks.Items.AddRange(new object[] {
            "40HRC ± 2",
            "54HRC ± 2",
            "60HRC ± 2",
            "Ni/Cr Anod",
            "Black Anod",
            "White Anod",
            "Hard Anod"});
            this.cbRemarks.Location = new System.Drawing.Point(12, 147);
            this.cbRemarks.Name = "cbRemarks";
            this.cbRemarks.Size = new System.Drawing.Size(121, 21);
            this.cbRemarks.TabIndex = 6;
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(160, 250);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(121, 24);
            this.btnWrite.TabIndex = 8;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Location = new System.Drawing.Point(150, 40);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(130, 130);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 10;
            this.pictureBox.TabStop = false;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.ForeColor = System.Drawing.Color.DarkRed;
            this.lbStatus.Location = new System.Drawing.Point(139, 15);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 13);
            this.lbStatus.TabIndex = 9;
            // 
            // txtImageLink
            // 
            this.txtImageLink.Location = new System.Drawing.Point(12, 200);
            this.txtImageLink.Multiline = true;
            this.txtImageLink.Name = "txtImageLink";
            this.txtImageLink.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtImageLink.Size = new System.Drawing.Size(121, 75);
            this.txtImageLink.TabIndex = 7;
            this.txtImageLink.TextChanged += new System.EventHandler(this.txtImageLink_TextChanged);
            // 
            // rbtnBodyAssign
            // 
            this.rbtnBodyAssign.Checked = true;
            this.rbtnBodyAssign.Location = new System.Drawing.Point(160, 170);
            this.rbtnBodyAssign.Name = "rbtnBodyAssign";
            this.rbtnBodyAssign.Size = new System.Drawing.Size(100, 17);
            this.rbtnBodyAssign.TabIndex = 12;
            this.rbtnBodyAssign.TabStop = true;
            this.rbtnBodyAssign.Text = "Body Assign";
            // 
            // rbtnScreenShot
            // 
            this.rbtnScreenShot.Location = new System.Drawing.Point(160, 190);
            this.rbtnScreenShot.Name = "rbtnScreenShot";
            this.rbtnScreenShot.Size = new System.Drawing.Size(100, 17);
            this.rbtnScreenShot.TabIndex = 11;
            this.rbtnScreenShot.Text = "Screen Shot";
            // 
            // rbtnWriteXML
            // 
            this.rbtnWriteXML.Location = new System.Drawing.Point(160, 210);
            this.rbtnWriteXML.Name = "rbtnWriteXML";
            this.rbtnWriteXML.Size = new System.Drawing.Size(100, 17);
            this.rbtnWriteXML.TabIndex = 13;
            this.rbtnWriteXML.Text = "Write XML";
            // 
            // rbtnRemoveTag
            // 
            this.rbtnRemoveTag.Location = new System.Drawing.Point(160, 230);
            this.rbtnRemoveTag.Name = "rbtnRemoveTag";
            this.rbtnRemoveTag.Size = new System.Drawing.Size(100, 17);
            this.rbtnRemoveTag.TabIndex = 14;
            this.rbtnRemoveTag.Text = "Remove Tag";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.cbFilter);
            this.Controls.Add(this.cbPartNo);
            this.Controls.Add(this.cbPartName);
            this.Controls.Add(this.cbQuantity);
            this.Controls.Add(this.cbSize);
            this.Controls.Add(this.cbMaterial);
            this.Controls.Add(this.cbRemarks);
            this.Controls.Add(this.txtImageLink);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.rbtnScreenShot);
            this.Controls.Add(this.rbtnBodyAssign);
            this.Controls.Add(this.rbtnWriteXML);
            this.Controls.Add(this.rbtnRemoveTag);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Part List";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }

    #endregion

}


