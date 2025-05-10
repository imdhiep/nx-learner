

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.IO;
using System.Reflection;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using System.Diagnostics;
using System.Collections.Generic;
using tag_t = System.UInt32;

#region Entry
namespace Dll0202App
{
    /// <summary>
    /// Journal single code: 
    /// 1. Run(Session session) => Main(string[] args)
    /// 2. Turn on: Session session = Session.GetSession(); 
    /// </summary>
    public static class Entry
    {
        public static void Main(string[] args)
        //public static void Run(Session session)
        {

            try
            {
                Session session = Session.GetSession();
                Core.SessionManager.Init(session);
                Core.Logging.Enable(true); // ✅ Logging On/OFF

                //Tools.BodyTools.GetAttributesOfSelectionBodies("user");
                // Tools.BodyTools.SixFaces();
                //Tools.FaceTools.GetDataRowOfSelectionFaces();

                //✅ Form run
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Dll0202App.Shell.MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Plugin error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static int GetUnloadOption(string dummy)
        {
            return (int)Session.LibraryUnloadOption.Immediately;
        }
    }
}
#endregion

#region Core
namespace Dll0202App.Core
{
    public static class SessionManager
    {
        public static Session Session { get; private set; }
        public static UI UI { get; private set; }
        public static UFSession UFSession { get; private set; }
        public static Part WorkPart { get; private set; }

        public static void Init(Session session)
        {
            Session = session;
            UI = UI.GetUI();
            UFSession = UFSession.GetUFSession();
            WorkPart = session.Parts.Work;
        }
    }
    public static class Logging
    {
        private static bool _enabled = true;

        public static void Enable(bool enable)
        {
            _enabled = enable;
        }

        public static void MessageBox(string message)
        {
            if (_enabled)
                System.Windows.Forms.MessageBox.Show(message, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Listing(string message)
        {
            if (!_enabled || SessionManager.Session == null) return;

            ListingWindow lw = SessionManager.Session.ListingWindow;
            if (!lw.IsOpen) lw.Open();
            lw.WriteLine(message);
        }

        public static void NXMessager(string message)
        {
            if (_enabled && SessionManager.UI != null)
                SessionManager.UI.NXMessageBox.Show("Notice", NXMessageBox.DialogType.Information, message);
        }

        public static void Error(string message)
        {
            if (_enabled)
                System.Windows.Forms.MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
#endregion

#region Shell
namespace Dll0202App.Shell
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
        }

        private void button1Send_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void textBox1Chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string input = textBox1Chat.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            //var form = this; 
            //FormController.CollapseRichTextBox(form);

            var handler = new Dll0202App.Commands.ChatCommandHandler();

            AppendColoredText("User : " + input); // Sir/User input

            string response = handler.Handle(input);

            this.Show();
            this.TopMost = true;

            if (!string.IsNullOrEmpty(response))
            {
                AppendColoredText("Rose: " + response); // Bot phản hồi
            }

            richTextBox1Chat.SelectionStart = richTextBox1Chat.Text.Length;
            richTextBox1Chat.ScrollToCaret();
            textBox1Chat.Clear();
        }

        private void AppendColoredText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            // ✅ Ghi mồi 1 dòng trống nếu là lần đầu tiên
            if (richTextBox1Chat.TextLength == 0)
            {
                richTextBox1Chat.AppendText(" "); // mồi kích hoạt format
                richTextBox1Chat.SelectionStart = richTextBox1Chat.TextLength;
            }

            richTextBox1Chat.SelectionStart = richTextBox1Chat.TextLength;
            richTextBox1Chat.SelectionLength = 0;

            Font currentFont = richTextBox1Chat.Font;

            if (text.StartsWith("User:") || text.StartsWith("//"))
            {
                richTextBox1Chat.SelectionColor = Color.White;
                richTextBox1Chat.SelectionFont = new Font(currentFont, FontStyle.Regular);
            }
            else if (text.StartsWith("Rose:"))
            {
                richTextBox1Chat.SelectionColor = Color.DarkCyan;
                richTextBox1Chat.SelectionFont = new Font(currentFont.FontFamily, 7.5f, FontStyle.Italic);

            }
            else
            {
                richTextBox1Chat.SelectionColor = richTextBox1Chat.ForeColor;
                richTextBox1Chat.SelectionFont = new Font(currentFont, FontStyle.Regular);
            }

            richTextBox1Chat.AppendText(text + Environment.NewLine);

            // reset lại font cho các dòng sau
            richTextBox1Chat.SelectionFont = currentFont;
            richTextBox1Chat.SelectionColor = richTextBox1Chat.ForeColor;
        }

        public void ShowBotMessage(string msg)
        {
            AppendColoredText("Rose: " + msg);  // ✅ Đúng
        }

    }

    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.textBox1Chat = new System.Windows.Forms.TextBox();
            this.button1Send = new System.Windows.Forms.Button();
            this.richTextBox1Chat = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // textBox1Chat
            this.textBox1Chat.BackColor = System.Drawing.Color.White;
            this.textBox1Chat.Location = new System.Drawing.Point(5, 467);
            this.textBox1Chat.Multiline = true;
            this.textBox1Chat.Name = "textBox1Chat";
            this.textBox1Chat.Size = new System.Drawing.Size(620, 40);
            this.textBox1Chat.TabIndex = 0;
            this.textBox1Chat.Text = "Ask me anything 💬";
            this.textBox1Chat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1Chat_KeyDown);

            // richTextBox1Chat
            this.richTextBox1Chat.BackColor = System.Drawing.Color.White;
            this.richTextBox1Chat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1Chat.Location = new System.Drawing.Point(5, 5);
            this.richTextBox1Chat.Name = "richTextBox1Chat";
            this.richTextBox1Chat.Size = new System.Drawing.Size(675, 460);
            this.richTextBox1Chat.TabIndex = 3;
            this.richTextBox1Chat.Text = "";
            this.richTextBox1Chat.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.richTextBox1Chat.ReadOnly = true;

            // button1Send
            this.button1Send.Location = new System.Drawing.Point(634, 467);
            this.button1Send.Name = "button1Send";
            this.button1Send.Size = new System.Drawing.Size(40, 40);
            this.button1Send.TabIndex = 1;
            this.button1Send.Text = ">>>";
            this.button1Send.UseVisualStyleBackColor = true;
            this.button1Send.Click += new System.EventHandler(this.button1Send_Click);

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 511);
            this.Controls.Add(this.richTextBox1Chat);
            this.Controls.Add(this.button1Send);
            this.Controls.Add(this.textBox1Chat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "Dll0202                                                                            ♡VN✩ 30/4/1975 -30/4/2025";
            this.ResumeLayout(false);
            this.PerformLayout();

            // panelMain
            this.panelMain = new Panel();
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Size = new System.Drawing.Size(684, 400);
            this.panelMain.BackColor = Color.White;
            this.panelMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(this.panelMain);

        }

        private System.Windows.Forms.TextBox textBox1Chat;
        private System.Windows.Forms.Button button1Send;
        private System.Windows.Forms.RichTextBox richTextBox1Chat;
        private System.Windows.Forms.Panel panelMain;
        public bool IsGridLoaded()
        {
            foreach (Control ctrl in this.panelMain.Controls)
            {
                if (ctrl is UserControl1)
                    return true;
            }
            return false;
        }

    }

    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public void LoadCsvResult(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("❌ File không tồn tại.");
                return;
            }

            var lines = File.ReadAllLines(path);
            if (lines.Length < 2) return;

            dataGridView1.Rows.Clear(); // Xoá dòng cũ nếu có

            for (int i = 1; i < lines.Length; i++) // Bỏ qua dòng header
            {
                var values = lines[i].Split(',');

                var row = new List<object>();
                row.Add(false); // Checkbox đầu dòng

                foreach (string s in values)
                {
                    row.Add(s.Trim()); // Thêm từng giá trị vào
                }

                dataGridView1.Rows.Add(row.ToArray());
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1) return; // Chỉ xử lý khi click vào cột ID

            string id = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();
            Dll0202App.Tools.BodyTagResolver.HighlightByID(id);
        }


    }

    partial class UserControl1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private List<NXOpen.Tag> _currentlyHighlightedTags = new List<NXOpen.Tag>();

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0) // cột checkbox
            {
                // 1. Unhighlight tất cả tag cũ
                NXOpen.UF.UFSession uf = Dll0202App.Core.SessionManager.UFSession;
                foreach (NXOpen.Tag t in _currentlyHighlightedTags)
                {
                    if (t != NXOpen.Tag.Null)
                        uf.Disp.SetHighlight(t, 0); // tắt highlight
                }

                _currentlyHighlightedTags.Clear();

                // 2. Duyệt toàn bộ dòng để lấy tag từ các dòng được check
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    var row = dataGridView1.Rows[i];
                    bool isChecked = Convert.ToBoolean(row.Cells[0].Value);

                    if (isChecked)
                    {
                        string id = row.Cells[1].Value.ToString().Trim();
                        NXOpen.Tag tag = Dll0202App.Tools.BodyTagResolver.GetTagFromID(id);
                        if (tag != NXOpen.Tag.Null)
                        {
                            uf.Disp.SetHighlight(tag, 1);
                            _currentlyHighlightedTags.Add(tag);
                        }
                    }
                }
            }
        }


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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Collum1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            // this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Collum1,
            this.Column3,
            this.Column4,
            this.Column2,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            //this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13});

            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(673, 320);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            this.dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;



            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.button1.Location = new System.Drawing.Point(345, 325);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 1;
            this.button1.Text = "△";
            //this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.button2.Location = new System.Drawing.Point(315, 325);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 25);
            this.button2.TabIndex = 2;
            this.button2.Text = "▽";
            // this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Collum1
            // 
            this.Collum1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Collum1.FillWeight = 20F;
            this.Collum1.HeaderText = "";
            this.Collum1.Name = "Collum1";
            this.Collum1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Collum1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Collum1.Width = 25;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "ID";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "Cluster";
            this.Column4.Name = "Column4";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Group";
            this.Column2.Name = "Column2";
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.HeaderText = "PartNo";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.HeaderText = "Name";
            this.Column6.Name = "Column6";
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.HeaderText = "Type";
            this.Column7.Name = "Column7";
            // 
            // Column8
            // 
            this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column8.HeaderText = "Revision";
            this.Column8.Name = "Column8";
            // 
            // Column9
            // 
            //this.Column9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //this.Column9.HeaderText = "Quantity";
            //this.Column9.Name = "Column9";
            // 
            // Column10
            // 
            this.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column10.HeaderText = "Material";
            this.Column10.Name = "Column10";
            // 
            // Column11
            // 
            this.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column11.HeaderText = "Size";
            this.Column11.Name = "Column11";
            // 
            // Column12
            // 
            this.Column12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column12.HeaderText = "Mass";
            this.Column12.Name = "Column12";
            // 
            // Column13
            // 
            this.Column13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column13.HeaderText = "Remarks";
            this.Column13.Name = "Column13";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(673, 350);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Collum1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        //private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
    }

    public static class FormController
    {
        private static Dll0202App.Shell.UserControl1 uc1;

        public static void ShowUserControl1()
        {
            var form = System.Windows.Forms.Application.OpenForms["MainForm"] as System.Windows.Forms.Form;
            if (form == null) return;

            if (uc1 == null)
            {
                uc1 = new Dll0202App.Shell.UserControl1();
                uc1.Name = "uc1";
                uc1.Location = new System.Drawing.Point(5, 5);
                uc1.Size = new System.Drawing.Size(673, 350);
                form.Controls.Add(uc1);
            }

            uc1.BringToFront();
            uc1.Visible = true;

            CollapseRichTextBox(form);
        }

        public static void RemoveUserControl1()
        {
            var form = System.Windows.Forms.Application.OpenForms["MainForm"] as System.Windows.Forms.Form;
            if (form == null) return;

            if (uc1 != null)
            {
                form.Controls.Remove(uc1);
                uc1.Dispose();
                uc1 = null;
            }
            else
            {
                var found = form.Controls["uc1"] as Dll0202App.Shell.UserControl1;
                if (found != null)
                {
                    form.Controls.Remove(found);
                    found.Dispose();
                }
            }

            RestoreRichTextBox(form);
        }

        public static void CollapseRichTextBox(System.Windows.Forms.Form form)
        {
            var rtb = form.Controls["richTextBox1Chat"] as System.Windows.Forms.RichTextBox;
            if (rtb == null) return;

            rtb.Location = new System.Drawing.Point(5, 360);
            rtb.Size = new System.Drawing.Size(670, 100);
            //rtb.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            //rtb.ForeColor = Color.Gainsboro;   // hoặc Color.Gainsboro cho nhẹ

            if (rtb.TextLength == 0)
            {
                rtb.Font = new Font("Segoe UI Emoji", 7.0f, FontStyle.Regular);
            }
        }

        public static void RestoreRichTextBox(System.Windows.Forms.Form form)
        {
            var rtb = form.Controls["richTextBox1Chat"] as System.Windows.Forms.RichTextBox;
            if (rtb == null) return;

            rtb.Location = new System.Drawing.Point(5, 5);
            rtb.Size = new System.Drawing.Size(673, 467);
        }

        public static void LoadCsvToPartList(string csvPath)
        {
            var form = System.Windows.Forms.Application.OpenForms["MainForm"] as System.Windows.Forms.Form;
            if (form == null) return;

            if (uc1 == null)
            {
                uc1 = new Dll0202App.Shell.UserControl1();
                uc1.Name = "uc1";
                uc1.Location = new System.Drawing.Point(5, 5);
                uc1.Size = new System.Drawing.Size(673, 350);
                form.Controls.Add(uc1);
                CollapseRichTextBox(form);
            }

            uc1.BringToFront();
            uc1.Visible = true;

            if (!System.IO.File.Exists(csvPath))
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không tìm thấy file CSV: " + csvPath);
                return;
            }

            uc1.LoadCsvResult(csvPath);

            var chatBox = form.Controls["textBox1Chat"] as System.Windows.Forms.TextBox;
            if (chatBox != null)
            {
                chatBox.Focus();  // Trả lại focus cho ô nhập lệnh
            }

        }
    }

    public class ChatLogger
    {
        private StringBuilder _buffer = new StringBuilder();
        private Color _color = Color.MediumVioletRed;

        public void Begin()
        {
            _buffer.Clear();
        }

        public void AddLine(string text)
        {
            _buffer.AppendLine(text);
        }

        public void FlushToRichTextBox(RichTextBox richTextBox)
        {
            if (_buffer.Length > 0)
            {
                richTextBox.SelectionStart = richTextBox.TextLength;
                richTextBox.SelectionLength = 0;
                richTextBox.SelectionColor = _color;
                richTextBox.AppendText(_buffer.ToString());
                richTextBox.SelectionColor = richTextBox.ForeColor;
                _buffer.Clear();
            }
        }
    }

    public class CsvProgressWatcherForm : Form
    {
        private Label lblTime;
        private Label lblStatus;
        private Timer _uiTimer;
        private Stopwatch _watch;
        private string _csvPath;
        private Func<bool> _isGridLoadedChecker;

        public CsvProgressWatcherForm(string csvPath, Func<bool> isGridLoaded)
        {
            _csvPath = csvPath;
            _isGridLoadedChecker = isGridLoaded;

            InitializeComponent();

            _watch = Stopwatch.StartNew();

            _uiTimer = new Timer();
            _uiTimer.Interval = 100; // 100 ms = 0.1s
            _uiTimer.Tick += UiTimer_Tick;
            _uiTimer.Start();
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            double sec = _watch.Elapsed.TotalMilliseconds / 1000.0;
            lblTime.Text = "⏱ " + sec.ToString("0.00") + " sec";

            if (File.Exists(_csvPath))
            {
                lblStatus.Text = "📄 CSV created.";
            }

            if (_isGridLoadedChecker != null && _isGridLoadedChecker())
            {
                lblStatus.Text = "✅ Grid loaded.";
                _uiTimer.Stop();
                System.Threading.Thread.Sleep(500);
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.lblTime = new Label();
            this.lblStatus = new Label();

            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTime.Location = new System.Drawing.Point(10, 10);
            this.lblTime.Size = new System.Drawing.Size(220, 30);
            this.lblTime.Text = "⏱ 0.00 sec";

            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(10, 45);
            this.lblStatus.Size = new System.Drawing.Size(220, 25);
            this.lblStatus.Text = "🔄 Progressing...";

            // 
            // Form
            // 
            this.ClientSize = new System.Drawing.Size(240, 85);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Tracking progress...";
            this.TopMost = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
    }


}
#endregion


#region Commands
namespace Dll0202App.Commands
{

    public class ChatCommandHandler
    {
        private readonly IO.JsonRuleProcessor processor;
        private readonly Dictionary<string, Action> commandHandlers;

        public ChatCommandHandler()
        {
            processor = new IO.JsonRuleProcessor();

            commandHandlers = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                //Các lệnh check
                { "set_color", () => Tools.BodyTools.SetColorForSelectionBodies() },
                { "set_name", () => Tools.BodyTools.SetNameToSelectionBodies("Test Name") },
                { "set_attr", () => Tools.BodyTools.SetAttributesForSelectionBodies("BodyInfo", "Base", "S45C", "58HRC") },
                { "get_attr", () => Tools.BodyTools.GetAttributesOfSelectionBodies("BodyInfo") },
                { "get_faces", () => Tools.BodyTools.GetFacesOfSelectionBodies() },
                { "get_edges", () => Tools.BodyTools.GetEdgesOfSelectionBodies() },
                { "body_check", () => Tools.BodyTools.GetTypeSolidOrSheetOfSelectionBodies() },
                { "get_display", () => Tools.BodyTools.GetDisplayPropOfSelectionBodies() },
                { "get_tags", () => Tools.BodyTools.GetTagsOfSelectionBodies() },
                { "get_name", () => Tools.BodyTools.GetNameOfSelectionBodies() },
                { "get_mass", () => Tools.BodyTools.GetMassProp3DOfSelectionBodies() },
                { "get_bounding", () => Tools.BodyTools.GetBoundingBoxOfSelectionBody() },
                { "get_tag_of_faces", () => Tools.FaceTools.GetTagOfSelectionFaces() },
                { "get_face_data", () => Tools.FaceTools.GetDataRowOfSelectionFaces() },
                { "get_edges_of_faces", () => Tools.FaceTools.GetEdgePropertiesOfSelectionFaces() },
                { "get_loops", () => Tools.FaceTools.GetLoopsOfSelectionFaces() },
                { "get_bounding_rectangle", () => Tools.FaceTools.GetBoundingRectangleOfSelectionFace() },
                { "sample_edges_of_faces", () => Dll0202App.Tools.FaceTools.GetSamplingAllEdgesOfSelectionFace() },
                { "sample_uv_faces", () => Dll0202App.Tools.FaceTools.GetSamplingGridUVOfSelectionFaces() },
                { "part_list", () => Dll0202App.Shell.FormController.ShowUserControl1() },
                { "close_part_list", () => Dll0202App.Shell.FormController.RemoveUserControl1() },
                //{ "load_csv", () => Dll0202App.Shell.FormController.LoadCsvToPartList() },
                { "write_system_id", () => Tools.BodyTools.WriteSystemIDToAllBodies() },
                { "clear_system_id", () => Tools.BodyTools.ClearSystemIDToSelectionBodies() },
                { "create_csys_face_z", () => Tools.FaceTools.CreatCsysAtFacesOfSelectionBody() },
                { "create_axis_face_normal", () => Tools.FaceTools.CreatNormalVectorAtCenterOfSelectionFaces() },
                { "create_bounding_line", () => Tools.FaceTools.CreatBouding2PointCurveOfSelectionFaces() },
                { "create_farest_plane_body", () => Tools.BodyTools.CreateFarestPlaneInDirectionOfSelectionBody() },
                { "filter_faces_orthogonal", () => Tools.BodyTools.CreateBoundingPlanesAndCsysOfSelectionBody() },
                { "create_bounding_planes_corrected_csys", () => Tools.BodyTools.CreateBodyCsysbyMainFaceofSelectionBody() },
                { "check_shaft", () => Tools.BodyTools.CheckShaftTypeOfSelectionBodies() },
                { "move_to_origin", () => Dll0202App.Tools.BodyTools.MoveToOriginSelectionBodies() },
                { "write_body_info", () => Dll0202App.Tools.BodyTools.WriteInfomationOfSeletionBody() },

                //
                 { "AI_write_system_id", () => Dll0202App.AI.DataIO.WriteSystemIDToAllBodies() }


            };
        }

        public string Handle(string input)
        {
            var rule = processor.GetMatchedRule(input);
            if (rule == null)
                return "❓ Command not recognized.";

            if (!string.IsNullOrEmpty(rule.command))
            {
                Action action;
                if (commandHandlers.TryGetValue(rule.command, out action))
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                        // ❌ Nếu lỗi action cũng kệ, không báo lỗi gì.
                    }
                }
            }

            // ✅ Sau khi action xong, nếu có response thì dùng response.
            // ✅ Nếu không có response thì luôn trả mặc định OK
            return !string.IsNullOrEmpty(rule.response)
                ? rule.response
                : "";
        }



    }

}
#endregion

