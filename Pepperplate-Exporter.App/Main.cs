using Pepperplate_Exporter.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pepperplate_Exporter.App
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void chooseFolder_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            //validate all textboxes have been filled in with valid values
            if (!ValidateForm()) return;

            console.Text = "";
            console.Visible = true;

            //get a reference to the formatter
            IRecipeFormatter recipeFormatter;
            if (format.SelectedItem.ToString() == "Text")
            {
                recipeFormatter = new Lib.TextRecipeFormatter();
            }
            else if (format.SelectedItem.ToString() == "XML")
            {
                recipeFormatter = new Lib.PPXMLFormatter();
            }
            else
            {
                MessageBox.Show("Invalid format chosen.  You must choose Text or XML.");
                return;
            }

            export.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            
            Task.Factory.StartNew(() =>
            {
                //do the export
                try
                {
                    var bl = new Lib.PepperplateExporter(recipeFormatter);
                    var recipeCount = bl.ExportRecipes(username.Text, password.Text, folder.Text, (string message) => WriteTextSafe(message));
                    MessageBox.Show($"{recipeCount} recipes were successfully exported.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Sorry, an error occurred exporting your recipes: {ex.Message}");
                }
                finally
                {
                    Reset();
                }
            });
        }

        private delegate void SafeCallDelegate(string text);
        private void WriteTextSafe(string text)
        {
            if (console.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteTextSafe);
                console.Invoke(d, new object[] { text });
            }
            else
            {
                console.AppendText(text + Environment.NewLine);
                console.ScrollToCaret();
            }
        }

        private delegate void SafeCallDelegate2();
        private void Reset()
        {
            if (console.InvokeRequired)
            {
                var d = new SafeCallDelegate2(Reset);
                console.Invoke(d);
            }
            else
            {
                export.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidateForm()
        {
            if (String.IsNullOrEmpty(format.SelectedItem.ToString()))
            {
                MessageBox.Show("You must choose a format to export to.");
                format.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(folder.Text))
            {
                MessageBox.Show("You must choose a folder to export to.");
                folder.Focus();
                return false;
            }
            if (!System.IO.Directory.Exists(folder.Text))
            {
                MessageBox.Show("You must choose a valid folder to export to.");
                folder.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(username.Text))
            {
                MessageBox.Show("You must enter your Pepperplate username.");
                username.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(password.Text))
            {
                MessageBox.Show("You must enter your Pepperplate password.");
                password.Focus();
                return false;
            }
            return true;
        }
        
    }
}
