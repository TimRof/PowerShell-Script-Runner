using PowerShellScriptRunner.Services;

namespace PowerShellScriptRunner
{
    public partial class MainForm : Form
    {
        private readonly string _scriptsDirectory;
        private readonly PowerShellService _powerShellService;
        private readonly Dictionary<string, Control> _parameterInputs = new();
        private readonly Dictionary<string, string> _dependencies = new();

        public MainForm()
        {
            InitializeComponent();
            _scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");

            if (!Directory.Exists(_scriptsDirectory))
            {
                Directory.CreateDirectory(_scriptsDirectory);
            }

            _powerShellService = new PowerShellService(_scriptsDirectory);

            LoadLocalScriptsAsync().ConfigureAwait(false);
        }

        private async Task LoadLocalScriptsAsync()
        {
            try
            {
                var scripts = await _powerShellService.GetAvailableScriptsAsync();

                await scriptComboBox.InvokeAsync(() =>
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
            var parameters = new List<(string Name, Type DataType, object? DefaultValue)>();
            var dependencies = new Dictionary<string, string>();
            try
            {
                var result = await PowerShellService.GetScriptParametersAsync(scriptPath);
                parameters = result.Item1;
                dependencies = result.Item2;

                _dependencies.Clear();
                foreach (var dep in dependencies)
                {
                    _dependencies[dep.Key] = dep.Value;
                }

                if (!parameters.Any())
                {
                    var lbl = new Label { Text = "No parameters found.", Width = 200 };
                    parametersPanel.Controls.Add(lbl);
                    return;
                }

                await parametersPanel.InvokeAsync(() =>
                {
                    ClearParameterControls();

                    int yOffset = 10;
                    foreach (var (name, type, defaultValue) in parameters)
                    {
                        CreateParameterControl(name, type, yOffset, defaultValue);
                        yOffset += 30;
                    }

                    // Disable controls based on dependencies
                    DisableDependentControls();
                });
            }
            catch (Exception e)
            {
                ShowError("Error loading script parameters", e);
            }
        }

        private void ClearParameterControls()
        {
            parametersPanel.Controls.Clear();
            _parameterInputs.Clear();
        }

        private void CreateParameterControl(string name, Type type, int yOffset, object? defaultValue)
        {
            int controlWidth = parametersPanel.Size.Width / 2;
            int labelWidth = (int)Math.Floor(controlWidth * 0.7);
            int controlLabelPadding = 25;

            var lbl = new Label { Text = $"{name}:", Top = yOffset, Width = labelWidth };

            Control inputControl = type switch
            {
                Type t when t == typeof(bool) => new CheckBox { Name = name, Top = yOffset, Left = labelWidth + controlLabelPadding, Checked = defaultValue is bool b && b },
                Type t when t == typeof(DateTime) => new DateTimePicker { Name = name, Top = yOffset, Left = labelWidth + controlLabelPadding, Width = controlWidth, Format = DateTimePickerFormat.Short, Value = defaultValue is DateTime dt ? dt : DateTime.Now },
                Type t when t == typeof(int) => new NumericUpDown { Name = name, Top = yOffset, Left = labelWidth + controlLabelPadding, Width = controlWidth, Minimum = 0, Maximum = 1000000, Value = defaultValue is int i ? i : 0 },
                Type t when t == typeof(decimal) => new NumericUpDown { Name = name, Top = yOffset, Left = labelWidth + controlLabelPadding, Width = controlWidth, DecimalPlaces = 2, Minimum = 0, Maximum = 1000000, Value = defaultValue is decimal d ? d : 0 },
                Type t when t == typeof(string) => new TextBox { Name = name, Top = yOffset, Left = labelWidth + controlLabelPadding, Width = controlWidth, Text = defaultValue as string ?? "" },
                _ => throw new ArgumentException($"Unsupported parameter type: {type}")
            };

            parametersPanel.Controls.Add(lbl);
            parametersPanel.Controls.Add(inputControl);
            _parameterInputs[name] = inputControl;

            // Add event handler to update dependency status when value changes
            if (inputControl is CheckBox checkBox)
            {
                checkBox.CheckedChanged += (sender, e) => DisableDependentControls();
            }
            else if (inputControl is TextBox textBox)
            {
                textBox.TextChanged += (sender, e) => DisableDependentControls();
            }
            else if (inputControl is DateTimePicker dateTimePicker)
            {
                dateTimePicker.ValueChanged += (sender, e) => DisableDependentControls();
            }
            else if (inputControl is NumericUpDown numericUpDown)
            {
                numericUpDown.ValueChanged += (sender, e) => DisableDependentControls();
            }
        }

        private void DisableDependentControls()
        {
            foreach (var entry in _parameterInputs)
            {
                string paramName = entry.Key;
                Control control = entry.Value;

                if (_dependencies.ContainsKey(paramName))
                {
                    string dependentParam = _dependencies[paramName];

                    // Check the value of the dependent parameter and disable/enable the control
                    if (_parameterInputs.ContainsKey(dependentParam))
                    {
                        Control dependentControl = _parameterInputs[dependentParam];

                        bool isDependentEnabled = dependentControl switch
                        {
                            CheckBox checkBox => checkBox.Checked,
                            TextBox textBox => !string.IsNullOrWhiteSpace(textBox.Text),
                            DateTimePicker dateTimePicker => dateTimePicker.Value != DateTime.MinValue,
                            NumericUpDown numericUpDown => numericUpDown.Value > 0,
                            _ => true
                        };

                        control.Enabled = isDependentEnabled;
                    }
                }
            }
        }

        private async void runScriptButton_Click(object sender, EventArgs e)
        {
            if (scriptComboBox.SelectedItem == null) return;

            string scriptPath = GetSelectedScriptPath();
            var parameters = CollectParameterValues();

            try
            {
                await Task.Run(() => _powerShellService.RunScriptInNewWindow(scriptPath, parameters));
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

        private async void ScriptComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearParameterControls();
            await LoadScriptParametersAsync();
        }

        private static void ShowError(string title, Exception ex)
        {
            MessageBox.Show($"{title}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void ScriptListRefreshButton_Click(object sender, EventArgs e)
        {
            await LoadLocalScriptsAsync();
        }
    }
}
