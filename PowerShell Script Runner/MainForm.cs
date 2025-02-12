using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PowerShellScriptRunner.Services;

namespace PowerShellScriptRunner
{
    public partial class MainForm : Form
    {
        private string _scriptsDirectory;
        private PowerShellService _powerShellService;
        private Dictionary<string, Control> _parameterInputs = new Dictionary<string, Control>();

        public MainForm()
        {
            InitializeComponent();
            InitializeScriptsDirectory();
            LoadLocalScripts();
        }

        private void InitializeScriptsDirectory()
        {
            // Get the directory where the application is running
            _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");

            // Create the Scripts directory if it doesn't exist
            if (!Directory.Exists(_scriptsDirectory))
            {
                Directory.CreateDirectory(_scriptsDirectory);
            }

            _powerShellService = new PowerShellService(_scriptsDirectory);
        }

        private void LoadLocalScripts()
        {
            try
            {
                var scripts = Directory.GetFiles(_scriptsDirectory, "*.ps1")
                                       .Select(Path.GetFileName)
                                       .ToArray();
                scriptComboBox.Items.Clear();
                scriptComboBox.Items.AddRange(scripts);

                if (scriptComboBox.Items.Count > 0)
                {
                    scriptComboBox.SelectedIndex = 0;
                    LoadScriptParameters();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading scripts", ex);
            }
        }

        private void LoadScriptParameters()
        {
            if (scriptComboBox.SelectedItem == null) return;

            string scriptPath = GetSelectedScriptPath();
            var parameters = _powerShellService.GetScriptParameters(scriptPath);

            parametersPanel.Controls.Clear();
            _parameterInputs.Clear();

            int yOffset = 10;
            foreach (var (name, type) in parameters)
            {
                CreateParameterControl(name, type, yOffset);
                yOffset += 30;
            }
        }

        private void CreateParameterControl(string name, Type type, int yOffset)
        {
            // Create label
            Label lbl = new Label { Text = $"{name}:", Top = yOffset };

            // Create control based on parameter type
            Control inputControl = type switch
            {
                Type t when t == typeof(bool) => new CheckBox { Name = name, Top = yOffset, Left = 100 },
                Type t when t == typeof(DateTime) => new DateTimePicker { Name = name, Top = yOffset, Left = 100 },
                Type t when t == typeof(int) => new NumericUpDown { Name = name, Top = yOffset, Left = 100, Width = 200, Minimum = 0, Maximum = 1000000 },
                Type t when t == typeof(decimal) => new NumericUpDown { Name = name, Top = yOffset, Left = 100, Width = 200, DecimalPlaces = 2, Minimum = 0, Maximum = 1000000 },
                Type t when t == typeof(string) => new TextBox { Name = name, Top = yOffset, Left = 100, Width = 200 },
                _ => throw new ArgumentException("Unsupported parameter type")
            };

            // Add label and input control to the panel
            parametersPanel.Controls.Add(lbl);
            parametersPanel.Controls.Add(inputControl);

            // Store the control in the dictionary for later use
            _parameterInputs[name] = inputControl;
        }


        private void runScriptButton_Click(object sender, EventArgs e)
        {
            if (scriptComboBox.SelectedItem == null) return;

            string scriptPath = GetSelectedScriptPath();
            var parameters = CollectParameterValues();

            try
            {
                string output = _powerShellService.ExecuteScript(scriptPath, parameters);
                outputRichTextBox.Text = output;
            }
            catch (Exception ex)
            {
                ShowError("Execution Error", ex);
            }
        }

        private Dictionary<string, object> CollectParameterValues()
        {
            var parameters = new Dictionary<string, object>();

            foreach (var entry in _parameterInputs)
            {
                parameters[entry.Key] = entry.Value switch
                {
                    TextBox txtBox => txtBox.Text,
                    CheckBox checkBox => checkBox.Checked,
                    DateTimePicker dateTimePicker => dateTimePicker.Value,
                    NumericUpDown numericUpDown => numericUpDown.Value,  // Handle int/decimal values
                    _ => null
                };
            }

            return parameters;
        }


        private string GetSelectedScriptPath()
        {
            return Path.Combine(_scriptsDirectory, scriptComboBox.SelectedItem.ToString());
        }

        private void scriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScriptParameters();
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show($"{title}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
