using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;

namespace Translucency
{
    public class TranslucencyForm : Form
    {
        private NXOpen.UI theUI;
        private NXOpen.Session theSession;
        private Label lblMessage;
        private Button btnToggle;
        private Button btn0;
        private Button btn50;
        private Button btn70;
        private Button btn90;
        private int currentTranslucency = 0;

        public TranslucencyForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Translucency";
            this.ClientSize = new Size(250, 110);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            // Label
            lblMessage = new Label();
            lblMessage.Text = "Initializing NX Session...";
            lblMessage.Location = new System.Drawing.Point(10, 5);
            lblMessage.Size = new Size(280, 20);
            this.Controls.Add(lblMessage);

            // Button Toggle: Show/Hide Translucency
            btnToggle = new Button();
            btnToggle.Text = "Show/Hide";
            btnToggle.Location = new System.Drawing.Point(10, 30);
            btnToggle.Size = new Size(100, 30);
            btnToggle.Click += BtnToggle_Click;
            this.Controls.Add(btnToggle);

            // Button 0%
            btn0 = new Button();
            btn0.Text = "0%";
            btn0.Location = new System.Drawing.Point(10, 70);
            btn0.Size = new Size(50, 30);
            btn0.Click += Btn0_Click;
            this.Controls.Add(btn0);

            // Button 50%
            btn50 = new Button();
            btn50.Text = "50%";
            btn50.Location = new System.Drawing.Point(70, 70);
            btn50.Size = new Size(50, 30);
            btn50.Click += Btn50_Click;
            this.Controls.Add(btn50);

            // Button 70%
            btn70 = new Button();
            btn70.Text = "70%";
            btn70.Location = new System.Drawing.Point(130, 70);
            btn70.Size = new Size(50, 30);
            btn70.Click += Btn70_Click;
            this.Controls.Add(btn70);

            // Button 90%
            btn90 = new Button();
            btn90.Text = "90%";
            btn90.Location = new System.Drawing.Point(190, 70);
            btn90.Size = new Size(50, 30);
            btn90.Click += Btn90_Click;
            this.Controls.Add(btn90);

            this.Load += (s, e) =>
            {
                this.Location = new System.Drawing.Point(this.Location.X + 500, this.Location.Y - 200);
            };

            // Khởi tạo Session và UI trên background thread sau khi form hiển thị
            this.Shown += async (s, e) =>
            {
                await Task.Run(() =>
                {
                    theSession = NXOpen.Session.GetSession();
                    theUI = NXOpen.UI.GetUI();
                });
                this.Invoke((Action)(() =>
                {
                    lblMessage.Text = "Session Already!";
                }));
            };
        }

        // Event Show/Hide Translucency
        private void BtnToggle_Click(object sender, EventArgs e)
        {
            if (theUI != null)
            {
                bool current = theUI.VisualizationVisualPreferences.Translucency;
                theUI.VisualizationVisualPreferences.Translucency = !current;
                lblMessage.Text = "Translucency " + (theUI.VisualizationVisualPreferences.Translucency ? "On" : "Off");
            }
            else
            {
                lblMessage.Text = "NXOpen not initialized!";
            }
        }

        // Event các nút % 
        private void Btn0_Click(object sender, EventArgs e)
        {
            currentTranslucency = 0;
            ApplyTranslucencyWithSelection();
        }

        private void Btn50_Click(object sender, EventArgs e)
        {
            currentTranslucency = 50;
            ApplyTranslucencyWithSelection();
        }

        private void Btn70_Click(object sender, EventArgs e)
        {
            currentTranslucency = 70;
            ApplyTranslucencyWithSelection();
        }

        private void Btn90_Click(object sender, EventArgs e)
        {
            currentTranslucency = 90;
            ApplyTranslucencyWithSelection();
        }

        // Apply Translucency dựa trên sự lựa chọn
        private void ApplyTranslucencyWithSelection()
        {
            if (theUI == null || theSession == null)
            {
                lblMessage.Text = "Translucency Updated!";
                return;
            }

            NXObjectSelector selector = new NXObjectSelector();
            NXObject[] selectedObjects = selector.SelectObjectsForTranslucency();

            if (selectedObjects != null && selectedObjects.Length > 0)
            {
                ApplyTranslucencyToSelectedObjects(selectedObjects, currentTranslucency);
                theUI.VisualizationVisualPreferences.Translucency = true;
                lblMessage.Text = "Applied " + currentTranslucency + "% translucency.";
                this.Close();
            }
            else
            {
                lblMessage.Text = "No objects selected!";
            }
        }

        // Áp dụng translucency cho các đối tượng được chọn
        private void ApplyTranslucencyToSelectedObjects(NXObject[] selectedObjects, int translucencyValue)
        {
            DisplayableObject[] displayableObjects = new DisplayableObject[selectedObjects.Length];
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                displayableObjects[i] = selectedObjects[i] as DisplayableObject;
            }

            DisplayModification dispMod = theSession.DisplayManager.NewDisplayModification();
            dispMod.ApplyToAllFaces = true;
            dispMod.NewTranslucency = translucencyValue;
            dispMod.Apply(displayableObjects);
            dispMod.Dispose();
        }

        // Override Dispose để dọn dẹp các component đã tạo ra
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Giải phóng các control nếu chúng khác null
                if (lblMessage != null) lblMessage.Dispose();
                if (btnToggle != null) btnToggle.Dispose();
                if (btn0 != null) btn0.Dispose();
                if (btn50 != null) btn50.Dispose();
                if (btn70 != null) btn70.Dispose();
                if (btn90 != null) btn90.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class NXObjectSelector
    {
        Session theSession = Session.GetSession();

        public NXObject[] SelectObjectsForTranslucency()
        {
            UI ui = UI.GetUI();
            string message = "Select body";
            string title = "Select a Body";
            NXObject[] selectedObjects;

            Selection.MaskTriple[] selectionMask = new Selection.MaskTriple[1];
            selectionMask[0] = new Selection.MaskTriple
            {
                Type = UFConstants.UF_solid_type,
                Subtype = 0,
                SolidBodySubtype = UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY
            };

            Selection.Response response = ui.SelectionManager.SelectObjects(
                message,
                title,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                selectionMask,
                out selectedObjects);

            if (response == Selection.Response.Ok)
            {
                return selectedObjects;
            }
            return null;
        }
    }

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Sử dụng khối using để đảm bảo rằng TranslucencyForm được Dispose sau khi đóng
            using (TranslucencyForm form = new TranslucencyForm())
            {
                Application.Run(form);
            }
        }

        public static int GetUnloadOption(string dummy)
        {
            return (int)Session.LibraryUnloadOption.Immediately;
        }
    }
}