#region AI
namespace Dll0202App.AI
{
    public static class DataIO
    {

        public static void WriteSystemIDToAllBodies()
        {
            string projectId = GetProjectID();
            string csvPath = Path.Combine(Path.GetTempPath(), projectId + ".csv");

            // Tạo và hiển thị form đếm thời gian
            Func<bool> isGridLoaded = () =>
            {
                var mainForm = Dll0202App.Shell.MainForm.Instance;
                if (mainForm == null) return false;

                foreach (Control ctrl in mainForm.Controls)
                {
                    if (ctrl is Dll0202App.Shell.UserControl1)
                        return true;
                }
                return false;
            };

            var watcher = new Dll0202App.Shell.CsvProgressWatcherForm(csvPath, isGridLoaded);
            watcher.Show();
            Application.DoEvents(); // Cho form render ngay

            // Chạy xử lý nền
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                try
                {
                    // Gán ID và xuất CSV: chạy nền
                    AssignSystemIDToAllBodies();
                    ExportBodyInfoToResultCSV(projectId);

                    // Nạp grib: phải gọi trên UI thread
                    if (Dll0202App.Shell.MainForm.Instance != null)
                    {
                        Dll0202App.Shell.MainForm.Instance.Invoke(new MethodInvoker(delegate
                        {
                            LoadResultCsvToGrib(projectId);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    if (Dll0202App.Shell.MainForm.Instance != null)
                    {
                        Dll0202App.Shell.MainForm.Instance.Invoke(new MethodInvoker(delegate
                        {
                            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ " + ex.Message);
                        }));
                    }
                }
            });

            t.IsBackground = true;
            t.Start();
        }


        private static string GetProjectID()
        {
            var part = Dll0202App.Core.SessionManager.WorkPart;
            string value;
            bool found;

            try
            {
                Dll0202App.Core.SessionManager.UFSession.Attr.GetStringUserAttribute(part.Tag, "info", 0, out value, out found);
                if (found && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            return "IP000000"; // fallback nếu không có
        }

        //public static void AssignSystemIDToAllBodies()
        //{
        //    var mainForm = Dll0202App.Shell.MainForm.Instance;
        //    var session = Dll0202App.Core.SessionManager.Session;
        //    var uf = Dll0202App.Core.SessionManager.UFSession;
        //    var part = Dll0202App.Core.SessionManager.WorkPart;

        //    string systemCsvPath = Dll0202App.IO.FilePathManager.SystemCsvPath;
        //    var systemCsv = new Dll0202App.IO.CsvManager(systemCsvPath);

        //    if (!System.IO.File.Exists(systemCsvPath))
        //    {
        //        mainForm.ShowBotMessage("❌ Missing System.csv: " + systemCsvPath);
        //        return;
        //    }

        //    string projectId = "";
        //    bool hasProjectID = false;
        //    try
        //    {
        //        string value;
        //        bool found;
        //        uf.Attr.GetStringUserAttribute(part.Tag, "info", 0, out value, out found);
        //        if (found && !string.IsNullOrEmpty(value))
        //        {
        //            projectId = value;
        //            hasProjectID = true;
        //        }
        //    }
        //    catch { hasProjectID = false; }

        //    if (!hasProjectID)
        //    {
        //        projectId = GenerateNewProjectID();
        //        var builder = session.AttributeManager.CreateAttributePropertiesBuilder(
        //            part, new NXOpen.NXObject[] { part }, NXOpen.AttributePropertiesBuilder.OperationType.None);
        //        builder.Title = "info";
        //        builder.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;
        //        builder.IsArray = true;
        //        builder.StringValue = projectId;
        //        builder.Commit();
        //        builder.Destroy();
        //        mainForm.ShowBotMessage("✅ Assigned new ProjectID to Part: " + projectId);
        //    }
        //    else
        //    {
        //        mainForm.ShowBotMessage("ℹ Using existing ProjectID from Part: " + projectId);
        //    }

        //    var systemRows = systemCsv.ReadAllRows();
        //    if (systemRows == null || systemRows.Count == 0)
        //    {
        //        mainForm.ShowBotMessage("❌ System.csv empty or invalid.");
        //        return;
        //    }

        //    string lastBodyId = systemRows[systemRows.Count - 1][0].Trim();
        //    if (string.IsNullOrEmpty(lastBodyId) || !lastBodyId.StartsWith("B"))
        //    {
        //        mainForm.ShowBotMessage("❌ Invalid last BodyID.");
        //        return;
        //    }

        //    int nextNumber = int.Parse(lastBodyId.Substring(1)) + 1;
        //    int assignedCount = 0;
        //    var newSystemRows = new List<List<string>>();

        //    foreach (NXOpen.Body nxBody in part.Bodies)
        //    {
        //        if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;

        //        var body = new Dll0202App.Objects.Body(nxBody.Tag);
        //        var infoAttr = body.GetObjectAttributes("info");

        //        if (infoAttr.Length > 0 && !string.IsNullOrEmpty(infoAttr[0]))
        //        {
        //            string id = infoAttr[0].Trim();
        //            Dll0202App.Tools.BodyTagResolver.CacheIDTag(id, body.Tag);
        //            continue;
        //        }

        //        string newBodyId = "B" + nextNumber.ToString("D7");
        //        body.SetObjectAttribute("info", 0, newBodyId);
        //        newSystemRows.Add(new List<string> { newBodyId, projectId });
        //        Dll0202App.Tools.BodyTagResolver.CacheIDTag(newBodyId, body.Tag);
        //        nextNumber++;
        //        assignedCount++;
        //    }

        //    if (newSystemRows.Count > 0)
        //    {
        //        foreach (var row in newSystemRows)
        //            systemCsv.AppendRow(row);

        //        mainForm.ShowBotMessage("✅ System.csv updated with " + newSystemRows.Count + " new entries.");
        //    }
        //    else
        //    {
        //        mainForm.ShowBotMessage("ℹ No new BodyID assigned.");
        //    }

        //    mainForm.ShowBotMessage("🎯 Total bodies assigned: " + assignedCount);
        //}

        public static void AssignSystemIDToAllBodies()
        {
            var mainForm = Dll0202App.Shell.MainForm.Instance;
            var session = Dll0202App.Core.SessionManager.Session;
            var uf = Dll0202App.Core.SessionManager.UFSession;
            var part = Dll0202App.Core.SessionManager.WorkPart;

            string systemCsvPath = Dll0202App.IO.FilePathManager.SystemCsvPath;
            var systemCsv = new Dll0202App.IO.CsvManager(systemCsvPath);

            if (!File.Exists(systemCsvPath))
            {
                mainForm.ShowBotMessage("❌ Missing System.csv: " + systemCsvPath);
                return;
            }

            // === 1. Lấy ProjectID hợp lệ từ Part ===
            string projectId = "";
            bool validProjectID = false;

            try
            {
                string val;
                bool found;
                uf.Attr.GetStringUserAttribute(part.Tag, "info", 0, out val, out found);
                if (found && !string.IsNullOrEmpty(val))
                {
                    val = val.Trim();
                    if (val.Length == 8 && val[1] == 'P')
                    {
                        int num;
                        if (int.TryParse(val.Substring(2), out num))
                        {
                            projectId = val;
                            validProjectID = true;
                        }
                    }
                }
            }
            catch { }

            // === 2. Nếu không hợp lệ → cấp mới ===
            if (!validProjectID)
            {
                var rows = systemCsv.ReadAllRows();
                int maxProjectNum = 0;
                char prefixChar = 'I'; // default prefix

                foreach (var row in rows)
                {
                    if (row.Count >= 2)
                    {
                        string pid = row[1].Trim();
                        if (pid.Length == 8 && pid[1] == 'P')
                        {
                            int num;
                            if (int.TryParse(pid.Substring(2), out num))
                            {
                                if (num > maxProjectNum)
                                {
                                    maxProjectNum = num;
                                    prefixChar = pid[0]; // lưu prefix ký tự đầu
                                }
                            }
                        }
                    }
                }

                projectId = prefixChar + "P" + (maxProjectNum + 1).ToString("D6");

                var builder = session.AttributeManager.CreateAttributePropertiesBuilder(
                    part, new NXOpen.NXObject[] { part }, NXOpen.AttributePropertiesBuilder.OperationType.None);
                builder.Title = "info";
                builder.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;
                builder.IsArray = true;
                builder.StringValue = projectId;
                builder.Commit(); builder.Destroy();

                mainForm.ShowBotMessage("✅ Assigned new ProjectID: " + projectId);
            }
            else
            {
                mainForm.ShowBotMessage("ℹ Using existing ProjectID: " + projectId);
            }

            // === 3. Lấy mã BodyID cuối cùng theo ProjectID ===
            var allRows = systemCsv.ReadAllRows();
            int maxBodyNumber = 0;

            foreach (var row in allRows)
            {
                if (row.Count >= 2 && row[1].Trim() == projectId)
                {
                    string id = row[0].Trim();
                    if (id.Length == 8 && id.StartsWith("B"))
                    {
                        int num;
                        if (int.TryParse(id.Substring(1), out num))
                            if (num > maxBodyNumber) maxBodyNumber = num;
                    }
                }
            }

            int nextNumber = maxBodyNumber + 1;
            int assignedCount = 0;
            var newRows = new List<List<string>>();

            // === 4. Duyệt tất cả body và gán ID nếu chưa có hoặc sai định dạng ===
            foreach (NXOpen.Body nxBody in part.Bodies)
            {
                if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;

                var body = new Dll0202App.Objects.Body(nxBody.Tag);
                var infoAttr = body.GetObjectAttributes("info");

                bool hasValidID = false;
                if (infoAttr.Length > 0 && !string.IsNullOrEmpty(infoAttr[0]))
                {
                    string id = infoAttr[0].Trim();
                    if (id.Length == 8 && id.StartsWith("B"))
                    {
                        int num;
                        if (int.TryParse(id.Substring(1), out num))
                        {
                            Dll0202App.Tools.BodyTagResolver.CacheIDTag(id, body.Tag);
                            hasValidID = true;
                        }
                    }
                }

                if (hasValidID) continue;

                string newBodyId = "B" + nextNumber.ToString("D7");
                body.SetObjectAttribute("info", 0, newBodyId);
                Dll0202App.Tools.BodyTagResolver.CacheIDTag(newBodyId, body.Tag);
                newRows.Add(new List<string> { newBodyId, projectId });
                nextNumber++;
                assignedCount++;
            }

            // === 5. Ghi dòng mới vào System.csv ===
            foreach (var row in newRows)
                systemCsv.AppendRow(row);

            if (assignedCount > 0)
                mainForm.ShowBotMessage("✅ Gán mới " + assignedCount + " BodyID.");
            else
                mainForm.ShowBotMessage("ℹ Không có body nào cần gán mã.");
        }

        public static void ExportBodyInfoToResultCSV(string projectId)
        {
            var mainForm = Dll0202App.Shell.MainForm.Instance;
            var part = Dll0202App.Core.SessionManager.WorkPart;

            string exportFolder = Dll0202App.IO.FilePathManager.GetProjectResultFolder(projectId);
            if (!System.IO.Directory.Exists(exportFolder))
                System.IO.Directory.CreateDirectory(exportFolder);

            string exportCsvPath = Dll0202App.IO.FilePathManager.GetProjectResultCsvPath(projectId);
            var exportCsv = new Dll0202App.IO.CsvManager(exportCsvPath);

            var header = new List<string>
    {
        "ID", "Cluster", "Group", "PartNo", "Name", "Type", "Revision", "Material", "Size", "Mass", "Remarks"
    };

            if (!System.IO.File.Exists(exportCsvPath))
                exportCsv.CreateNewFile(header);

            var oldRows = exportCsv.ReadAllRows();
            if (oldRows == null)
                oldRows = new List<List<string>>();

            if (oldRows.Count == 0 || oldRows[0].Count == 0 || oldRows[0][0] != "ID")
                oldRows.Insert(0, header);

            int addedCount = 0;

            foreach (NXOpen.Body body in part.Bodies)
            {
                if (body == null || body.Tag == NXOpen.Tag.Null) continue;

                var wrapBody = new Dll0202App.Objects.Body(body.Tag);
                var infoAttr = wrapBody.GetObjectAttributes("info");

                if (infoAttr.Length > 0 && !string.IsNullOrEmpty(infoAttr[0]))
                {
                    string bodyId = infoAttr[0].Trim();
                    var result = Dll0202App.AI.BodyTypeClassifier.DetectBodyTypeFull(wrapBody);
                    string type = result.Type;
                    string size = result.Size;

                    bool found = false;

                    for (int i = 1; i < oldRows.Count; i++)
                    {
                        if (oldRows[i].Count > 0 && oldRows[i][0] == bodyId)
                        {
                            while (oldRows[i].Count < header.Count)
                                oldRows[i].Add("");

                            oldRows[i][5] = type;
                            oldRows[i][8] = size;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var newRow = new List<string>();
                        newRow.Add(bodyId);
                        newRow.Add(""); // Cluster
                        newRow.Add(""); // Group
                        newRow.Add(""); // PartNo
                        newRow.Add(""); // Name
                        newRow.Add(type); // Type
                        newRow.Add(""); // Revision
                        newRow.Add(""); // Material
                        newRow.Add(size); // Size
                        newRow.Add(""); // Mass
                        newRow.Add(""); // Remarks

                        oldRows.Add(newRow);
                        addedCount++;
                    }
                }
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < oldRows.Count; i++)
            {
                string line = "";
                for (int j = 0; j < oldRows[i].Count; j++)
                {
                    if (j > 0) line += ",";
                    line += oldRows[i][j];
                }
                sb.AppendLine(line);
            }
            string newContent = sb.ToString();

            string oldContent = System.IO.File.Exists(exportCsvPath)
    ? System.IO.File.ReadAllText(exportCsvPath).Trim()
    : "";

            if (newContent.Trim() != oldContent)
            {
                exportCsv.OverwriteAll(oldRows);
                mainForm.ShowBotMessage("📄 Result CSV updated.");
            }
            else
            {
                mainForm.ShowBotMessage("ℹ No change detected. CSV not rewritten.");
            }

        }

        public static void LoadResultCsvToGrib(string projectId)
        {
            string path = Dll0202App.IO.FilePathManager.GetProjectResultCsvPath(projectId);
            if (!System.IO.File.Exists(path))
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không tìm thấy file kết quả: " + path);
                return;
            }

            Dll0202App.Shell.FormController.LoadCsvToPartList(path);
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📄 Đã load danh sách kết quả từ CSV.");
        }


        private static bool IsFileReady(string filename)
        {
            try
            {
                using (var inputStream = System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
        }

        private static string GenerateNewProjectID()
        {
            string systemCsvPath = Dll0202App.IO.FilePathManager.SystemCsvPath;
            if (!System.IO.File.Exists(systemCsvPath))
                return "IP000001";

            var systemCsv = new Dll0202App.IO.CsvManager(systemCsvPath);
            var rows = systemCsv.ReadAllRows();
            if (rows == null || rows.Count == 0)
                return "IP000001";

            int maxProjectNumber = 0;

            foreach (var row in rows)
            {
                if (row.Count >= 2)
                {
                    string projectId = row[1].Trim();
                    if (projectId.StartsWith("IP"))
                    {
                        int number;
                        if (int.TryParse(projectId.Substring(2), out number))
                        {
                            if (number > maxProjectNumber)
                                maxProjectNumber = number;
                        }
                    }
                }
            }

            int newProjectNumber = maxProjectNumber + 1;
            return "IP" + newProjectNumber.ToString("D6");
        }
    }

    public class BodyTypeResult
    {
        public string Type;
        public string Size;
    }

    public static class BodyTypeClassifier
    {
        public static BodyTypeResult DetectBodyTypeFull(Dll0202App.Objects.Body body)
        {
            double radius, length;
            if (IsShaft(body, out radius, out length))
            {
                string size = "ø" + (radius * 2).ToString("F0") + " x L" + length.ToString("F0");
                return new BodyTypeResult { Type = "Shaft", Size = size };
            }

            return new BodyTypeResult { Type = "Unknown", Size = "" };
        }

        public static string DetectBodyType(Dll0202App.Objects.Body body)
        {
            var result = DetectBodyTypeFull(body);
            return result.Type;
        }

        private static bool IsShaft(Dll0202App.Objects.Body body, out double bestRadius, out double length)
        {
            UFSession uf = Dll0202App.Core.SessionManager.UFSession;

            bestRadius = -1.0;
            length = 0.0;

            Tag[] faceList;
            uf.Modl.AskBodyFaces(body.Tag, out faceList);

            Tag bestFaceTag = Tag.Null;
            double[] bestCenter = null;
            double[] bestDirection = null;

            for (int i = 0; i < faceList.Length; i++)
            {
                Dll0202App.Objects.Face face = new Dll0202App.Objects.Face(faceList[i]);
                double[] data = face.AskFaceDataRaw();

                int faceType = (int)data[0];
                double[] center = new double[] { data[1], data[2], data[3] };
                double[] dir = new double[] { data[4], data[5], data[6] };
                double radius = data[13];
                int normDir = (int)data[15];

                if (faceType == 16 && normDir == 1 && radius > 0.5)
                {
                    if (bestFaceTag == Tag.Null || radius > bestRadius)
                    {
                        bestFaceTag = faceList[i];
                        bestRadius = radius;
                        bestCenter = center;
                        bestDirection = dir;
                    }
                }
            }

            if (bestFaceTag == Tag.Null)
                return false;

            Dll0202App.Objects.Edge[] edges = body.GetEdges();
            double tol = 0.01;

            for (int i = 0; i < edges.Length; i++)
            {
                NXOpen.Point3d start, end;
                edges[i].GetNativeEdge().GetVertices(out start, out end);

                if (!PointInsideTrunk(start, bestCenter, bestDirection, bestRadius, tol) ||
                    !PointInsideTrunk(end, bestCenter, bestDirection, bestRadius, tol))
                {
                    return false;
                }
            }

            length = GetTrunkLengthFromVertices(edges, bestCenter, bestDirection);
            return true;
        }

        private static double GetTrunkLengthFromVertices(Dll0202App.Objects.Edge[] edges, double[] origin, double[] dir)
        {
            double dirLen = Math.Sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);
            double[] Z = new double[] { dir[0] / dirLen, dir[1] / dirLen, dir[2] / dirLen };

            double minT = double.MaxValue;
            double maxT = double.MinValue;

            for (int i = 0; i < edges.Length; i++)
            {
                NXOpen.Point3d p1, p2;
                edges[i].GetNativeEdge().GetVertices(out p1, out p2);

                NXOpen.Point3d[] points = new NXOpen.Point3d[] { p1, p2 };
                for (int j = 0; j < 2; j++)
                {
                    double[] P = new double[] { points[j].X, points[j].Y, points[j].Z };
                    double[] OP = new double[] { P[0] - origin[0], P[1] - origin[1], P[2] - origin[2] };
                    double t = OP[0] * Z[0] + OP[1] * Z[1] + OP[2] * Z[2];

                    if (t < minT) minT = t;
                    if (t > maxT) maxT = t;
                }
            }

            return Math.Abs(maxT - minT);
        }

        private static bool PointInsideTrunk(Point3d pt, double[] origin, double[] dir, double R, double tol)
        {
            double[] P = new double[] { pt.X, pt.Y, pt.Z };
            double[] OP = new double[] { P[0] - origin[0], P[1] - origin[1], P[2] - origin[2] };

            double[] cross = new double[]
            {
                OP[1] * dir[2] - OP[2] * dir[1],
                OP[2] * dir[0] - OP[0] * dir[2],
                OP[0] * dir[1] - OP[1] * dir[0]
            };

            double crossLen = Math.Sqrt(cross[0] * cross[0] + cross[1] * cross[1] + cross[2] * cross[2]);
            double dirLen = Math.Sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);

            double dist = crossLen / dirLen;
            return dist <= R + tol;
        }
    }
}


public class BodyTypeResult
{
    public string Type;    // Shaft, Unknown, ...
    public string Size;    // ø20 x L150, hoặc ""


}

#endregion

#region Objects
namespace Dll0202App.Objects
{
    public abstract class Tagged
    {
        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_ask_type_and_subtype(uint objTag, out int type, out int subtype);

        public Tag Tag { get; private set; }

        protected Tagged(Tag tag)
        {
            this.Tag = tag;
        }

        public bool IsValid()
        {
            return Tag != Tag.Null;
        }

        public override string ToString()
        {
            return "Tag: " + Tag.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// 
        /// 
        public void GetObjectType(out int type, out int subtype)
        {
            int error = UF_OBJ_ask_type_and_subtype((uint)Tag, out type, out subtype);
            if (error != 0)
                throw new Exception("UF_OBJ_ask_type_and_subtype failed with code: " + error);
        }
    }

    public abstract class NXObject : Tagged
    {
        protected NXObject(Tag tag) : base(tag) { }

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_set_name(uint objectTag, string name);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_ask_name(uint objectTag, System.Text.StringBuilder name);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_ATTR_set_string_user_attribute(
            Tag tag,
            [MarshalAs(UnmanagedType.LPStr)] string title,
            int index,
            [MarshalAs(UnmanagedType.LPStr)] string value,
            [MarshalAs(UnmanagedType.I1)] bool append);

        public void SetObjectAttribute(string title, int index, string value)
        {
            int error = UF_ATTR_set_string_user_attribute(this.Tag, title, index, value, true);
            if (error != 0)
                throw new Exception("UF_ATTR_set_string_user_attribute failed with code: " + error);
        }

        public string[] GetObjectAttributes(string title, int maxIndex = 5)
        {
            System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();
            for (int i = 0; i <= maxIndex; i++)
            {
                string value;
                bool found;
                Core.SessionManager.UFSession.Attr.GetStringUserAttribute(this.Tag, title, i, out value, out found);
                if (!found) break;
                values.Add(value);
            }
            return values.ToArray();

        }

        public void ClearObjectAttribute(string title, int maxIndex = 5)
        {
            for (int i = 0; i <= maxIndex; i++)
            {
                UF_ATTR_set_string_user_attribute(this.Tag, title, i, string.Empty, true);
            }
        }

        /// <summary>
        /// Đặt tên cho đối tượng NX (Body, Face, Edge...) bằng UF cấp thấp
        /// </summary>
        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            int error = UF_OBJ_set_name((uint)Tag, name);
            if (error != 0)
                throw new Exception("UF_OBJ_set_name failed with code: " + error);
        }

        /// <summary>
        /// Lấy tên của đối tượng NX (Body, Face...) qua UF cấp thấp
        /// </summary>
        public string GetName()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(132); // NX max length 132
            int error = UF_OBJ_ask_name((uint)Tag, sb);
            if (error != 0)
                throw new Exception("UF_OBJ_ask_name failed with code: " + error);
            return sb.ToString();
        }
    }

    public class DisplayableObject : NXObject
    {
        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_set_color(uint objectTag, int color);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_set_layer(uint objectTag, int layer);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_set_blank_status(uint objectTag, int status);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_ask_blank_status(uint objectTag, out int status);

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_DISP_set_highlight(uint objectTag, int highlight);

        [StructLayout(LayoutKind.Sequential)]
        public struct UF_OBJ_disp_props_t
        {
            public int layer;
            public int color;
            public int blank_status;
            public int line_width;
            public int line_font;
        }

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_OBJ_ask_display_properties(uint objectTag, ref UF_OBJ_disp_props_t props);


        public DisplayableObject(Tag tag) : base(tag) { }

        public void SetColor(int color)
        {
            UF_OBJ_set_color((uint)Tag, color);
        }

        public void SetLayer(int layer)
        {
            UF_OBJ_set_layer((uint)Tag, layer);
        }

        public void SetBlank(bool blank)
        {
            int status = blank ? 1 : 0;
            UF_OBJ_set_blank_status((uint)Tag, status);
        }

        //public bool IsBlanked()
        //{
        //    int status;
        //    int err = UF_OBJ_ask_blank_status((uint)Tag, out status);
        //    return err == 0 && status == 1;
        //}

        public void Highlight()
        {
            UF_DISP_set_highlight((uint)Tag, 1);
        }

        public void Unhighlight()
        {
            UF_DISP_set_highlight((uint)Tag, 0);
        }

        /// <summary>
        /// int props.color :ID Color
        /// int props.layer : ID Layer
        /// int props.blank_status : 1 => blanked (ẩn), 0 => not blanked (hiện)
        /// int props.line_width : 5=>0.13, 6=>0.18, 7=>0.25...
        /// int props.line_font : 1=>solid, 2=>dashed, 3=>phantom...
        /// </summary>
        /// <returns></returns>
        public UF_OBJ_disp_props_t GetDisplayProperties()
        {
            UF_OBJ_disp_props_t props = new UF_OBJ_disp_props_t();
            int error = UF_OBJ_ask_display_properties((uint)Tag, ref props);
            return props;
        }

        public NXOpen.Body GetNativeBody()
        {
            return (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(this.Tag);
        }


    }

    public class Body : DisplayableObject
    {
        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_MODL_ask_mass_props_3d(
            Tag body,
            double[] mass_props,
            double[] matrix);

        public Body(Tag tag) : base(tag) { }

        /// <summary>
        /// Lấy danh sách các mặt (Face) từ Body
        /// </summary>
        /// <returns>Mảng các đối tượng Face</returns>
        public Face[] GetFaces()
        {
            NXOpen.Body nxBody = (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(Tag);
            var faces = nxBody.GetFaces();
            List<Face> result = new List<Face>();
            foreach (var f in faces)
            {
                result.Add(new Face(f.Tag));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Lấy danh sách các cạnh (Edge) từ Body
        /// </summary>
        /// <returns>Mảng các đối tượng Edge</returns>
        public Edge[] GetEdges()
        {
            NXOpen.Body nxBody = (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(Tag);
            var edges = nxBody.GetEdges();
            List<Edge> result = new List<Edge>();
            foreach (var e in edges)
            {
                result.Add(new Edge(e.Tag));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Kiểm tra Body có phải dạng solid (khối đặc) không
        /// </summary>
        /// <returns>True nếu là solid body, false nếu không</returns>
        public bool IsSolidBody()
        {
            NXOpen.Body nxBody = (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(Tag);
            return nxBody.IsSolidBody;
        }

        /// <summary>
        /// Kiểm tra Body có phải dạng sheet (tấm mỏng) không
        /// </summary>
        /// <returns>True nếu là sheet body, false nếu không</returns>
        public bool IsSheetBody()
        {
            NXOpen.Body nxBody = (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(Tag);
            return nxBody.IsSheetBody;
        }

        /// <summary>
        /// dùng với 1 tag
        /// Mảng 47 phần tử chứa các thông số vật lý, đơn vị đã chuyển về mm:
        /// S   [0]  Surface Area (mm²)
        /// S   [1]  Volume (mm³)
        /// S   [2]  Mass (g)
        /// M   [3-5] Center of Mass (mm): X, Y, Z
        /// M   [6-8] First Moments (mm⁴): X, Y, Z
        /// S   [9-11] Moments of Inertia in WCS (mm⁵): Ixx, Iyy, Izz
        /// S   [12-14] Moments of Inertia about Centroid (mm⁵): Ixx, Iyy, Izz
        /// S   [15] Spherical Moment of Inertia (mm⁵)
        /// M   [16-18] Inertia Products in WCS (mm⁵): Ixy, Iyz, Izx
        /// M   [19-21] Inertia Products about Centroid (mm⁵): Ixy, Iyz, Izx
        /// M   [22-24] Principal Axis #1 (unitless): X, Y, Z
        /// M   [25-27] Principal Axis #2 (unitless): X, Y, Z
        /// M   [28-30] Principal Axis #3 (unitless): X, Y, Z
        /// S   [31-33] Principal Moments (mm⁵): I1, I2, I3
        /// M   [34-36] Radii of Gyration in WCS (mm): X, Y, Z
        /// S   [37-39] Radii of Gyration about Centroid (mm): X, Y, Z
        /// S   [40] Spherical Radius of Gyration (mm)
        ///      [41-45] (No use)
        /// S   [46] Density (g/mm³)
        /// <summary>
        public double[] LibAskMassProps3D()
        {
            var part = Core.SessionManager.WorkPart;
            bool partIsMM = (part.PartUnits == BasePart.Units.Millimeters);

            // Scale theo đơn vị
            double scale = partIsMM ? 1.0 : 25.4;
            double scale2 = scale * scale;
            double scale3 = scale2 * scale;
            double scale4 = scale3 * scale;
            double scale5 = scale4 * scale;

            double[] props = new double[47];    // Kết quả gốc từ UF
            double[] matrix = new double[9];    // Ma trận quán tính (không dùng ở đây)

            int err = UF_MODL_ask_mass_props_3d(this.Tag, props, matrix);
            if (err != 0)
                throw new Exception("UF_MODL_ask_mass_props_3d failed with code: " + err);

            // Scale lại đơn vị
            double[] result = new double[47];
            for (int i = 0; i < 47; i++)
            {
                if (i == 0) result[i] = props[i] * scale2;         // Area
                else if (i == 1) result[i] = props[i] * scale3;    // Volume
                else if (i == 2) result[i] = props[i];             // Mass
                else if (i >= 3 && i <= 5) result[i] = props[i] * scale;        // Center of Mass
                else if (i >= 6 && i <= 8) result[i] = props[i] * scale4;       // First moments
                else if (i >= 9 && i <= 14) result[i] = props[i] * scale5;      // Moment of inertia
                else if (i == 15) result[i] = props[i] * scale5;                // Spherical moment
                else if (i >= 16 && i <= 21) result[i] = props[i] * scale5;     // Inertia products
                else if (i >= 22 && i <= 30) result[i] = props[i];              // Principal axes (unitless)
                else if (i >= 31 && i <= 33) result[i] = props[i] * scale5;     // Principal moments
                else if (i >= 34 && i <= 36) result[i] = props[i] * scale;      // Radii of gyration (WCS)
                else if (i >= 37 && i <= 39) result[i] = props[i] * scale;      // Radii of gyration (centroidal)
                else if (i == 40) result[i] = props[i] * scale;                 // Spherical radius of gyration
                else if (i == 46) result[i] = props[i];                         // Density
                else result[i] = props[i]; // các phần tử không dùng
            }

            return result;
        }


        /// </returns>
        /// <summary>
        /// Wrapper UF, dùng trên nhiều tags
        /// Trả về mảng 47 phần tử chứa các thông số vật lý, đơn vị gốc:
        /// [0]  Surface Area
        /// [1]  Volume
        /// [2]  Mass (g)
        /// [3-5] Center of Mass: X, Y, Z
        /// [6-8] First Moments: X, Y, Z
        /// [9-11] Moments of Inertia in WCS: Ixx, Iyy, Izz
        /// [12-14] Moments of Inertia about Centroid: Ixx, Iyy, Izz
        /// [15] Spherical Moment of Inertia
        /// [16-18] Inertia Products in WCS: Ixy, Iyz, Izx
        /// [19-21] Inertia Products about Centroid: Ixy, Iyz, Izx
        /// [22-24] Principal Axis #1: X, Y, Z
        /// [25-27] Principal Axis #2: X, Y, Z
        /// [28-30] Principal Axis #3: X, Y, Z
        /// [31-33] Principal Moments: I1, I2, I3
        /// [34-36] Radii of Gyration in WCS: X, Y, Z
        /// [37-39] Radii of Gyration about Centroid: X, Y, Z
        /// [40] Spherical Radius of Gyration
        /// [41-45] (No use)
        /// [46] Density (g/mm³ nếu mm; g/in³ nếu inch)
        /// </summary>
        public double[] UFAskMassProps3D()
        {
            var uf = Core.SessionManager.UFSession;

            Tag[] tags = new Tag[] { Tag };
            double[] acc = new double[1];
            double[] props = new double[47];
            double[] stats = new double[13];

            try
            {
                uf.Modl.AskMassProps3d(tags, 1, 1, 1, 1.0, 1, acc, props, stats);
            }
            catch (Exception ex)
            {
                throw new Exception("UFModl.AskMassProps3d failed: " + ex.Message);
            }


            return props;
        }

        public double[] AskBoundingBox()
        {
            double[] box = new double[6];
            Core.SessionManager.UFSession.Modl.AskBoundingBox(this.Tag, box);
            return box;
        }
    }

    public class Face : DisplayableObject
    {
        public Face(Tag tag) : base(tag) { }

        [DllImport("libufun.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UF_MODL_ask_face_data(
            Tag faceTag,
            out int faceType,
            [Out] double[] facePoint,
            [Out] double[] faceDir,
            [Out] double[] boundingBox,
            out double radius,
            out double radData,
            out int normDir);

        /// <summary>
        /// Trả về mảng 16 giá trị mô tả hình học của một Face từ hàm UF_MODL_ask_face_data.
        ///
        /// Các ý nghĩa:
        /// [0]  → Type: Mã loại mặt (NX Surface Type Code):
        ///         16 = Cylinder, 17 = Cone, 18 = Sphere, 19 = Revolved/Torus,
        ///         20 = Extruded Surface, 22 = Bounded Plane,
        ///         23 = Fillet/Blend Surface, 43 = B-surface, 65 = Offset Surface, 66 = Foreign Surface
        ///
        /// [1]–[3] → Điểm đặc trưng (Point):
        ///         - [1]: X tọa độ
        ///         - [2]: Y tọa độ
        ///         - [3]: Z tọa độ
        ///       → Là tâm cầu, tâm xuyến, trục cylinder/cone, hoặc 1 điểm trên mặt phẳng.
        ///
        /// [4]–[6] → Vector hướng đặc trưng (Direction Vector):
        ///         - [4]: X thành phần vector
        ///         - [5]: Y thành phần vector
        ///         - [6]: Z thành phần vector
        ///       → Là vector pháp tuyến (Plane) hoặc trục (Cylinder, Cone, Torus).
        ///
        /// [7]–[12] → Bounding Box bao ngoài mặt (AABB):
        ///         - [7]: Min X
        ///         - [8]: Min Y
        ///         - [9]: Min Z
        ///         - [10]: Max X
        ///         - [11]: Max Y
        ///         - [12]: Max Z
        ///       → Hộp giới hạn hình chữ nhật bao sát mặt theo trục tọa độ tuyệt đối.
        ///
        /// [13] → Radius (Bán kính chính):
        ///         - Cylinder: bán kính trụ.
        ///         - Sphere: bán kính cầu.
        ///         - Cone: bán kính tại vị trí [1–3].
        ///         - Torus: bán kính lớn (major radius).
        ///         - Các mặt khác (plane, bspline,...) = 0.
        ///
        /// [14] → RadData (Thông tin phụ):
        ///         - Cone: nửa góc nón (half-angle) (đơn vị radian).
        ///         - Torus: bán kính nhỏ (minor radius).
        ///         - Các mặt khác = 0.
        ///
        /// [15] → NormDir (Hướng pháp tuyến):
        ///         - +1: Pháp tuyến cùng chiều surface normal.
        ///         - -1: Pháp tuyến ngược chiều surface normal.
        ///
        /// Ghi chú:
        /// - Tất cả các tọa độ và vector trả về theo hệ tọa độ tuyệt đối (Absolute Coordinate System) của Display Part.
        /// - Bounding box không hoàn toàn chính xác tuyệt đối, chỉ để tham khảo nhanh.
        /// </summary>

        public double[] AskFaceDataRaw()
        {
            int type, normDir;
            double radius, radData;
            double[] point = new double[3];
            double[] dir = new double[3];
            double[] bbox = new double[6];

            int err = UF_MODL_ask_face_data(Tag, out type, point, dir, bbox, out radius, out radData, out normDir);
            if (err != 0)
                throw new Exception("UF_MODL_ask_face_data failed, code = " + err);

            double[] result = new double[16];
            result[0] = type;                      // [0] Loại mặt
            Array.Copy(point, 0, result, 1, 3);    // [1-3] Điểm đặc trưng (X, Y, Z)
            Array.Copy(dir, 0, result, 4, 3);      // [4-6] Vector hướng (X, Y, Z)
            Array.Copy(bbox, 0, result, 7, 6);     // [7-12] Bounding box (Min/Max XYZ)
            result[13] = radius;                   // [13] Bán kính
            result[14] = radData;                  // [14] Dữ liệu phụ về bán kính
            result[15] = normDir;                  // [15] Hướng pháp tuyến

            return result;
        }

        public Edge[] GetEdges()
        {
            NXOpen.Face nxFace = (NXOpen.Face)NXObjectManager.Get(this.Tag);
            var nxEdges = nxFace.GetEdges();
            List<Edge> result = new List<Edge>();
            foreach (var edge in nxEdges)
            {
                result.Add(new Edge(edge.Tag));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Trả về NXOpen.Face tương ứng từ Tag hiện tại.
        /// </summary>
        public NXOpen.Face GetNativeFace()
        {
            return (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(this.Tag);
        }

        /// <summary>
        /// Trả về danh sách các cạnh (Edge) được chia sẻ giữa mặt này và một mặt khác.
        /// </summary>
        /// <param name="otherFace">Face cần so sánh.</param>
        /// <returns>Danh sách các cạnh chung (Edge[]).</returns>
        public Edge[] AskSharedEdges(Face otherFace)
        {
            if (otherFace == null)
                return new Edge[0];

            UFSession ufSession = Dll0202App.Core.SessionManager.UFSession;

            Tag[] sharedEdgeTags;
            ufSession.Modl.AskSharedEdges(this.Tag, otherFace.Tag, out sharedEdgeTags);

            if (sharedEdgeTags == null || sharedEdgeTags.Length == 0)
                return new Edge[0];

            List<Edge> result = new List<Edge>();
            foreach (var edgeTag in sharedEdgeTags)
            {
                if (edgeTag != Tag.Null)
                {
                    result.Add(new Edge(edgeTag));
                }
            }

            return result.ToArray();
        }



    }

    public class Edge : DisplayableObject
    {
        public Edge(Tag tag) : base(tag) { }

        public NXOpen.Edge.EdgeType SolidEdgeType
        {
            get
            {
                NXOpen.Edge edge = (NXOpen.Edge)NXObjectManager.Get(this.Tag);
                return edge.SolidEdgeType;
            }
        }

        public double GetLength()
        {
            NXOpen.Edge edge = (NXOpen.Edge)NXObjectManager.Get(this.Tag);
            return edge.GetLength();
        }

        public void GetVertices(out Point3d start, out Point3d end)
        {
            NXOpen.Edge edge = (NXOpen.Edge)NXObjectManager.Get(this.Tag);
            edge.GetVertices(out start, out end);
        }

        public NXOpen.Edge GetNativeEdge()
        {
            return (NXOpen.Edge)NXOpen.Utilities.NXObjectManager.Get(this.Tag);
        }

    }
}
#endregion

#region Tools
namespace Dll0202App.Tools
{
    public static class SelectTool
    {
        /// <summary>
        /// Select Bodies
        /// </summary>
        public static Objects.Body[] SelectBody()
        {
            UI ui = Core.SessionManager.UI;
            TaggedObject[] selected;
            string message = "Select Body";

            var response = ui.SelectionManager.SelectTaggedObjects(
                message,
                message,
                NXOpen.Selection.SelectionScope.AnyInAssembly,
                false,
                new NXOpen.Selection.SelectionType[] { NXOpen.Selection.SelectionType.All },
                out selected
            );

            if (response != NXOpen.Selection.Response.Ok || selected == null)
                return new Objects.Body[0];

            List<Objects.Body> bodies = new List<Objects.Body>();
            foreach (TaggedObject obj in selected)
            {
                NXOpen.Body nxBody = obj as NXOpen.Body;
                if (nxBody != null)
                    bodies.Add(new Objects.Body(nxBody.Tag));
            }

            return bodies.ToArray();
        }

        /// <summary>
        /// Select Faces
        /// </summary>
        public static Dll0202App.Objects.Face[] SelectFace()
        {
            UI ui = UI.GetUI();
            TaggedObject[] selected;
            string message = "Select Face";

            var resp = ui.SelectionManager.SelectTaggedObjects(
                message,
                message,
                NXOpen.Selection.SelectionScope.AnyInAssembly,
                false,
                new[] { NXOpen.Selection.SelectionType.Faces },
                out selected
            );

            if (resp != NXOpen.Selection.Response.Ok || selected == null)
                return new Dll0202App.Objects.Face[0];

            List<Dll0202App.Objects.Face> faces = new List<Dll0202App.Objects.Face>();
            foreach (var obj in selected)
            {
                if (obj is NXOpen.Face)
                    faces.Add(new Dll0202App.Objects.Face(obj.Tag));
            }

            return faces.ToArray();
        }

        /// <summary>
        /// Trả về danh sách tất cả Body trong Part hiện tại, đã được gói trong Template1.Objects.Body.
        /// </summary>
        public static List<Dll0202App.Objects.Body> GetAllBodiesInPart()
        {
            var part = Core.SessionManager.WorkPart;
            var bodies = new List<Dll0202App.Objects.Body>();

            foreach (NXOpen.Body nxBody in part.Bodies)
            {
                if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;
                bodies.Add(new Dll0202App.Objects.Body(nxBody.Tag));
            }

            return bodies;
        }
    }

    public static class BodyTagResolver
    {
        private static Dictionary<string, NXOpen.Tag> _cachedIDTagMap = new Dictionary<string, NXOpen.Tag>();
        private static NXOpen.Tag _lastHighlightedTag = NXOpen.Tag.Null;
        private static bool _hasBuiltFallbackMap = false;



        /// <summary>Gọi sau khi chạy các hàm phân loại/tính toán, để cache ID → Tag</summary>
        public static void CacheIDTag(string bodyID, NXOpen.Tag tag)
        {
            if (!_cachedIDTagMap.ContainsKey(bodyID))
                _cachedIDTagMap.Add(bodyID, tag);
        }

        /// <summary>Clear toàn bộ cache nếu cần</summary>
        public static void ClearCache()
        {
            _cachedIDTagMap.Clear();
        }

        /// <summary>Lấy tag từ ID. Nếu không có trong cache, duyệt toàn bộ để tìm.</summary>
        public static NXOpen.Tag GetTagFromID(string bodyID)
        {
            // Nếu đã có trong cache → dùng luôn
            if (_cachedIDTagMap.ContainsKey(bodyID))
                return _cachedIDTagMap[bodyID];

            // Nếu chưa xây fallback map → duyệt 1 lần duy nhất để lấy toàn bộ
            if (!_hasBuiltFallbackMap)
            {
                BuildFullIDTagMap(); // duyệt toàn bộ body để cache hết
                _hasBuiltFallbackMap = true;
            }

            // Thử lại lần 2 sau khi đã build toàn bộ
            if (_cachedIDTagMap.ContainsKey(bodyID))
                return _cachedIDTagMap[bodyID];

            return NXOpen.Tag.Null;
        }

        private static void BuildFullIDTagMap()
        {
            NXOpen.Part part = Dll0202App.Core.SessionManager.WorkPart;

            foreach (NXOpen.Body nxBody in part.Bodies)
            {
                if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;

                var wrap = new Dll0202App.Objects.Body(nxBody.Tag);
                var infoAttr = wrap.GetObjectAttributes("info");

                if (infoAttr.Length > 0)
                {
                    string id = infoAttr[0].Trim();
                    if (!_cachedIDTagMap.ContainsKey(id))
                        _cachedIDTagMap.Add(id, nxBody.Tag);
                }
            }
        }


        /// <summary>Duyệt toàn bộ body, kiểm tra attribute "info" để tìm body có ID tương ứng</summary>
        private static NXOpen.Tag FindTagByID(string bodyID)
        {
            NXOpen.Part part = Dll0202App.Core.SessionManager.WorkPart;

            foreach (NXOpen.Body nxBody in part.Bodies)
            {
                if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;

                var wrap = new Dll0202App.Objects.Body(nxBody.Tag);
                var attrs = wrap.GetObjectAttributes("info");

                if (attrs.Length > 0 && attrs[0].Trim() == bodyID)
                    return nxBody.Tag;
            }

            return NXOpen.Tag.Null;
        }

        public static void HighlightByID(string id)
        {
            NXOpen.Tag tag = GetTagFromID(id);

            if (tag != NXOpen.Tag.Null)
            {
                HighlightTag(tag);
            }
            else
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không tìm thấy body có ID = " + id);
            }
        }


        public static void HighlightTag(NXOpen.Tag tag)
        {
            NXOpen.UF.UFSession uf = Dll0202App.Core.SessionManager.UFSession;

            // Bỏ highlight body cũ nếu có
            if (_lastHighlightedTag != NXOpen.Tag.Null)
                uf.Disp.SetHighlight(_lastHighlightedTag, 0); // 0 = tắt

            // Highlight body mới nếu hợp lệ
            if (tag != NXOpen.Tag.Null)
            {
                uf.Disp.SetHighlight(tag, 1); // 1 = bật
                _lastHighlightedTag = tag;
            }
        }
    }

    public static class BodyTools
    {
        /// <summary>
        /// Assign color to selected Bodies and all their Faces.
        /// </summary>
        /// <param name="bodyColor">Color index to apply to Body</param>
        /// <param name="faceColor">Color index to apply to Faces</param>
        public static void SetColorForSelectionBodies(int bodyColor = 5, int faceColor = 5)
        {
            var bodies = SelectTool.SelectBody();

            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            foreach (var body in bodies)
            {
                // Set color for body
                body.SetColor(bodyColor);

                // Set color for each face in the body
                var faces = body.GetFaces();
                foreach (var face in faces)
                {
                    face.SetColor(faceColor);
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🎨 Colored {0} selected body(ies) and their faces. Body Color = {1}, Face Color = {2}",
                bodies.Length, bodyColor, faceColor));
        }

        /// <summary>
        /// Set a name to all selected Bodies from the UI.
        /// </summary>
        /// <param name="name">Name to assign (default = "Test Name")</param>
        public static void SetNameToSelectionBodies(string name = "Test Name")
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❗ No bodies selected.");
                return;
            }

            int count = 0;

            foreach (var body in bodies)
            {
                try
                {
                    body.SetName(name);
                    count++;
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Failed to set name for Body Tag " + (uint)body.Tag + " : " + ex.Message);
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("✅ Set name '{0}' to {1} body(ies).", name, count));
        }

        /// <summary>
        /// Assign multiple indexed values under one attribute title to selected Bodies.
        /// Tools.BodyTools.SetAttributesForSelectionBodies("BodyInfo", "Base", "S45C", "±58HRC", "Black Anod");
        /// </summary>
        /// <param name="title">Attribute title (e.g., "BodyInfo")</param>
        /// <param name="values">Values to assign at index 0,1,2,...</param>
        public static void SetAttributesForSelectionBodies(string title, params string[] values)
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No body selected.");
                return;
            }

            foreach (var body in bodies)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    body.SetObjectAttribute(title, i, values[i]);
                }

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("✔ Body {0} → Title '{1}' with {2} values", (uint)body.Tag, title, values.Length));
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Attributes assigned.");
        }

        /// <summary>
        /// Read and display all indexed attributes of selected Bodies with the given title, using UI chat output.
        /// </summary>
        /// <param name="title">Attribute title (e.g., "BodyInfo")</param>
        public static void GetAttributesOfSelectionBodies(string title)
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No body selected.");
                return;
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("📌 Reading attributes with title: '{0}'", title));

            foreach (var body in bodies)
            {
                string[] values = body.GetObjectAttributes(title);
                if (values.Length == 0)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔸 Body {0} → No attributes found.", (uint)body.Tag));
                    continue;
                }

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("📦 Body {0}:", (uint)body.Tag));
                for (int i = 0; i < values.Length; i++)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("    Index {0} → {1}", i, values[i]));
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Done reading attributes.");
        }


        /// <summary>
        /// Print the number of Faces for each selected Body.
        /// </summary>
        public static void GetFacesOfSelectionBodies()
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No body selected.");
                return;
            }

            foreach (var body in bodies)
            {
                var faces = body.GetFaces();
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔷 Body {0} : {1} face(s).", (uint)body.Tag, faces.Length));
            }

        }

        /// <summary>
        /// Print the number of Edges for each selected Body.
        /// </summary>
        public static void GetEdgesOfSelectionBodies()
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No body selected.");
                return;
            }

            foreach (var body in bodies)
            {
                var edges = body.GetEdges();
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔶 Body {0} : {1} edge(s).", (uint)body.Tag, edges.Length));
            }
        }

        /// <summary>
        /// Check and display if selected bodies are Solid or Sheet type.
        /// </summary>
        public static void GetTypeSolidOrSheetOfSelectionBodies()
        {
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📌 Checking type (Solid/Sheet) of selected bodies...");

            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            foreach (var body in bodies)
            {
                string type = "Unknown";
                if (body.IsSolidBody()) type = "Solid";
                else if (body.IsSheetBody()) type = "Sheet";

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔷 Body {0} → Type: {1}", (uint)body.Tag, type));
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Done checking body types.");
        }

        /// <summary>
        /// Read and display visual properties (color, layer, blank status, etc.)
        /// of selected Bodies from the current NX Part.
        /// </summary>
        public static void GetDisplayPropOfSelectionBodies()
        {
            var lw = Core.SessionManager.Session.ListingWindow;
            lw.Open();

            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            foreach (var body in bodies)
            {
                try
                {
                    var props = body.GetDisplayProperties();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format(
                        "🔷 Body Tag {0} → Color: {1}, Layer: {2}, Blank: {3}, Width: {4}, Font: {5}",
                        (uint)body.Tag,
                        props.color,
                        props.layer,
                        props.blank_status,
                        props.line_width,
                        props.line_font
                    ));
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("❌ Error with Body Tag {0}: {1}", (uint)body.Tag, ex.Message));
                }
            }
        }

        /// <summary>
        /// Return a list of original Tags of selected bodies from UI.
        /// If the body is an occurrence, returns its prototype tag.
        /// </summary>
        public static List<string> GetTagsOfSelectionBodies()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return new List<string>();
            }

