using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;

namespace NXBodySelector
{
    public class BodySelectionForm : Form
    {
        // Session and UI objects from NXOpen
        private static NXOpen.Session theSession;
        private static NXOpen.UI theUI;
        private static Part workPart;

        // UI Controls
        private ComboBox comboBox = new ComboBox();
        private Button displayButton = new Button();
        private Label lblMessage = new Label();

        public BodySelectionForm()
        {
            // Get session, UI, and work part from NXOpen
            theSession = NXOpen.Session.GetSession();
            theUI = NXOpen.UI.GetUI();
            workPart = theSession.Parts.Work;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Initialize ComboBox for body selection
            comboBox.Items.AddRange(GetBodyNames());
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox.Location = new System.Drawing.Point(10, 20);
            comboBox.Width = 250;

            // Initialize Display button
            displayButton.Text = "Display";
            displayButton.Location = new System.Drawing.Point(70, 60);
            displayButton.Size = new Size(120, 30);
            displayButton.Enabled = false; // Initially disabled
            displayButton.Click += new EventHandler(DisplayButton_Click);

            // Initialize Message Label
            lblMessage.Location = new System.Drawing.Point(70, 100);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(200, 23);
            lblMessage.Text = "Select a body";

            // Add controls to form
            this.ClientSize = new Size(300, 180);
            this.Controls.Add(comboBox);
            this.Controls.Add(displayButton);
            this.Controls.Add(lblMessage);
            this.Text = "Select a Body";

            // Event handler for ComboBox selection change
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        }

        // Populate ComboBox with names of all bodies in the current part
        private string[] GetBodyNames()
        {
            List<string> bodyNames = new List<string>();

            // Loop through all bodies in the current work part
            foreach (Body body in workPart.Bodies)
            {
                // Exclude bodies that do not have a name or whose name starts with "Body"
                if (!string.IsNullOrEmpty(body.Name) && !body.Name.StartsWith("Body"))
                {
                    bodyNames.Add(body.Name);
                }
            }

            bodyNames.Sort(BodyNameComparer); // Sort the body names
            return bodyNames.ToArray();
        }

        // Custom comparer function to sort body names
        private int BodyNameComparer(string name1, string name2)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^(\d+)([A-Za-z].*)?$");

            var match1 = regex.Match(name1);
            var match2 = regex.Match(name2);

            if (match1.Success && match2.Success)
            {
                int num1 = int.Parse(match1.Groups[1].Value);
                int num2 = int.Parse(match2.Groups[1].Value);

                if (num1 != num2)
                {
                    return num1.CompareTo(num2);
                }

                string alpha1 = match1.Groups[2].Success ? match1.Groups[2].Value : "";
                string alpha2 = match2.Groups[2].Success ? match2.Groups[2].Value : "";

                return alpha1.CompareTo(alpha2);
            }
            else
            {
                return name1.CompareTo(name2); // Fallback to alphabetical comparison
            }
        }

        // Event handler for ComboBox selection change
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedItem != null && !string.IsNullOrEmpty(comboBox.SelectedItem.ToString()))
            {
                displayButton.Enabled = true;
            }
            else
            {
                displayButton.Enabled = false;
            }
        }

        // Event handler for Display button click
        private void DisplayButton_Click(object sender, EventArgs e)
        {
            string selectedBodyName = comboBox.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(selectedBodyName))
            {
                HideAllBodies();
                ShowBody(selectedBodyName);
                lblMessage.Text = "Displayed {selectedBodyName}";
            }
        }

        // Hide all bodies in the work part
        private void HideAllBodies()
        {
            var markId = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Hide All Bodies");
            theSession.SetUndoMarkName(markId, "Hide All Bodies");

            theSession.DisplayManager.HideByType("SHOW_HIDE_TYPE_SOLID_BODIES", DisplayManager.ShowHideScope.AnyInAssembly);

            int nErrs = theSession.UpdateManager.DoUpdate(markId);
            if (nErrs > 0)
            {
                Console.WriteLine("Errors occurred while hiding bodies.");
            }

            theSession.DeleteUndoMark(markId, null);
        }

        // Show the selected body in the work part
        private void ShowBody(string bodyName)
        {
            List<DisplayableObject> objectsToShow = new List<DisplayableObject>();

            foreach (Body body in workPart.Bodies)
            {
                if (body.Name == bodyName)
                {
                    objectsToShow.Add(body);
                }
            }

            if (objectsToShow.Count > 0)
            {
                theSession.DisplayManager.ShowObjects(objectsToShow.ToArray(), DisplayManager.LayerSetting.ChangeLayerToSelectable);
            }

            workPart.Views.WorkView.FitAfterShowOrHide(NXOpen.View.ShowOrHideType.ShowOnly);
        }
    }

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Create and run the form
            Application.Run(new BodySelectionForm());
        }

        public static int GetUnloadOption(string dummy) 
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately; 
        }
    }
}
