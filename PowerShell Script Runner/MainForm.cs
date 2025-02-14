using PowerShellScriptRunner.Services;

namespace PowerShellScriptRunner
{
    public partial class MainForm : Form
    {
        private readonly string _scriptsDirectory;
        private readonly PowerShellService _powerShellService;
        private readonly Dictionary<string, Control> _parameterInputs = new();

        public MainForm()
        {
            InitializeComponent();
            _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");

            if (!Directory.Exists(_scriptsDirectory))
            {
                Directory.CreateDirectory(_scriptsDirectory);
            }

            _powerShellService = new PowerShellService(_scriptsDirectory);

            LoadLocalScriptsAsync();
        }

        private async void LoadLocalScriptsAsync()
        {
            try
            {
                var scripts = await _powerShellService.GetAvailableScriptsAsync();

                scriptComboBox.Invoke(() =>
                {
                    scriptComboBox.Items.Clear();
                    scriptComboBox.Items.AddRange(scripts.Select(Path.GetFileName).ToArray());

                    if (scriptComboBox.Items.Count > 0)
                    {
                        scriptComboBox.SelectedIndex = 0;
                    }
                });

                await LoadScriptParametersAsync();
            }
            catch (Exception ex)
            {
                ShowError("Error loading scripts", ex);
            }
        }

        private async Task LoadScriptParametersAsync()
        {
            if (scriptComboBox.SelectedItem == null) return;

            string scriptPath = GetSelectedScriptPath();
            var parameters = await _powerShellService.GetScriptParametersAsync(scriptPath);

            parametersPanel.Invoke(() =>
            {
                parametersPanel.Controls.Clear();
                _parameterInputs.Clear();

                int yOffset = 10;
                foreach (var (name, type, defaultValue) in parameters)
                {
                    CreateParameterControl(name, type, yOffset, defaultValue);
                    yOffset += 30;
                }
            });
        }

        private void CreateParameterControl(string name, Type type, int yOffset, object? defaultValue)
        {
            var lbl = new Label { Text = $"{name}:", Top = yOffset };
            Control inputControl = type switch
            {
                Type t when t == typeof(bool) => new CheckBox { Name = name, Top = yOffset, Left = 100, Checked = defaultValue is bool b && b },
                Type t when t == typeof(DateTime) => new DateTimePicker { Name = name, Top = yOffset, Left = 100, Value = defaultValue is DateTime dt ? dt : DateTime.Now },
                Type t when t == typeof(int) => new NumericUpDown { Name = name, Top = yOffset, Left = 100, Width = 200, Minimum = 0, Maximum = 1000000, Value = defaultValue is int i ? i : 0 },
                Type t when t == typeof(decimal) => new NumericUpDown { Name = name, Top = yOffset, Left = 100, Width = 200, DecimalPlaces = 2, Minimum = 0, Maximum = 1000000, Value = defaultValue is decimal d ? d : 0 },
                Type t when t == typeof(string) => new TextBox { Name = name, Top = yOffset, Left = 100, Width = 200, Text = defaultValue as string ?? "" },
                _ => throw new ArgumentException($"Unsupported parameter type: {type}")
            };

            parametersPanel.Controls.Add(lbl);
            parametersPanel.Controls.Add(inputControl);
            _parameterInputs[name] = inputControl;
        }

        private void runScriptButton_Click(object sender, EventArgs e)
        {
            if (scriptComboBox.SelectedItem == null) return;

            string scriptPath = GetSelectedScriptPath();
            var parameters = CollectParameterValues();

            try
            {
                _powerShellService.RunScriptInNewWindow(scriptPath, parameters);
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
                    DateTimePicker dateTimePicker => dateTimePicker.Value.ToString("yyyy-MM-dd"),
                    NumericUpDown numericUpDown => numericUpDown.Value,
                    _ => null
                };
            }

            return parameters;
        }

        private string GetSelectedScriptPath()
        {
            return Path.Combine(_scriptsDirectory, scriptComboBox.SelectedItem.ToString() ?? string.Empty);
        }

        private async void scriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await LoadScriptParametersAsync();
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show($"{title}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void scriptListRefreshButton_Click(object sender, EventArgs e)
        {
            LoadLocalScriptsAsync();
        }
    }
}