            List<string> result = new List<string>();

            foreach (var body in bodies)
            {
                NXOpen.Body native = (NXOpen.Body)NXOpen.Utilities.NXObjectManager.Get(body.Tag);
                NXOpen.Body protoBody = native.IsOccurrence ? (NXOpen.Body)native.Prototype : native;
                NXOpen.Tag tag = protoBody.Tag;

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("🔷 Body Tag = " + ((uint)tag).ToString());
                result.Add(tag.ToString());
            }

            return result;
        }

        /// <summary>
        /// Get and display the names of all selected Bodies from the UI.
        /// </summary>
        public static void GetNameOfSelectionBodies()
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❗ No bodies selected.");
                return;
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📋 List of body names:");

            foreach (var body in bodies)
            {
                try
                {
                    string name = body.GetName();
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔷 Body Tag: {0} → Name: {1}", (uint)body.Tag, name));
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("❌ Body Tag: {0} → Error: {1}", (uint)body.Tag, ex.Message));
                }
            }
        }

        /// <summary>
        /// Select multiple bodies from the UI and retrieve 47 physical mass properties for each.
        /// </summary>
        public static void GetMassProp3DOfSelectionBodiesIndex()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            foreach (var body in bodies)
            {
                double[] props = body.UFAskMassProps3D();

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📦 Body Tag = " + (uint)body.Tag + " → 47 Mass Properties:");
                for (int i = 0; i < props.Length; i++)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   [{0}] = {1}", i, props[i].ToString("F4")));
                }
            }
        }

        /// <summary>
        /// Chọn nhiều body từ giao diện và truy xuất 47 chỉ số vật lý cho từng body
        /// </summary>
        public static void GetMassProp3DOfSelectionBodies()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ Không có body nào được chọn.");
                return;
            }

            foreach (var nxBody in bodies)
            {
                var myBody = new Dll0202App.Objects.Body(nxBody.Tag);

                // Kiểm tra solid body
                if (!myBody.IsSolidBody())
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Bỏ qua Body " + (uint)myBody.Tag + " (không phải Solid).");
                    continue;
                }

                double[] raw;
                try
                {
                    raw = myBody.UFAskMassProps3D();
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Lỗi khi lấy khối lượng Body " + (uint)myBody.Tag + ": " + ex.Message);
                    continue;
                }

                double area = raw[0] * 25.4 * 25.4;
                double volume = raw[1] * 25.4 * 25.4 * 25.4;
                double mass = raw[2];
                double[] centerOfMass = { raw[3] * 25.4, raw[4] * 25.4, raw[5] * 25.4 };
                double[] firstMoments = { raw[6], raw[7], raw[8] };
                double[] inertiaWCS = { raw[9], raw[10], raw[11] };
                double[] inertiaCentroid = { raw[12], raw[13], raw[14] };
                double sphericalMoment = raw[15];
                double[] inertiaProductsWCS = { raw[16], raw[17], raw[18] };
                double[] inertiaProductsCentroid = { raw[19], raw[20], raw[21] };

                double[][] principalAxes = new double[3][]
        {
            new double[3] { raw[22], raw[23], raw[24] },
            new double[3] { raw[25], raw[26], raw[27] },
            new double[3] { raw[28], raw[29], raw[30] }
        };

                double[] principalMoments = { raw[31], raw[32], raw[33] };
                double[] radiiWCS = { raw[34], raw[35], raw[36] };
                double[] radiiCentroid = { raw[37], raw[38], raw[39] };
                double sphericalRadius = raw[40];
                double density = raw[46];

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("=== Body Tag = " + myBody.Tag + " ===");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Area (mm²): " + area.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Volume (mm³): " + volume.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Mass: " + mass.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Center of Mass: [" + string.Join(", ", ConvertArrayToString(centerOfMass)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("First Moments: [" + string.Join(", ", ConvertArrayToString(firstMoments)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Inertia (WCS): [" + string.Join(", ", ConvertArrayToString(inertiaWCS)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Inertia (Centroid): [" + string.Join(", ", ConvertArrayToString(inertiaCentroid)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Spherical Moment: " + sphericalMoment.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Inertia Products (WCS): [" + string.Join(", ", ConvertArrayToString(inertiaProductsWCS)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Inertia Products (Centroid): [" + string.Join(", ", ConvertArrayToString(inertiaProductsCentroid)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Principal Axes:");
                for (int i = 0; i < 3; i++)
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("  Axis " + (i + 1) + ": [" + string.Join(", ", ConvertArrayToString(principalAxes[i])) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Principal Moments: [" + string.Join(", ", ConvertArrayToString(principalMoments)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Radii of Gyration (WCS): [" + string.Join(", ", ConvertArrayToString(radiiWCS)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Radii of Gyration (Centroid): [" + string.Join(", ", ConvertArrayToString(radiiCentroid)) + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Spherical Radius of Gyration: " + sphericalRadius.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Density: " + density.ToString("F5"));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("--------------------------------------------------");
            }
        }

        /// <summary>
        /// Assign unique BodyID (Bxxxxxxx) to all bodies in the current WorkPart that do not already have one.
        /// ProjectID (IPxxxxxx) is read from Part.info[0], or from System.csv.
        /// If System.csv is missing or invalid, the function stops with error.
        /// Each assigned BodyID is logged in System.csv as a separate line: Bxxxxxxx,IPxxxxxx.
        /// </summary>
        public static void WriteSystemIDToAllBodies()
        {
            string filePath = @"E:\dll0202\Shared\System.csv";
            var theSession = Core.SessionManager.Session; // ✅ lấy từ hệ thống quản lý
            var uf = Core.SessionManager.UFSession;
            var part = Core.SessionManager.WorkPart;

            // 1. Check file existence
            if (!File.Exists(filePath))
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Missing System.csv: " + filePath);
                return;
            }

            // 2. Read ProjectID from Part attribute: info[0]
            string projectId = "";
            bool hasProjectID = false;

            try
            {
                string value;
                bool found;
                uf.Attr.GetStringUserAttribute(part.Tag, "info", 0, out value, out found);
                if (found && !string.IsNullOrEmpty(value))
                {
                    projectId = value;
                    hasProjectID = true;
                }
            }
            catch { hasProjectID = false; }

            // 3. Read last line from System.csv
            string lastBodyId = "";
            string lastProjectId = "";

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                string[] last = lines[lines.Length - 1].Split(',');
                if (last.Length > 1)
                {
                    lastBodyId = last[0].Trim();
                    lastProjectId = last[1].Trim();
                }
            }

            // 4. If Part has no ProjectID → use from System.csv
            if (!hasProjectID)
            {
                if (!string.IsNullOrEmpty(lastProjectId))
                {
                    projectId = lastProjectId;

                    // Gán vào Part bằng AttributePropertiesBuilder
                    NXOpen.NXObject[] targets = new NXOpen.NXObject[1];
                    targets[0] = part;

                    var builder = theSession.AttributeManager.CreateAttributePropertiesBuilder(part, targets, NXOpen.AttributePropertiesBuilder.OperationType.None);
                    builder.Title = "info";
                    builder.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;
                    builder.IsArray = true;
                    builder.StringValue = projectId;
                    builder.SetAttributeObjects(targets);
                    builder.Commit();
                    builder.Destroy();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ ProjectID written to Part: " + projectId);
                }
                else
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ No ProjectID found in Part or System.csv → Cannot proceed.");
                    return;
                }
            }
            else
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("ℹ Using ProjectID from Part: " + projectId);
            }

            // 5. Validate lastBodyId
            if (string.IsNullOrEmpty(lastBodyId))
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Missing last BodyID in System.csv → Cannot determine next ID.");
                return;
            }

            int lastNumber = int.Parse(lastBodyId.Substring(1));
            int nextNumber = lastNumber + 1;

            // 6. Assign BodyIDs
            int count = 0;
            List<string> newLines = new List<string>();

            foreach (NXOpen.Body nxBody in part.Bodies)
            {
                if (nxBody == null || nxBody.Tag == NXOpen.Tag.Null) continue;

                var body = new Dll0202App.Objects.Body(nxBody.Tag);
                string[] infoAttr = body.GetObjectAttributes("info");

                bool hasBodyID = infoAttr.Length > 0 && !string.IsNullOrEmpty(infoAttr[0]);
                if (hasBodyID)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ Skipped: Body already has ID → " + infoAttr[0]);
                    continue;
                }

                string newBodyId = "B" + nextNumber.ToString("D7");
                body.SetObjectAttribute("info", 0, newBodyId);

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✔ Assigned " + newBodyId + " → Body Tag: " + (uint)body.Tag);
                newLines.Add(newBodyId + "," + projectId);

                nextNumber++;
                count++;
            }

            // 7. Write each new assignment as separate line
            if (newLines.Count > 0)
            {
                File.AppendAllLines(filePath, newLines.ToArray());
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ System.csv updated with " + newLines.Count + " entries.");
            }
            else
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("ℹ No new bodies were assigned.");
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("🎯 Total bodies assigned: " + count);
        }

        /// <summary>
        /// Clears all attribute values under "info" array (index 0 to 20) for selected bodies.
        /// This effectively resets BodyID, ProjectID, and other system tags.
        /// </summary>
        public static void ClearSystemIDToSelectionBodies()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody(); // returns Dll0202App.Objects.Body[]

            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            int clearedCount = 0;

            foreach (var body in bodies) // type: Dll0202App.Objects.Body
            {
                if (body == null || body.Tag == NXOpen.Tag.Null) continue;

                for (int i = 0; i <= 20; i++)
                {
                    body.SetObjectAttribute("info", i, "");
                }

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("🧹 Cleared info[0–20] → Body Tag: " + (uint)body.Tag);
                clearedCount++;
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Cleared system attributes on " + clearedCount + " bodies.");
        }


        private static string[] ConvertArrayToString(double[] arr)
        {
            string[] result = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                result[i] = arr[i].ToString("F4");
            return result;
        }

        /// <summary>
        /// Get bounding box (LxWxH) of selected solid bodies using UF.
        /// </summary>
        public static void GetBoundingBoxOfSelectionBody()
        {
            var bodies = SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No bodies selected.");
                return;
            }

            int count = 0;

            foreach (var body in bodies)
            {
                try
                {
                    double[] box = body.AskBoundingBox();

                    double length = Math.Abs(box[3] - box[0]);
                    double width = Math.Abs(box[4] - box[1]);
                    double height = Math.Abs(box[5] - box[2]);

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("🔷 Body #{0} | Tag: {1}", ++count, (uint)body.Tag));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   Min Corner: ({0:F3}, {1:F3}, {2:F3})", box[0], box[1], box[2]));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   Max Corner: ({0:F3}, {1:F3}, {2:F3})", box[3], box[4], box[5]));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   Bounding Box (LxWxH): {0:F3} x {1:F3} x {2:F3} mm", length, width, height));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("--------------------------------------------------");
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Failed to get bounding box of body " + (uint)body.Tag + ": " + ex.Message);
                }
            }
        }

        public static void CreateFarestPlaneInDirectionOfSelectionBody()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0) return;

            var body = bodies[0];
            var faces = body.GetFaces();

            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            System.Collections.Generic.List<Tag> faceTags = new System.Collections.Generic.List<Tag>();
            System.Collections.Generic.List<double[]> centers = new System.Collections.Generic.List<double[]>();
            System.Collections.Generic.List<double[]> normals = new System.Collections.Generic.List<double[]>();

            // B1: Duyệt mặt, chỉ lấy mặt phẳng và mặt trụ (data[0] == 22 hoặc 16)
            foreach (var face in faces)
            {
                double[] data = face.AskFaceDataRaw();

                //if (data[0] != 22 && data[0] != 16)
                if (data[0] != 22)
                    continue; // chỉ lấy mặt phẳng hoặc trụ

                if (data[4] == 0 && data[5] == 0 && data[6] == 0)
                    continue; // bỏ mặt không có normal

                faceTags.Add(face.Tag);
                centers.Add(new double[] { data[1], data[2], data[3] });
                normals.Add(new double[] { data[4], data[5], data[6] });
            }

            // B2: Gom nhóm các mặt có hướng normal gần giống nhau
            System.Collections.Generic.List<System.Collections.Generic.List<int>> groups = new System.Collections.Generic.List<System.Collections.Generic.List<int>>();
            double dotThreshold = 0.999;

            for (int i = 0; i < normals.Count; i++)
            {
                bool added = false;
                for (int j = 0; j < groups.Count; j++)
                {
                    int refIndex = groups[j][0];
                    if (Dot(normals[i], normals[refIndex]) > dotThreshold)
                    {
                        groups[j].Add(i);
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    var newGroup = new System.Collections.Generic.List<int>();
                    newGroup.Add(i);
                    groups.Add(newGroup);
                }
            }

            // B3: Trên mỗi nhóm, tìm mặt xa nhất và tạo Datum Plane
            foreach (var group in groups)
            {
                int farestIndex = -1;
                double maxDistance = double.MinValue;

                foreach (int idx in group)
                {
                    double d = Dot(centers[idx], normals[idx]);
                    if (d > maxDistance)
                    {
                        maxDistance = d;
                        farestIndex = idx;
                    }
                }

                if (farestIndex >= 0)
                {
                    NXOpen.Point3d centerPt = new NXOpen.Point3d(centers[farestIndex][0], centers[farestIndex][1], centers[farestIndex][2]);
                    NXOpen.Vector3d normalVec = new NXOpen.Vector3d(normals[farestIndex][0], normals[farestIndex][1], normals[farestIndex][2]);

                    if (normalVec.X == 0 && normalVec.Y == 0 && normalVec.Z == 0)
                    {
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                            string.Format("⚠ Bỏ qua mặt {0} vì vector normal = 0", (uint)faceTags[farestIndex])
                        );
                        continue;
                    }

                    // Tạo Datum Plane kiểu PointDir
                    NXOpen.Features.DatumPlaneBuilder datumPlaneBuilder = workPart.Features.CreateDatumPlaneBuilder(null);
                    NXOpen.Plane plane = datumPlaneBuilder.GetPlane();
                    plane.SetUpdateOption(NXOpen.SmartObject.UpdateOption.WithinModeling);
                    plane.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

                    NXOpen.Point point = workPart.Points.CreatePoint(centerPt);
                    NXOpen.Direction direction = workPart.Directions.CreateDirection(centerPt, normalVec, NXOpen.SmartObject.UpdateOption.WithinModeling);

                    NXOpen.NXObject[] geom = new NXOpen.NXObject[2];
                    geom[0] = point;
                    geom[1] = direction;
                    plane.SetGeometry(geom);

                    NXOpen.Features.Feature datumPlaneFeature = datumPlaneBuilder.CommitFeature();
                    datumPlaneBuilder.Destroy();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("📄 Created Datum Plane at Face {0}", (uint)faceTags[farestIndex])
                    );
                }
            }
        }


        /// <summary>
        /// Tính toán hệ XYZ
        /// Gom mặt => Kiểm tra theo Face lớn, Pricial => Chọn hướng Z+ , X+
        /// </summary>
        /// <param name="body"></param>
        /// <param name="centerMass"></param>
        /// <param name="xVec"></param>
        /// <param name="yVec"></param>
        /// <param name="zVec"></param>
        public static void AnalyzeBodyMainFaceDirections(Dll0202App.Objects.Body body, out double[] centerMass, out double[] xVec, out double[] yVec, out double[] zVec)
        {
            var workPart = Dll0202App.Core.SessionManager.WorkPart;

            double[] massProps = body.UFAskMassProps3D();
            centerMass = new double[] { massProps[3] * 25.4, massProps[4] * 25.4, massProps[5] * 25.4 };

            double[] xPrincipal = new double[] { massProps[22], massProps[23], massProps[24] };
            double[] yPrincipal = new double[] { massProps[25], massProps[26], massProps[27] };
            double[] zPrincipal = new double[] { massProps[28], massProps[29], massProps[30] };

            var faces = body.GetFaces();
            List<double[]> centers = new List<double[]>();
            List<double[]> planeNormals = new List<double[]>();   // Mặt phẳng
            List<double[]> cylinderAxes = new List<double[]>();   // Mặt trụ
            List<double[]> bboxMins = new List<double[]>();
            List<double[]> bboxMaxs = new List<double[]>();

            for (int i = 0; i < faces.Length; i++)
            {
                double[] data = faces[i].AskFaceDataRaw();
                int faceType = (int)data[0];

                if (faceType == 22) // Plane
                {
                    if (Math.Abs(data[4]) < 1e-6 && Math.Abs(data[5]) < 1e-6 && Math.Abs(data[6]) < 1e-6) continue;

                    centers.Add(new double[] { data[1], data[2], data[3] });
                    planeNormals.Add(new double[] { data[4], data[5], data[6] });
                    bboxMins.Add(new double[] { data[7], data[8], data[9] });
                    bboxMaxs.Add(new double[] { data[10], data[11], data[12] });
                }
                //else if (faceType == 16) // Cylinder
                //{
                //    if (Math.Abs(data[4]) < 1e-6 && Math.Abs(data[5]) < 1e-6 && Math.Abs(data[6]) < 1e-6) continue;

                //    cylinderAxes.Add(new double[] { data[4], data[5], data[6] });
                //}
            }

            if (centers.Count == 0)
            {
                // ❗ Không có mặt phẳng thì fallback massProps
                xVec = Normalize(xPrincipal);
                yVec = Normalize(yPrincipal);
                zVec = Normalize(zPrincipal);
                return;
            }

            // 🔥 Gộp planeNormals + cylinderAxes chỉ để Adjust Z
            List<double[]> allDirections = new List<double[]>();
            allDirections.AddRange(planeNormals);
            allDirections.AddRange(cylinderAxes);

            // 🔥 Gom nhóm hướng mặt (chỉ dùng planeNormals)
            List<double[]> mainDirs = GroupMainDirections(planeNormals, 0.996);

            // 🔥 Tìm hướng Z
            double[] z0 = FindClosestDirection(zPrincipal, mainDirs);
            double[] x0 = FindClosestDirection(xPrincipal, mainDirs);
            double[] y0 = Normalize(Cross(z0, x0));

            double[] zAxis = SelectBestZAxis(centers, planeNormals, bboxMins, bboxMaxs, new List<double[]> { x0, y0, z0 });

            if (zAxis == null || Length(zAxis) < 1e-6)
            {
                zAxis = Normalize(zPrincipal); // fallback
            }
            else
            {
                zAxis = AdjustDirectionByMajority(allDirections, zAxis); // 🔥 Sửa hướng Z+ dựa trên plane + cylinder
            }

            // 🔥 Tìm hướng X
            double[] xAxis = SelectBestXAxis(centers, planeNormals, bboxMins, bboxMaxs, x0, y0, zAxis);

            if (xAxis == null || Length(xAxis) < 1e-6)
            {
                xAxis = Normalize(xPrincipal); // fallback
            }
            else
            {
                xAxis = Normalize(AdjustDirectionByMajority(planeNormals, xAxis)); // 🔥 Chỉ adjust X theo planeNormals
            }

            // 🔥 Kiểm tra Orthogonalize nếu X gần song song Z
            xAxis = Orthogonalize(xAxis, zAxis);
            if (Length(xAxis) < 1e-6 || Math.Abs(Dot(xAxis, zAxis)) > 0.995)
            {
                xAxis = BuildAnyPerpendicularVector(zAxis);
            }
            xAxis = Normalize(xAxis);

            double[] yAxis = Normalize(Cross(zAxis, xAxis));

            xVec = xAxis;
            yVec = yAxis;
            zVec = zAxis;
        }

        #region Hàm phụ CSYS
        private static int FindLargestFace(List<double[]> centers, List<double[]> mins, List<double[]> maxs)
        {
            double maxSize = -1;
            int bestIndex = -1;

            for (int i = 0; i < centers.Count; i++)
            {
                double[] delta = new double[] { maxs[i][0] - mins[i][0], maxs[i][1] - mins[i][1], maxs[i][2] - mins[i][2] };
                double size = Length(delta);
                if (size > maxSize)
                {
                    maxSize = size;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static int FindFaceMostOrthogonalToZ(List<double[]> centers, List<double[]> normals, List<double[]> mins, List<double[]> maxs, double[] zAxis, int skipIndex)
        {
            double bestScore = -1;
            int bestIndex = -1;

            for (int i = 0; i < centers.Count; i++)
            {
                if (i == skipIndex) continue; // bỏ qua mặt đã chọn làm Z

                double dot = Math.Abs(Dot(normals[i], zAxis));
                if (dot < 0.2) // khá vuông góc
                {
                    double[] delta = new double[] { maxs[i][0] - mins[i][0], maxs[i][1] - mins[i][1], maxs[i][2] - mins[i][2] };
                    double size = Length(delta);

                    if (size > bestScore)
                    {
                        bestScore = size;
                        bestIndex = i;
                    }
                }
            }

            return bestIndex;
        }

        private static double[] SelectBestZAxis(List<double[]> centers, List<double[]> normals, List<double[]> bboxMins, List<double[]> bboxMaxs, List<double[]> baseDirs)
        {
            double[] bestNormal = null;
            double maxDist = -1;

            for (int i = 0; i < centers.Count; i++)
            {
                double[] normal = Normalize(normals[i]);

                foreach (var dir in baseDirs)
                {
                    if (Math.Abs(Dot(normal, dir)) > 0.9)
                    {
                        double[] minProj = ProjectPointToPlane(bboxMins[i], centers[i], normal);
                        double[] maxProj = ProjectPointToPlane(bboxMaxs[i], centers[i], normal);
                        double dist = Length(new double[] { maxProj[0] - minProj[0], maxProj[1] - minProj[1], maxProj[2] - minProj[2] });

                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            bestNormal = normal;
                        }
                    }
                }
            }

            return bestNormal;
        }

        private static double[] SelectBestXAxis(List<double[]> centers, List<double[]> normals, List<double[]> bboxMins, List<double[]> bboxMaxs, double[] x0, double[] y0, double[] zVec)
        {
            double[] bestNormal = null;
            double maxDist = -1;

            for (int i = 0; i < centers.Count; i++)
            {
                double[] normal = Normalize(normals[i]);
                if (Math.Abs(Dot(normal, zVec)) > 0.01) continue;

                double alignX = Math.Abs(Dot(normal, x0));
                double alignY = Math.Abs(Dot(normal, y0));

                if (alignX < 0.7 && alignY < 0.7) continue;

                double[] minProj = ProjectPointToPlane(bboxMins[i], centers[i], normal);
                double[] maxProj = ProjectPointToPlane(bboxMaxs[i], centers[i], normal);
                double dist = Length(new double[] { maxProj[0] - minProj[0], maxProj[1] - minProj[1], maxProj[2] - minProj[2] });

                if (dist > maxDist)
                {
                    maxDist = dist;
                    bestNormal = normal;
                }
            }

            return bestNormal;
        }

        private static double[] AdjustDirectionByMajority(List<double[]> normals, double[] axis)
        {
            int countPositive = 0, countNegative = 0;
            foreach (var normal in normals)
            {
                double dot = Dot(normal, axis);
                if (dot > 0.7) countPositive++;
                else if (dot < -0.7) countNegative++;
            }
            if (countNegative > countPositive)
            {
                axis = new double[] { -axis[0], -axis[1], -axis[2] };
            }
            return axis;
        }

        private static double[] ProjectPointToPlane(double[] p, double[] center, double[] normal)
        {
            double[] v = new double[] { p[0] - center[0], p[1] - center[1], p[2] - center[2] };
            double d = Dot(v, normal);
            return new double[]
    {
        p[0] - d * normal[0],
        p[1] - d * normal[1],
        p[2] - d * normal[2]
    };
        }



        #endregion

        #region Hàm phụ Math
        private static double[] Cross(double[] a, double[] b)
        {
            return new double[]
    {
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0]
    };
        }

        private static double Dot(double[] a, double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }

        private static double Length(double[] v)
        {
            return Math.Sqrt(Dot(v, v));
        }

        private static double[] Normalize(double[] v)
        {
            double len = Length(v);
            if (len < 1e-6) return new double[] { 1, 0, 0 };
            return new double[] { v[0] / len, v[1] / len, v[2] / len };
        }

        private static double[] Orthogonalize(double[] vec, double[] refVec)
        {
            double dot = Dot(vec, refVec);
            return Normalize(new double[] {
        vec[0] - dot * refVec[0],
        vec[1] - dot * refVec[1],
        vec[2] - dot * refVec[2]
    });
        }

        private static double[] BuildAnyPerpendicularVector(double[] zVec)
        {
            // Nếu Z gần trùng với trục X, chọn Y làm chuẩn
            if (Math.Abs(zVec[0]) < Math.Abs(zVec[1]) && Math.Abs(zVec[0]) < Math.Abs(zVec[2]))
            {
                return Normalize(Cross(zVec, new double[] { 1, 0, 0 }));
            }
            else
            {
                return Normalize(Cross(zVec, new double[] { 0, 1, 0 }));
            }
        }
        #endregion

        //Bounding + Gom csys theo Group face
        #region Dựng Gốc theo Pricipal + Normal và Plane Bound
        public static void CreateBoundingPlanesAndCsysOfSelectionBody()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0) return;

            var body = bodies[0];
            var workPart = Dll0202App.Core.SessionManager.WorkPart;

            double[] massProps = body.UFAskMassProps3D();

            double[] xPrincipal = new double[] { massProps[22], massProps[23], massProps[24] };
            double[] yPrincipal = new double[] { massProps[25], massProps[26], massProps[27] };
            double[] zPrincipal = new double[] { massProps[28], massProps[29], massProps[30] };

            double[] centerMass = new double[] { massProps[3] * 25.4, massProps[4] * 25.4, massProps[5] * 25.4 };

            var faces = body.GetFaces();
            List<Tag> faceTags = new List<Tag>();
            List<double[]> centers = new List<double[]>();
            List<double[]> normals = new List<double[]>();

            foreach (var face in faces)
            {
                double[] data = face.AskFaceDataRaw();
                if (data[0] != 22) continue; // chỉ lấy mặt phẳng
                if (Math.Abs(data[4]) < 1e-6 && Math.Abs(data[5]) < 1e-6 && Math.Abs(data[6]) < 1e-6) continue;

                faceTags.Add(face.Tag);
                centers.Add(new double[] { data[1], data[2], data[3] });
                normals.Add(new double[] { data[4], data[5], data[6] });
            }

            if (centers.Count == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không có mặt phẳng nào.");
                return;
            }

            // 🔥 Gom nhóm hướng mặt với độ chặt 0.996 (lệch tối đa ~5°)
            List<double[]> mainDirs = GroupMainDirections(normals, 0.996);

            // 🔥 Tìm hướng Z gần principal
            double[] zVec = FindClosestDirection(zPrincipal, mainDirs);
            double[] xVec = FindClosestDirection(xPrincipal, mainDirs);

            // 🔥 Orthogonalize X ⊥ Z
            xVec = Orthogonalize(xVec, zVec);
            if (Length(xVec) < 1e-6)
            {
                xVec = BuildAnyPerpendicularVector(zVec);
            }
            xVec = Normalize(xVec);

            double[] yVec = Normalize(Cross(zVec, xVec));
            zVec = Normalize(zVec);

            // ⚡ Nếu X gần song song Z thì Build lại
            if (Math.Abs(Dot(xVec, zVec)) > 0.995)
            {
                xVec = BuildAnyPerpendicularVector(zVec);
                yVec = Normalize(Cross(zVec, xVec));
            }

            // 🔥 Tìm các mặt bao ngoài
            int facePosX = FindFarestFace(centers, xVec, true);
            int faceNegX = FindFarestFace(centers, xVec, false);
            int facePosY = FindFarestFace(centers, yVec, true);
            int faceNegY = FindFarestFace(centers, yVec, false);
            int facePosZ = FindFarestFace(centers, zVec, true);
            int faceNegZ = FindFarestFace(centers, zVec, false);

            List<int> finalFaces = new List<int>();
            if (facePosX != -1) finalFaces.Add(facePosX);
            if (faceNegX != -1) finalFaces.Add(faceNegX);
            if (facePosY != -1) finalFaces.Add(facePosY);
            if (faceNegY != -1) finalFaces.Add(faceNegY);
            if (facePosZ != -1) finalFaces.Add(facePosZ);
            if (faceNegZ != -1) finalFaces.Add(faceNegZ);

            foreach (int idx in finalFaces)
            {
                CreateDatumPlane(workPart, centers[idx], normals[idx]);
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📄 Created Datum Plane at Face " + faceTags[idx].ToString());
            }

            // 🔥 Cuối cùng, tạo hệ tọa độ CSYS
            CreateCsys(workPart, centerMass, xVec, yVec, zVec);
        }


        //private static List<double[]> GroupMainDirections(List<double[]> normals)
        //{
        //    List<double[]> groups = new List<double[]>();

        //    foreach (var n in normals)
        //    {
        //        bool added = false;
        //        foreach (var g in groups)
        //        {
        //            if (Math.Abs(Dot(n, g)) > 0.99)
        //            {
        //                added = true;
        //                break;
        //            }
        //        }
        //        if (!added)
        //        {
        //            groups.Add(Normalize(n));
        //        }
        //    }

        //    return groups;
        //}

        private static List<double[]> GroupMainDirections(List<double[]> normals, double dotThreshold)
        {
            List<double[]> groups = new List<double[]>();

            foreach (var n in normals)
            {
                bool added = false;
                foreach (var g in groups)
                {
                    if (Math.Abs(Dot(n, g)) > dotThreshold) // Gom theo ngưỡng
                    {
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    groups.Add(Normalize(n));
                }
            }

            return groups;
        }


        private static double[] FindClosestDirection(double[] target, List<double[]> candidates)
        {
            double maxDot = -1;
            double[] best = null;
            foreach (var dir in candidates)
            {
                double d = Math.Abs(Dot(target, dir));
                if (d > maxDot)
                {
                    maxDot = d;
                    best = dir;
                }
            }
            return best ?? Normalize(target);
        }

        private static int FindFarestFace(List<double[]> centers, double[] direction, bool positive)
        {
            int bestIdx = -1;
            double bestValue = positive ? double.MinValue : double.MaxValue;
            for (int i = 0; i < centers.Count; i++)
            {
                double proj = Dot(centers[i], direction);
                if ((positive && proj > bestValue) || (!positive && proj < bestValue))
                {
                    bestValue = proj;
                    bestIdx = i;
                }
            }
            return bestIdx;
        }

        private static void CreateDatumPlane(NXOpen.Part workPart, double[] center, double[] normal)
        {
            if (normal[0] == 0 && normal[1] == 0 && normal[2] == 0) return;

            var builder = workPart.Features.CreateDatumPlaneBuilder(null);
            var plane = builder.GetPlane();
            plane.SetUpdateOption(NXOpen.SmartObject.UpdateOption.WithinModeling);
            plane.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

            var pt = new NXOpen.Point3d(center[0], center[1], center[2]);
            var dir = new NXOpen.Vector3d(normal[0], normal[1], normal[2]);

            var point = workPart.Points.CreatePoint(pt);
            var direction = workPart.Directions.CreateDirection(pt, dir, NXOpen.SmartObject.UpdateOption.WithinModeling);

            plane.SetGeometry(new NXOpen.NXObject[] { point, direction });
            builder.CommitFeature();
            builder.Destroy();
        }

        private static void CreateCsys(NXOpen.Part workPart, double[] centerMass, double[] xVec, double[] yVec, double[] zVec)
        {
            var originPt = new NXOpen.Point3d(centerMass[0], centerMass[1], centerMass[2]);
            var originPoint = workPart.Points.CreatePoint(originPt);

            var xDirection = workPart.Directions.CreateDirection(originPt,
                new NXOpen.Vector3d(xVec[0], xVec[1], xVec[2]), NXOpen.SmartObject.UpdateOption.WithinModeling);

            var zDirection = workPart.Directions.CreateDirection(originPt,
                new NXOpen.Vector3d(zVec[0], zVec[1], zVec[2]), NXOpen.SmartObject.UpdateOption.WithinModeling);

            var xform = workPart.Xforms.CreateXformByPointXDirZDir(
                originPoint, xDirection, zDirection,
                NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0
            );

            var csys = workPart.CoordinateSystems.CreateCoordinateSystem(
                xform, NXOpen.SmartObject.UpdateOption.WithinModeling
            );

            xform.RemoveParameters();

            var datumCsysBuilder = workPart.Features.CreateDatumCsysBuilder(null);
            datumCsysBuilder.Csys = csys;
            datumCsysBuilder.DisplayScaleFactor = 1.0;
            datumCsysBuilder.CommitFeature();
            datumCsysBuilder.Destroy();
        }
        #endregion

        //Body Csys theo MAin FAce
        #region Dựng gốc theo hướng gom nhóm normal + gần principal

        public static void CreateBodyCsysbyMainFaceofSelectionBody()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0) return;

            var body = bodies[0];
            var workPart = Dll0202App.Core.SessionManager.WorkPart;

            double[] center, xVec, yVec, zVec;

            // ✅ Gọi luôn hàm phân tích đã fallback
            AnalyzeBodyMainFaceDirections(body, out center, out xVec, out yVec, out zVec);

            // ✅ Dựng CSYS tại center, theo X, Y, Z
            CreateCsys(workPart, center, xVec, yVec, zVec);

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Đã tạo hệ tọa độ cho Body theo hướng chính.");
        }

        #endregion

        //Di chuyển chi tiết về gốc chuẩn
        public static void MoveToOriginSelectionBodies()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ Không có body nào được chọn.");
                return;
            }

            var workPart = Dll0202App.Core.SessionManager.WorkPart;

            foreach (var body in bodies)
            {
                try
                {
                    double[] center, xVec, yVec, zVec;
                    AnalyzeBodyMainFaceDirections(body, out center, out xVec, out yVec, out zVec);

                    NXOpen.Point3d origin = new NXOpen.Point3d(center[0], center[1], center[2]);
                    NXOpen.Vector3d vx = new NXOpen.Vector3d(xVec[0], xVec[1], xVec[2]);
                    NXOpen.Vector3d vz = new NXOpen.Vector3d(zVec[0], zVec[1], zVec[2]);

                    NXOpen.Point point = workPart.Points.CreatePoint(origin);
                    NXOpen.Direction dirX = workPart.Directions.CreateDirection(origin, vx, NXOpen.SmartObject.UpdateOption.WithinModeling);
                    NXOpen.Direction dirZ = workPart.Directions.CreateDirection(origin, vz, NXOpen.SmartObject.UpdateOption.WithinModeling);

                    NXOpen.Xform xform = workPart.Xforms.CreateXformByPointXDirZDir(
                        point, dirX, dirZ, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0);

                    NXOpen.CartesianCoordinateSystem fromCsys = workPart.CoordinateSystems.CreateCoordinateSystem(xform, NXOpen.SmartObject.UpdateOption.WithinModeling);
                    NXOpen.CartesianCoordinateSystem toCsys = workPart.WCS.CoordinateSystem;

                    NXOpen.Features.MoveObjectBuilder mover = workPart.BaseFeatures.CreateMoveObjectBuilder(null);
                    mover.TransformMotion.Option = NXOpen.GeometricUtilities.ModlMotion.Options.CsysToCsys;
                    mover.TransformMotion.FromCsys = fromCsys;
                    mover.TransformMotion.ToCsys = toCsys;
                    mover.ObjectToMoveObject.Add(body.GetNativeBody());

                    mover.Commit();
                    mover.Destroy();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("✔ Moved Body Tag {0} về Origin.", (uint)body.Tag)
                    );
                }
                catch (System.Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("⚠ Bỏ qua Body {0}: {1}", (uint)body.Tag, ex.Message)
                    );
                    continue; // 👉 Quan trọng: bỏ qua body lỗi, move tiếp body sau
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Đã hoàn tất Move các Body hợp lệ về Origin.");
        }

        public static void WriteInfomationOfSeletionBody()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ Không có body nào được chọn.");
                return;
            }

            foreach (var body in bodies)
            {
                double[] center, xVec, yVec, zVec;
                AnalyzeBodyMainFaceDirections(body, out center, out xVec, out yVec, out zVec);

                string[] existingAttributes = body.GetObjectAttributes("info");
                int count = existingAttributes != null ? existingAttributes.Length : 0;

                try
                {
                    // 1. Fill trắng từ count đến 19 nếu thiếu
                    for (int i = count; i <= 19; i++)
                    {
                        body.SetObjectAttribute("info", i, " ");
                    }

                    // 2. Ghi center, xVec, yVec, zVec vào 20,21,22,23
                    body.SetObjectAttribute("info", 20, string.Format("{0:F4},{1:F4},{2:F4}", center[0], center[1], center[2]));
                    body.SetObjectAttribute("info", 21, string.Format("{0:F6},{1:F6},{2:F6}", xVec[0], xVec[1], xVec[2]));
                    body.SetObjectAttribute("info", 22, string.Format("{0:F6},{1:F6},{2:F6}", yVec[0], yVec[1], yVec[2]));
                    body.SetObjectAttribute("info", 23, string.Format("{0:F6},{1:F6},{2:F6}", zVec[0], zVec[1], zVec[2]));

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("✔ Ghi Body Info vào Body {0} (info[20..23])", (uint)body.Tag)
                    );
                }
                catch (System.Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("❌ Lỗi khi ghi Body {0}: {1}", (uint)body.Tag, ex.Message)
                    );
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Đã hoàn tất ghi thông tin tọa độ Body vào Attributes.");
        }

        //Kiểm tra chi tiết trục
        #region Check shaft
        public static void CheckShaftTypeOfSelectionBodies()
        {
            NXOpen.Session session = Dll0202App.Core.SessionManager.Session;
            NXOpen.UF.UFSession uf = Dll0202App.Core.SessionManager.UFSession;
            NXOpen.UI ui = Dll0202App.Core.SessionManager.UI;

            Dll0202App.Objects.Body[] bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không chọn được body.");
                return;
            }

            foreach (Dll0202App.Objects.Body body in bodies)
            {
                NXOpen.Tag[] faceList;
                uf.Modl.AskBodyFaces(body.Tag, out faceList);

                // Biến tìm trụ lớn nhất
                NXOpen.Tag bestFaceTag = NXOpen.Tag.Null;
                double bestRadius = -1.0;
                double[] bestCenter = null;
                double[] bestDirection = null;

                foreach (NXOpen.Tag faceTag in faceList)
                {
                    Dll0202App.Objects.Face face = new Dll0202App.Objects.Face(faceTag);
                    double[] data = face.AskFaceDataRaw();

                    int faceType = (int)data[0];
                    double[] center = new double[] { data[1], data[2], data[3] };
                    double[] dir = new double[] { data[4], data[5], data[6] };
                    double radius = data[13];
                    int normDir = (int)data[15];

                    if (faceType == 16 && normDir == 1 && radius > 0.5)
                    {
                        if (bestFaceTag == NXOpen.Tag.Null || radius > bestRadius)
                        {
                            bestFaceTag = faceTag;
                            bestRadius = radius;
                            bestCenter = center;
                            bestDirection = dir;
                        }
                    }
                }

                if (bestFaceTag == NXOpen.Tag.Null)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không có mặt trụ ngoài hợp lệ.");
                    continue;
                }

                Dll0202App.Objects.Edge[] edges = body.GetEdges();
                bool valid = true;
                double tol = 0.01;

                foreach (Dll0202App.Objects.Edge edge in edges)
                {
                    NXOpen.Edge nativeEdge = edge.GetNativeEdge();
                    NXOpen.Point3d start, end;
                    nativeEdge.GetVertices(out start, out end);

                    if (!PointInsideTrunk(start, bestCenter, bestDirection, bestRadius, tol) ||
                        !PointInsideTrunk(end, bestCenter, bestDirection, bestRadius, tol))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    double length = GetTrunkLengthFromVertices(edges, bestCenter, bestDirection);

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✔ Dạng trục: Có");
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Mặt trụ ngoài: Tag = " + bestFaceTag);
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Bán kính: " + bestRadius.ToString("F3") + " mm");
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Chiều dài trục: " + length.ToString("F3") + " mm");
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("Tâm: X=" + bestCenter[0].ToString("F3") +
                                                                       ", Y=" + bestCenter[1].ToString("F3") +
                                                                       ", Z=" + bestCenter[2].ToString("F3"));
                    body.SetColor(186);

                    // 🔥 Tô luôn màu mặt trụ ngoài
                    Dll0202App.Objects.Face bestFace = new Dll0202App.Objects.Face(bestFaceTag);
                    bestFace.SetColor(186);

                }
                else
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Dạng trục: Không (có điểm vượt ngoài mặt trụ ngoài)");
                }
            }
        }

        private static bool PointInsideTrunk(NXOpen.Point3d pt, double[] origin, double[] dir, double R, double tol)
        {
            double[] P = new double[] { pt.X, pt.Y, pt.Z };
            double[] OP = new double[] { P[0] - origin[0], P[1] - origin[1], P[2] - origin[2] };

            double[] cross = new double[]
            {
                OP[1] * dir[2] - OP[2] * dir[1],
                OP[2] * dir[0] - OP[0] * dir[2],
                OP[0] * dir[1] - OP[1] * dir[0]
            };

            double crossLen = System.Math.Sqrt(cross[0] * cross[0] + cross[1] * cross[1] + cross[2] * cross[2]);
            double dirLen = System.Math.Sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);

            double dist = crossLen / dirLen;
            return dist <= R + tol;
        }

        private static double GetTrunkLengthFromVertices(Dll0202App.Objects.Edge[] edges, double[] origin, double[] dir)
        {
            double dirLen = System.Math.Sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);
            double[] Z = new double[] { dir[0] / dirLen, dir[1] / dirLen, dir[2] / dirLen };

            double minT = double.MaxValue;
            double maxT = double.MinValue;

            foreach (Dll0202App.Objects.Edge edge in edges)
            {
                NXOpen.Edge nativeEdge = edge.GetNativeEdge();
                NXOpen.Point3d p1, p2;
                nativeEdge.GetVertices(out p1, out p2);

                NXOpen.Point3d[] points = new NXOpen.Point3d[] { p1, p2 };
                foreach (NXOpen.Point3d pt in points)
                {
                    double[] P = new double[] { pt.X, pt.Y, pt.Z };
                    double[] OP = new double[] { P[0] - origin[0], P[1] - origin[1], P[2] - origin[2] };

                    double t = OP[0] * Z[0] + OP[1] * Z[1] + OP[2] * Z[2];
                    if (t < minT) minT = t;
                    if (t > maxT) maxT = t;
                }
            }

            return System.Math.Abs(maxT - minT);
        }
        #endregion

        //Gom mặt phẳng, trụ bị split

        #region Gom mặt bị split (plane và cylinder)
        public static void GroupSplitFacesIntoLogicalSurfaces()
        {
            var bodies = Dll0202App.Tools.SelectTool.SelectBody();
            if (bodies == null || bodies.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No body selected.");
                return;
            }

            for (int b = 0; b < bodies.Length; b++)
            {
                Dll0202App.Objects.Body body = bodies[b];
                Dll0202App.Objects.Face[] faces = body.GetFaces();

                if (faces == null || faces.Length == 0)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No faces found in body.");
                    continue;
                }

                System.Collections.Generic.List<Dll0202App.Objects.Face> planeFaces = new System.Collections.Generic.List<Dll0202App.Objects.Face>();
                System.Collections.Generic.List<Dll0202App.Objects.Face> cylinderFaces = new System.Collections.Generic.List<Dll0202App.Objects.Face>();

                for (int i = 0; i < faces.Length; i++)
                {
                    double[] data = faces[i].AskFaceDataRaw();
                    int type = (int)data[0];

                    if (type == 22) // Plane
                        planeFaces.Add(faces[i]);
                    else if (type == 16) // Cylinder
                        cylinderFaces.Add(faces[i]);
                }

                int planeGroups = GroupPlaneFaces(planeFaces);
                int cylinderGroups = GroupCylinderFaces(cylinderFaces);

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📦 Body [" + (uint)body.Tag + "]");
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   ➤ Plane Faces: {0} ➔ Plane Groups: {1}", planeFaces.Count, planeGroups));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   ➤ Cylinder Faces: {0} ➔ Cylinder Groups: {1}", cylinderFaces.Count, cylinderGroups));
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("--------------------------------------------------");
            }
        }

        private static int GroupPlaneFaces(System.Collections.Generic.List<Dll0202App.Objects.Face> faces)
        {
            System.Collections.Generic.List<System.Collections.Generic.List<Dll0202App.Objects.Face>> groups = new System.Collections.Generic.List<System.Collections.Generic.List<Dll0202App.Objects.Face>>();
            double angleTolerance = 1.0;         // độ lệch hướng cho phép (degree)
            double distanceTolerance = 0.01;     // độ lệch bậc cho phép (mm)

            for (int i = 0; i < faces.Count; i++)
            {
                double[] data1 = faces[i].AskFaceDataRaw();
                double[] dir1 = new double[3] { data1[4], data1[5], data1[6] };
                double[] point1 = new double[3] { data1[1], data1[2], data1[3] };

                bool added = false;

                for (int j = 0; j < groups.Count; j++)
                {
                    Dll0202App.Objects.Face repFace = groups[j][0];
                    double[] data2 = repFace.AskFaceDataRaw();
                    double[] dir2 = new double[3] { data2[4], data2[5], data2[6] };
                    double[] point2 = new double[3] { data2[1], data2[2], data2[3] };

                    double angle = GetAngleBetweenVectors(dir1, dir2);
                    if (angle > angleTolerance)
                        continue;

                    double distance = GetLevelDistance(point1, point2, dir2);
                    if (distance > distanceTolerance)
                        continue;

                    groups[j].Add(faces[i]);
                    added = true;
                    break;
                }

                if (!added)
                {
                    System.Collections.Generic.List<Dll0202App.Objects.Face> newGroup = new System.Collections.Generic.List<Dll0202App.Objects.Face>();
                    newGroup.Add(faces[i]);
                    groups.Add(newGroup);
                }
            }

            return groups.Count;
        }

        private static int GroupCylinderFaces(System.Collections.Generic.List<Dll0202App.Objects.Face> faces)
        {
            System.Collections.Generic.List<System.Collections.Generic.List<Dll0202App.Objects.Face>> groups = new System.Collections.Generic.List<System.Collections.Generic.List<Dll0202App.Objects.Face>>();
            double angleTolerance = 1.0;         // độ lệch hướng cho phép (degree)
            double radiusTolerance = 0.01;       // độ lệch bán kính cho phép (mm)

            for (int i = 0; i < faces.Count; i++)
            {
                double[] data1 = faces[i].AskFaceDataRaw();
                double[] dir1 = new double[3] { data1[4], data1[5], data1[6] };
                double radius1 = data1[13];

                bool added = false;

                for (int j = 0; j < groups.Count; j++)
                {
                    Dll0202App.Objects.Face repFace = groups[j][0];
                    double[] data2 = repFace.AskFaceDataRaw();
                    double[] dir2 = new double[3] { data2[4], data2[5], data2[6] };
                    double radius2 = data2[13];

                    double angle = GetAngleBetweenVectors(dir1, dir2);
                    if (angle > angleTolerance)
                        continue;

                    double radiusDiff = System.Math.Abs(radius1 - radius2);
                    if (radiusDiff > radiusTolerance)
                        continue;

                    groups[j].Add(faces[i]);
                    added = true;
                    break;
                }

                if (!added)
                {
                    System.Collections.Generic.List<Dll0202App.Objects.Face> newGroup = new System.Collections.Generic.List<Dll0202App.Objects.Face>();
                    newGroup.Add(faces[i]);
                    groups.Add(newGroup);
                }
            }

            return groups.Count;
        }

        private static double GetLevelDistance(double[] p1, double[] p2, double[] normal)
        {
            double dx = p2[0] - p1[0];
            double dy = p2[1] - p1[1];
            double dz = p2[2] - p1[2];
            double dot = dx * normal[0] + dy * normal[1] + dz * normal[2];
            return System.Math.Abs(dot);
        }

        private static double GetAngleBetweenVectors(double[] v1, double[] v2)
        {
            double dot = v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
            double norm1 = System.Math.Sqrt(v1[0] * v1[0] + v1[1] * v1[1] + v1[2] * v1[2]);
            double norm2 = System.Math.Sqrt(v2[0] * v2[0] + v2[1] * v2[1] + v2[2] * v2[2]);
            double cosTheta = dot / (norm1 * norm2);
            cosTheta = System.Math.Max(-1.0, System.Math.Min(1.0, cosTheta));
            double angleRad = System.Math.Acos(cosTheta);
            double angleDeg = angleRad * 180.0 / System.Math.PI;
            return angleDeg;
        }

        #endregion




    }

    public static class FaceTools
    {
        /// <summary>
        /// Returns the list of Tags (as strings) of all Faces selected from the UI,
        /// and logs them to the Listing.
        /// </summary>
        /// <returns>List of string representations of Face Tags</returns>
        public static List<string> GetTagOfSelectionFaces()
        {
            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No face was selected.");
                return new List<string>();
            }

            List<string> result = new List<string>();
            foreach (var face in faces)
            {
                string tagStr = face.Tag.ToString();
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("   Face Tag = " + tagStr);
                result.Add(tagStr);
            }

            return result;
        }

        /// <summary>
        /// Trả về mảng 16 giá trị mô tả hình học của Face từ hàm UF_MODL_ask_face_data:
        /// [0]  → Type: loại mặt (plane, cylinder, cone...),
        /// [1]  → X tọa độ điểm đặc trưng (thường là tâm hoặc điểm gần giữa mặt),
        /// [2]  → Y tọa độ điểm đặc trưng,
        /// [3]  → Z tọa độ điểm đặc trưng,
        /// [4]  → X thành phần vector hướng (normal hoặc trục),
        /// [5]  → Y thành phần vector hướng,
        /// [6]  → Z thành phần vector hướng,
        /// [7]  → Min X của bounding box,
        /// [8]  → Min Y của bounding box,
        /// [9]  → Min Z của bounding box,
        /// [10] → Max X của bounding box,
        /// [11] → Max Y của bounding box,
        /// [12] → Max Z của bounding box,
        /// [13] → Radius: bán kính nếu là mặt trụ, mặt cầu..., 0 nếu là mặt phẳng,
        /// [14] → RadData: dữ liệu phụ về bán kính (tùy thuộc loại mặt),
        /// [15] → NormDir: chiều hướng của pháp tuyến (1 hoặc -1)
        /// </summary>
        public static void GetDataRowOfSelectionFaces()
        {
            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No face selected.");
                return;
            }

            foreach (var face in faces)
            {
                try
                {
                    double[] data = face.AskFaceDataRaw();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📘 Face Tag: " + (uint)face.Tag);
                    for (int i = 0; i < data.Length; i++)
                    {
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   [{0}] = {1:F4}", i, data[i]));
                    }
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("--------------------------------------------------");
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Error with Face Tag " + (uint)face.Tag + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Outputs detailed information about edges of selected face(s):
        /// - Edge Tag
        /// - Type (Line, Arc, etc.)
        /// - Length
        /// - Start and End Points
        /// </summary>
        public static void GetEdgePropertiesOfSelectionFaces()
        {
            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No face selected.");
                return;
            }

            foreach (var face in faces)
            {
                try
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📘 Face Tag: " + (uint)face.Tag);

                    var edges = face.GetEdges();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("   ▪ Edge Count: " + edges.Length);
                    for (int i = 0; i < edges.Length; i++)
                    {
                        var edge = edges[i];

                        string edgeType = edge.SolidEdgeType.ToString();
                        double length = edge.GetLength();

                        Point3d p1, p2;
                        edge.GetVertices(out p1, out p2);

                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("     ➤ Edge {0}:", i + 1));
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("        ▪ Tag     : {0}", (uint)edge.Tag));
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("        ▪ Type    : {0}", edgeType));
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("        ▪ Length  : {0:F4}", length));
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("        ▪ Start   : ({0:F4}, {1:F4}, {2:F4})", p1.X, p1.Y, p1.Z));
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("        ▪ End     : ({0:F4}, {1:F4}, {2:F4})", p2.X, p2.Y, p2.Z));
                    }
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Error reading edges from Face " + (uint)face.Tag + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Analyze loops (closed edge sequences) of selected faces, by connecting edges via shared vertices.
        /// </summary>
        public static void GetLoopsOfSelectionFaces()
        {
            var faces = Dll0202App.Tools.SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No face selected.");
                return;
            }

            foreach (var nativeFace in faces)
            {
                try
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📘 Face Tag: " + (uint)nativeFace.Tag);

                    Dll0202App.Objects.Face myFace = new Dll0202App.Objects.Face(nativeFace.Tag);
                    Dll0202App.Objects.Edge[] edges = myFace.GetEdges();

                    // Initialize edge list and loop container
                    List<Dll0202App.Objects.Edge> unusedEdges = new List<Dll0202App.Objects.Edge>(edges);
                    List<List<Dll0202App.Objects.Edge>> loops = new List<List<Dll0202App.Objects.Edge>>();

                    while (unusedEdges.Count > 0)
                    {
                        var loop = new List<Dll0202App.Objects.Edge>();
                        var current = unusedEdges[0];
                        unusedEdges.RemoveAt(0);
                        loop.Add(current);

                        NXOpen.Point3d start, end;
                        current.GetVertices(out start, out end);

                        NXOpen.Point3d loopStart = start;
                        NXOpen.Point3d lastEnd = end;
                        bool loopClosed = false;

                        while (!loopClosed)
                        {
                            Dll0202App.Objects.Edge nextEdge = null;

                            for (int i = 0; i < unusedEdges.Count; i++)
                            {
                                Dll0202App.Objects.Edge e = unusedEdges[i];
                                NXOpen.Point3d p1, p2;
                                e.GetVertices(out p1, out p2);

                                if (Dll0202App.Tools.FaceGeometryHelper.ArePointsEqual(p1, lastEnd))
                                {
                                    nextEdge = e;
                                    lastEnd = p2;
                                    break;
                                }
                                else if (Dll0202App.Tools.FaceGeometryHelper.ArePointsEqual(p2, lastEnd))
                                {
                                    nextEdge = e;
                                    lastEnd = p1;
                                    break;
                                }
                            }

                            if (nextEdge != null)
                            {
                                loop.Add(nextEdge);
                                unusedEdges.Remove(nextEdge);

                                if (Dll0202App.Tools.FaceGeometryHelper.ArePointsEqual(lastEnd, loopStart))
                                    loopClosed = true;
                            }
                            else break;
                        }

                        loops.Add(loop);
                    }

                    // Output results for each loop
                    for (int i = 0; i < loops.Count; i++)
                    {
                        var loop = loops[i];
                        double totalLength = 0;
                        List<string> edgeLengths = new List<string>();

                        foreach (var edge in loop)
                        {
                            double len = edge.GetLength();
                            totalLength += len;
                            edgeLengths.Add(string.Format("{0:F4}", len));
                        }

                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format(
                            "🔄 Loop {0}: Total Length = {1:F4} mm → {2}",
                            i + 1,
                            totalLength,
                            string.Join(" - ", edgeLengths.ToArray())
                        ));
                    }

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("------------------------------------------------------");
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Error processing Face " + (uint)nativeFace.Tag + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Tính bounding rectangle (Width, Height, Area) của các mặt phẳng (planar) được chọn từ UI.
        /// </summary>
        public static void GetBoundingRectangleOfSelectionFace()
        {
            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ No face selected.");
                return;
            }

            foreach (var f in faces)
            {
                try
                {
                    var face = new Dll0202App.Objects.Face(f.Tag);
                    var data = face.AskFaceDataRaw();

                    int type = (int)data[0];
                    //if (type != 1) // chỉ xử lý mặt phẳng (type 1)
                    //{
                    //    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⏩ Skip non-planar face: " + (uint)face.Tag);
                    //    continue;
                    //}

                    double minX = data[7];
                    double minY = data[8];
                    double minZ = data[9];
                    double maxX = data[10];
                    double maxY = data[11];
                    double maxZ = data[12];

                    // Tính chiều rộng và chiều cao theo hướng pháp tuyến
                    double width = 0, height = 0;

                    double nx = data[4], ny = data[5], nz = data[6]; // vector pháp tuyến

                    if (Math.Abs(nz) == 1.0)      // mặt phẳng XY
                    {
                        width = Math.Abs(maxX - minX);
                        height = Math.Abs(maxY - minY);
                    }
                    else if (Math.Abs(nx) == 1.0) // mặt phẳng YZ
                    {
                        width = Math.Abs(maxY - minY);
                        height = Math.Abs(maxZ - minZ);
                    }
                    else if (Math.Abs(ny) == 1.0) // mặt phẳng ZX
                    {
                        width = Math.Abs(maxX - minX);
                        height = Math.Abs(maxZ - minZ);
                    }
                    else
                    {
                        Dll0202App.Shell.MainForm.Instance.ShowBotMessage("⚠ Cannot determine direction for Face Tag " + (uint)face.Tag);
                        continue;
                    }

                    double area = width * height;

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("📘 Face Tag: " + (uint)face.Tag);
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   ▪ Width  : {0:F4}", width));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   ▪ Height : {0:F4}", height));
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(string.Format("   ▪ Area   : {0:F4}", area));
                }
                catch (Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Error processing Face: " + ex.Message);
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Done calculating bounding rectangle of planar faces.");
        }

        public static void GetSamplingAllEdgesOfSelectionFace()
        {
            // Chọn các mặt từ giao diện
            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                //Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không có mặt nào được chọn.");
                return;
            }

            int total = 0;

            // Duyệt qua từng mặt được chọn
            foreach (var faceObj in faces)
            {
                var nxFace = NXOpen.Utilities.NXObjectManager.Get(faceObj.Tag) as NXOpen.Face;
                if (nxFace == null) continue;

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("▶ Face: " + faceObj.Tag);

                // Duyệt qua từng cạnh thuộc mặt
                foreach (Edge edge in nxFace.GetEdges())
                {
                    var pts = EdgeTools.EdgeSamplingTool.SamplePointsOnEdge(edge.Tag, 5);
                    EdgeTools.EdgeSamplingTool.CreatePointObjects(pts);

                    foreach (var p in pts)
                    {
                        //string msg = string.Format("   ✔ P({0:F2}, {1:F2}, {2:F2})", p[0], p[1], p[2]);
                        // Dll0202App.Shell.MainForm.Instance.ShowBotMessage(msg);
                        total++;
                    }
                }
            }

            string doneMsg = "✔ Total Sampling : " + total;
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage(doneMsg);
        }

        public static void GetSamplingGridUVOfSelectionFaces()
        {
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("🔍 Sampling lưới UV từ các mặt được chọn...");

            var faces = SelectTool.SelectFace();
            if (faces == null || faces.Length == 0)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không có mặt nào được chọn.");
                return;
            }

            var uf = Core.SessionManager.UFSession;

            int uCount = 10;
            int vCount = 10;
            int totalPoints = 0;
            int inFacePoints = 0;
            int onLoopPoints = 0;

            foreach (var faceObj in faces)
            {
                var nxFace = NXOpen.Utilities.NXObjectManager.Get(faceObj.Tag) as NXOpen.Face;
                if (nxFace == null) continue;

                double[] uvMinMax = new double[4];
                uf.Modl.AskFaceUvMinmax(nxFace.Tag, uvMinMax);

                double umin = uvMinMax[0];
                double umax = uvMinMax[1];
                double vmin = uvMinMax[2];
                double vmax = uvMinMax[3];

                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("▶ Face " + nxFace.Tag + " | UV Range: [" + umin.ToString("F2") + ", " + umax.ToString("F2") +
                                  "] x [" + vmin.ToString("F2") + ", " + vmax.ToString("F2") + "]");

                for (int i = 0; i <= uCount; i++)
                {
                    for (int j = 0; j <= vCount; j++)
                    {
                        double u = umin + i * (umax - umin) / uCount;
                        double v = vmin + j * (vmax - vmin) / vCount;
                        double[] uv = new double[2] { u, v };

                        ModlSrfValue eval;
                        uf.Modl.EvaluateFace(nxFace.Tag, 0, uv, out eval);

                        double[] pos = new double[] { eval.srf_pos[0], eval.srf_pos[1], eval.srf_pos[2] };

                        int result;
                        uf.Modl.AskPointContainment(pos, nxFace.Tag, out result);

                        if (result == 1 || result == 3)
                        {
                            Tag ptTag;
                            uf.Curve.CreatePoint(pos, out ptTag);

                            //string msg = string.Format("   ✔ P({0:F2},{1:F2},{2:F2}) [{3}]",
                            //    pos[0], pos[1], pos[2],
                            //    (result == 1 ? "IN" : "LOOP"));

                            //Dll0202App.Shell.MainForm.Instance.ShowBotMessage(msg);

                            totalPoints++;
                            if (result == 1) inFacePoints++;
                            else if (result == 3) onLoopPoints++;
                        }
                    }
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✔ Tổng số điểm: " + totalPoints);
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("   ↳ Trong mặt: " + inFacePoints);
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("   ↳ Trên loop: " + onLoopPoints);
            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("✅ Sampling UV hoàn tất.");
        }

        public static void CreatCsysAtFacesOfSelectionBody()
        {
        }


        public static void CreatNormalVectorAtCenterOfSelectionFaces()
        {
            var faces = Dll0202App.Tools.SelectTool.SelectFace();
            if (faces == null || faces.Length == 0) return;

            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            System.Collections.Generic.List<double[]> normals = new System.Collections.Generic.List<double[]>();

            foreach (var face in faces)
            {
                try
                {
                    double[] data = face.AskFaceDataRaw(); // 16 thông số
                    double[] center = new double[] { data[1], data[2], data[3] };
                    double[] normal = new double[] { data[4], data[5], data[6] }; // đã chuẩn hóa

                    normals.Add(normal); // lưu để thống kê nhóm

                    // Tạo Point tại Center
                    NXOpen.Point3d ptCenter = new NXOpen.Point3d(center[0], center[1], center[2]);
                    NXOpen.Point pointObj = workPart.Points.CreatePoint(ptCenter);

                    // Tạo Direction từ Normal vector
                    NXOpen.Vector3d vecNormal = new NXOpen.Vector3d(normal[0], normal[1], normal[2]);
                    NXOpen.Direction directionObj = workPart.Directions.CreateDirection(ptCenter, vecNormal, NXOpen.SmartObject.UpdateOption.WithinModeling);

                    NXOpen.Features.DatumAxisBuilder datumAxisBuilder = workPart.Features.CreateDatumAxisBuilder(null);
                    datumAxisBuilder.Type = NXOpen.Features.DatumAxisBuilder.Types.PointAndDir;
                    datumAxisBuilder.Point = pointObj;
                    datumAxisBuilder.Vector = directionObj;

                    NXOpen.NXObject datumAxis = datumAxisBuilder.Commit();
                    datumAxisBuilder.Destroy();

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("📌 Created Datum Axis at Face {0}", (uint)face.Tag)
                    );
                }
                catch (System.Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("❌ Error Face {0}: {1}", (uint)face.Tag, ex.Message)
                    );
                }
            }

            // Gom nhóm vector normal gần giống nhau
            System.Collections.Generic.List<System.Collections.Generic.List<double[]>> groups = new System.Collections.Generic.List<System.Collections.Generic.List<double[]>>();
            double dotThreshold = 0.999; // ~cos(2.5°)

            foreach (var n in normals)
            {
                bool added = false;
                for (int i = 0; i < groups.Count; i++)
                {
                    double[] refVec = groups[i][0];
                    double dot = Dot(n, refVec);
                    if (dot > dotThreshold)
                    {
                        groups[i].Add(n);
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    System.Collections.Generic.List<double[]> newGroup = new System.Collections.Generic.List<double[]>();
                    newGroup.Add(n);
                    groups.Add(newGroup);
                }
            }

            Dll0202App.Shell.MainForm.Instance.ShowBotMessage("\n📊 Thống kê nhóm hướng normal:");
            for (int i = 0; i < groups.Count; i++)
            {
                double[] v = groups[i][0];
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                    string.Format("➡ Nhóm {0}: hướng approx ({1:F2}, {2:F2}, {3:F2}) – {4} mặt", i + 1, v[0], v[1], v[2], groups[i].Count)
                );
            }
        }

        public static void CreatBouding2PointCurveOfSelectionFaces()
        {
            var faces = Dll0202App.Tools.SelectTool.SelectFace();
            if (faces == null || faces.Length == 0) return;

            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            foreach (var face in faces)
            {
                try
                {
                    double[] data = face.AskFaceDataRaw(); // 16 thông số

                    // Lấy bounding box min/max
                    double[] minPt = new double[] { data[7], data[8], data[9] };
                    double[] maxPt = new double[] { data[10], data[11], data[12] };

                    // Tạo Line struct (đúng tên trường: start_point, end_point)
                    NXOpen.UF.UFCurve.Line line = new NXOpen.UF.UFCurve.Line();
                    line.start_point = minPt;
                    line.end_point = maxPt;

                    Tag lineTag;
                    Core.SessionManager.UFSession.Curve.CreateLine(ref line, out lineTag);

                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("📏 Created bounding line at Face {0}", (uint)face.Tag)
                    );
                }
                catch (System.Exception ex)
                {
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage(
                        string.Format("❌ Error at Face {0}: {1}", (uint)face.Tag, ex.Message)
                    );
                }
            }
        }

        #region Hàm phụ
        private static double[] Normalize(double[] v)
        {
            double len = Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
            if (len < 1e-8) return new double[] { 0, 0, 0 };
            return new[] { v[0] / len, v[1] / len, v[2] / len };
        }

        private static double[] Subtract(double[] a, double[] b)
        {
            return new[] { a[0] - b[0], a[1] - b[1], a[2] - b[2] };
        }

        private static double Dot(double[] a, double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }

        private static double[] Cross(double[] a, double[] b)
        {
            return new[]
    {
        a[1]*b[2] - a[2]*b[1],
        a[2]*b[0] - a[0]*b[2],
        a[0]*b[1] - a[1]*b[0]
    };
        }

        private static double[] Scale(double[] v, double s)
        {
            return new[] { v[0] * s, v[1] * s, v[2] * s };
        }
        #endregion

    }

    public static class FaceGeometryHelper
    {
        /// <summary>
        /// Compare two 3D points with tolerance
        /// </summary>
        public static bool ArePointsEqual(Point3d a, Point3d b, double tolerance = 1e-4)
        {
            return Math.Abs(a.X - b.X) < tolerance &&
                   Math.Abs(a.Y - b.Y) < tolerance &&
                   Math.Abs(a.Z - b.Z) < tolerance;
        }

        public static Vector3d Cross(Vector3d a, Vector3d b)
        {
            return new Vector3d(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

        public static Vector3d Normalize(Vector3d v)
        {
            double len = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            return new Vector3d(v.X / len, v.Y / len, v.Z / len);
        }

        public static Vector3d Subtract(Point3d a, Point3d b)
        {
            return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static double Dot(Vector3d a, Vector3d b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Point2d ProjectTo2D(Point3d pt, Point3d origin, Vector3d xDir, Vector3d yDir)
        {
            Vector3d vec = Subtract(pt, origin);
            return new Point2d(Dot(vec, xDir), Dot(vec, yDir));
        }

        public static Vector3d EstimateXDirection(Face face)
        {
            Edge longest = null;
            double maxLen = 0.0;
            foreach (Edge e in face.GetEdges())
            {
                Point3d p1, p2;
                e.GetVertices(out p1, out p2);
                double len = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.Z - p1.Z, 2));
                if (len > maxLen)
                {
                    maxLen = len;
                    longest = e;
                }
            }
            if (longest != null)
            {
                Point3d p1, p2;
                longest.GetVertices(out p1, out p2);
                return Normalize(Subtract(p2, p1));
            }
            return new Vector3d(1, 0, 0);
        }

        public static Vector3d EstimateFaceNormal(Face face)
        {
            Edge[] edges = face.GetEdges();
            if (edges.Length < 2) return new Vector3d(0, 0, 1);
            Point3d a1, a2, b1, b2;
            edges[0].GetVertices(out a1, out a2);
            edges[1].GetVertices(out b1, out b2);
            Vector3d v1 = Subtract(a2, a1);
            Vector3d v2 = Subtract(b2, b1);
            return Cross(v1, v2);
        }

        public static double GetBoundingRectangleArea(List<Point2d> pts, out double width, out double height)
        {
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            foreach (var pt in pts)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.Y > maxY) maxY = pt.Y;
            }
            width = maxX - minX;
            height = maxY - minY;
            return width * height;
        }


    }

    public static class EdgeTools
    {
        public static class EdgeSamplingTool
        {
            public static List<double[]> SamplePointsOnEdge(Tag edgeTag, int nStep = 5)
            {
                var points = new List<double[]>();
                var uf = Core.SessionManager.UFSession;

                IntPtr eval;
                uf.Eval.Initialize(edgeTag, out eval);

                double[] limits = new double[2];
                uf.Eval.AskLimits(eval, limits);

                for (int i = 0; i <= nStep; i++)
                {
                    double t = limits[0] + i * (limits[1] - limits[0]) / nStep;
                    double[] pt = new double[3];
                    double[] der = new double[3];
                    uf.Eval.Evaluate(eval, 1, t, pt, der);
                    points.Add(pt);
                }

                uf.Eval.Free(eval);
                return points;
            }

            public static void CreatePointObjects(List<double[]> coords)
            {
                var uf = Core.SessionManager.UFSession;
                foreach (var pt in coords)
                {
                    Tag dummy;
                    uf.Curve.CreatePoint(pt, out dummy);
                }
            }
        }
    }
    public static class StringTool { }
    public static class ComputeTool { }
    public static class MathTool { }

}
#endregion

#region IO
namespace Dll0202App.IO
{
    public static class FilePathManager
    {
        public static string RootBasePath = @"E:\dll0202\";

        public static string ConfigPath
        {
            get { return Path.Combine(RootBasePath, "Config"); }
        }

        public static string LibraryPath
        {
            get { return Path.Combine(RootBasePath, "Lib"); }
        }

        public static string ResultPath
        {
            get { return Path.Combine(RootBasePath, "Result"); }
        }

        public static string JsonRulePath
        {
            get { return Path.Combine(ConfigPath, "rule.json"); }
        }

        public static string NewtonsoftJsonDllPath
        {
            get { return Path.Combine(LibraryPath, "Newtonsoft.Json.dll"); }
        }

        public static string DefaultCsvResultPath
        {
            get { return Path.Combine(ResultPath, "result.csv"); }
        }

        //File hệ thống, cấp mã ID body, Projec ID
        public static string SystemCsvPath
        {
            get { return System.IO.Path.Combine(RootBasePath, "Shared", "System.csv"); }
        }

        // Thư mục chứa PRoject
        public static string GetProjectResultFolder(string projectId)
        {
            return System.IO.Path.Combine(RootBasePath, "InferenceProjects", projectId);
        }

        //Đường dẫn file kết quả csv (Kết quả lấy từ NX và AI trả về)
        public static string GetProjectResultCsvPath(string projectId)
        {
            string folder = GetProjectResultFolder(projectId);
            return System.IO.Path.Combine(folder, projectId + "result.csv");
        }

    }

    public class JsonRuleProcessor
    {
        public class RuleItem
        {
            public List<string> keywords { get; set; }
            public string command { get; set; }
            public string response { get; set; }
        }

        private readonly List<RuleItem> rules;

        public JsonRuleProcessor(string jsonPath = null)
        {
            string path = string.IsNullOrEmpty(jsonPath) ? FilePathManager.JsonRulePath : jsonPath;
            rules = new List<RuleItem>();

            if (!File.Exists(path))
            {
                if (Dll0202App.Shell.MainForm.Instance != null)
                    Dll0202App.Shell.MainForm.Instance.ShowBotMessage("❌ Không tìm thấy file JSON: " + path);
                return;
            }

            string json = File.ReadAllText(path);

            try
            {
                Assembly jsonAssembly = Assembly.LoadFrom(FilePathManager.NewtonsoftJsonDllPath);
                Type jsonConvertType = jsonAssembly.GetType("Newtonsoft.Json.JsonConvert");
                MethodInfo deserializeMethod = jsonConvertType.GetMethod(
                    "DeserializeObject",
                    new[] { typeof(string), typeof(Type) });

                object result = deserializeMethod.Invoke(null, new object[] { json, typeof(List<RuleItem>) });
                rules = (List<RuleItem>)result;
            }
            catch (Exception ex)
            {
                Dll0202App.Core.Logging.Listing("❌ Lỗi nạp JSON: " + ex.Message);
            }
        }

        public RuleItem GetMatchedRule(string input)
        {
            input = input.ToLower();
            RuleItem bestMatch = null;
            int longestKeyword = -1;

            foreach (var rule in rules)
            {
                foreach (string keyword in rule.keywords)
                {
                    var kw = keyword.ToLower();
                    if (input.Contains(kw) && kw.Length > longestKeyword)
                    {
                        bestMatch = rule;
                        longestKeyword = kw.Length;
                    }
                }
            }

            return bestMatch;
        }
    }

    public class CsvManager
    {
        private string _filePath;
        private Encoding _encoding;

        public CsvManager(string filePath, Encoding encoding = null)
        {
            _filePath = filePath;
            _encoding = encoding ?? Encoding.UTF8;
        }

        public string FilePath { get { return _filePath; } }

        #region Common Utilities

        private void EnsureDirectoryExists()
        {
            string folder = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                LogMessage("📂 Created missing folder: " + folder);
            }
        }

        private bool IsFileLocked(string path)
        {
            FileStream stream = null;
            try
            {
                if (!File.Exists(path)) return false;
                stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return false;
        }

        private bool FileExists()
        {
            return File.Exists(_filePath);
        }

        private void LogMessage(string msg)
        {
            if (Dll0202App.Shell.MainForm.Instance != null)
            {
                Dll0202App.Shell.MainForm.Instance.ShowBotMessage("[CsvManager] " + msg);
            }
        }

        #endregion

        #region File Operations

        public bool CreateNewFile(List<string> header = null)
        {
            try
            {
                EnsureDirectoryExists();
                if (FileExists())
                {
                    if (IsFileLocked(_filePath)) return false;
                    File.Delete(_filePath);
                }

                using (StreamWriter writer = new StreamWriter(_filePath, false, _encoding))
                {
                    if (header != null && header.Count > 0)
                    {
                        writer.WriteLine(string.Join(",", EscapeCsvCells(header)));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("❌ CreateNewFile error: " + ex.Message);
                return false;
            }
        }

        public bool DeleteFile()
        {
            try
            {
                if (!FileExists()) return false;
                if (IsFileLocked(_filePath)) return false;
                File.Delete(_filePath);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("❌ DeleteFile error: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Read/Write Operations

        public List<List<string>> ReadAllRows()
        {
            var rows = new List<List<string>>();
            try
            {
                if (!FileExists() || IsFileLocked(_filePath)) return null;

                using (StreamReader reader = new StreamReader(_filePath, _encoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        rows.Add(new List<string>(line.Split(',')));
                    }
                }
                return rows;
            }
            catch (Exception ex)
            {
                LogMessage("❌ ReadAllRows error: " + ex.Message);
                return null;
            }
        }

        public bool AppendRow(List<string> row)
        {
            try
            {
                EnsureDirectoryExists();
                if (IsFileLocked(_filePath)) return false;

                using (StreamWriter writer = new StreamWriter(_filePath, true, _encoding))
                {
                    writer.WriteLine(string.Join(",", EscapeCsvCells(row)));
                }
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("❌ AppendRow error: " + ex.Message);
                return false;
            }
        }

        public bool OverwriteAll(List<List<string>> allRows)
        {
            try
            {
                EnsureDirectoryExists();
                if (IsFileLocked(_filePath)) return false;

                using (StreamWriter writer = new StreamWriter(_filePath, false, _encoding))
                {
                    foreach (var row in allRows)
                    {
                        writer.WriteLine(string.Join(",", EscapeCsvCells(row)));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogMessage("❌ OverwriteAll error: " + ex.Message);
                return false;
            }
        }

        public bool UpdateRow(int rowIndex, List<string> newRowData)
        {
            try
            {
                var rows = ReadAllRows();
                if (rows == null || rowIndex < 0 || rowIndex >= rows.Count) return false;

                rows[rowIndex] = newRowData;
                return OverwriteAll(rows);
            }
            catch (Exception ex)
            {
                LogMessage("❌ UpdateRow error: " + ex.Message);
                return false;
            }
        }

        public bool DeleteRow(int rowIndex)
        {
            try
            {
                var rows = ReadAllRows();
                if (rows == null || rowIndex < 0 || rowIndex >= rows.Count) return false;

                rows.RemoveAt(rowIndex);
                return OverwriteAll(rows);
            }
            catch (Exception ex)
            {
                LogMessage("❌ DeleteRow error: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region CSV Escape

        private List<string> EscapeCsvCells(List<string> row)
        {
            var result = new List<string>();
            foreach (var cell in row)
            {
                string val = cell ?? "";
                if (val.Contains(",") || val.Contains("\""))
                {
                    val = "\"" + val.Replace("\"", "\"\"") + "\"";
                }
                result.Add(val);
            }
            return result;
        }

        #endregion
    }



}

#endregion

#region Data
namespace Dll0202App.Data
{


    public class BodyInfo
    {
        // --- Thông tin định danh
        public long Tag;                // Mã tag

        public string[] Attributes;     // Mảng Attribute

        public string Id;         // Mã hệ thống (Bxxxxxxx)
        public string Cluster;          // Cụm
        public string Group;            // Nhóm phân loại
        public string ProjectId;        // Mã dự án (IPxxxxxx)
        public string PartNo;           // Mã chi tiết
        public string PartName;         // Tên chi tiết
        public string Type;             // Phân loại chi tiết theo hình học
        public string Revision;         // Phiên bản
        public string Material;         // Vật liệu
        public string Size;              // Kích thước bao ngoài
        public string Mass;              // Khối lượng thực
        public string Remarks;          // Ghi 

        public string MaterialColor;    // Màu vật liệu
        public string Heattreatment;    // Nhiệt luyện
        public string Surfacetreatment; // Xử lý bề mặt (mạ, phủ, sơn...)

        public string PartCode;         // Mã code

        // --- Thông tin hiển thị Displayable
        public string BodyType;          // Sheet hoặc Solid
        public string BodyName;
        public int Color;
        public int Layer;
        public int BlankStatus;
        public bool Highlight;
        public int LineWidth;
        public int LineFont;

        // Thông tin bounding hệ WCS
        public double[] BoundingBox = new double[6];
        public double[] CenterOfMass = new double[3];

        /// <summary>
        /// Mảng 47 chỉ số vật lý được trả về từ UF_MODL_ask_mass_props_3d
        /// </summary>
        public double[] MassProps = new double[47];

        /// <summary>Diện tích toàn bộ body (mm²)</summary>
        public double Area { get { return MassProps[0]; } }

        /// <summary>Thể tích body (mm³)</summary>
        public double Volume { get { return MassProps[1]; } }

        /// <summary>Khối lượng override (nếu có gán thủ công)</summary>
        public double OverrideMass { get { return MassProps[2]; } }

        /// <summary>Tọa độ trọng tâm: X, Y, Z</summary>
        public double[] CenterOfMassProp { get { return new double[] { MassProps[3], MassProps[4], MassProps[5] }; } }

        /// <summary>Mô men bậc 1 theo trục X, Y, Z</summary>
        public double[] FirstMoments { get { return new double[] { MassProps[6], MassProps[7], MassProps[8] }; } }

        /// <summary>Mô men quán tính tại hệ WCS (X, Y, Z)</summary>
        public double[] InertiaWCS { get { return new double[] { MassProps[9], MassProps[10], MassProps[11] }; } }

        /// <summary>Mô men quán tính tại trọng tâm (X, Y, Z)</summary>
        public double[] InertiaCentroid { get { return new double[] { MassProps[12], MassProps[13], MassProps[14] }; } }

        /// <summary>Mô men quán tính cầu (spherical moment)</summary>
        public double SphericalMoment { get { return MassProps[15]; } }

        /// <summary>Inertia products tại WCS: Ixy, Iyz, Izx</summary>
        public double[] InertiaProductsWCS { get { return new double[] { MassProps[16], MassProps[17], MassProps[18] }; } }

        /// <summary>Inertia products tại trọng tâm: Ixy, Iyz, Izx</summary>
        public double[] InertiaProductsCentroid { get { return new double[] { MassProps[19], MassProps[20], MassProps[21] }; } }

        /// <summary>Các trục chính (3 vector đơn vị)</summary>
        public double[][] PrincipalAxes
        {
            get
            {
                return new double[][]
        {
            new double[] { MassProps[22], MassProps[23], MassProps[24] },
            new double[] { MassProps[25], MassProps[26], MassProps[27] },
            new double[] { MassProps[28], MassProps[29], MassProps[30] }
        };
            }
        }

        /// <summary>Giá trị mô men quán tính chính theo từng trục</summary>
        public double[] PrincipalMoments { get { return new double[] { MassProps[31], MassProps[32], MassProps[33] }; } }

        /// <summary>Bán kính quán tính theo WCS</summary>
        public double[] RadiiOfGyrationWCS { get { return new double[] { MassProps[34], MassProps[35], MassProps[36] }; } }

        /// <summary>Bán kính quán tính tại trọng tâm</summary>
        public double[] RadiiOfGyrationCentroid { get { return new double[] { MassProps[37], MassProps[38], MassProps[39] }; } }

        /// <summary>Bán kính quán tính cầu (spherical radius of gyration)</summary>
        public double SphericalRadiusOfGyration { get { return MassProps[40]; } }

        /// <summary>Khối lượng riêng override (nếu bị gán thủ công)</summary>
        public double OverrideDensity { get { return MassProps[46]; } }


        // --- Gốc Body Csys đặc trưng
        /// <summary>
        /// Gốc hệ tọa độ định hướng hình học của body
        /// </summary>
        public double[] BodyCsysOrigin;

        /// <summary>
        /// Vector X: hướng theo cạnh chính, hoặc dài nhất
        /// </summary>
        public double[] BodyCsysX;

        /// <summary>
        /// Vector Y: dựng từ tích có hướng Z x X
        /// </summary>
        public double[] BodyCsysY;

        /// <summary>
        /// Vector Z: hướng pháp tuyến của mặt chính
        /// </summary>
        public double[] BodyCsysZ;

        /// <summary>
        /// Ghi chú cách dựng CSYS (vd: "Plane + Longest Edge", "Cylinder Z")
        /// </summary>
        public string BodyCsysHint;

        //Ma trận Csys
        public double[] CsysMatrix = new double[9];

    }

    public class FaceInfo
    {
        //Thông tin định danh, hiển thị
        public long Tag;
        public long FaceID;
        public int FaceColor;
        public string FaceName;
        public string[] Attributes;

        /// 16 thuộc tính từ AskFacedata
        public int Type;                 // Kiểu mặt: plane, cylinder, sphere,...
        public double[] Point = new double[3];   // Tâm hoặc điểm đặc trưng [X,Y,Z]
        public double[] Direction = new double[3]; // Vector pháp tuyến hoặc trục
        public double[] BoundingBox = new double[6]; // MinX, MinY, MinZ, MaxX, MaxY, MaxZ
        public double Radius;            // Bán kính nếu là mặt tròn
        public double RadialData;        // Dữ liệu bán kính phụ (cone...)
        public int NormalDirection;      // 1 hoặc -1

        // Diện tích mặt
        public double FaceArea;
        public double FaceBoundingAre;

        // Số lượng loop
        public int LoopCount;

        // Mảng cạnh của mặt
        public List<EdgeInfo> FaceEdges = new List<EdgeInfo>();

        // Điểm UV mẫu (tuỳ chọn sampling )
        public List<double[]> BodyFacesSamplePoints = new List<double[]>();
    }

    public class EdgeInfo
    {
        public long Tag;
        public string Type;              // Line, Arc, Spline...
        public double Length;
        public double[] StartPoint = new double[3];
        public double[] EndPoint = new double[3];

        /// <summary>
        /// Điểm chia từ các cạnh (sampling theo đoạn)
        /// </summary>
        public List<double[]> BodyEdgesSamplePoints = new List<double[]>();
    }

}

#endregion

