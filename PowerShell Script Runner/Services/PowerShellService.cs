using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PowerShellScriptRunner.Services
{
    class PowerShellService
    {
        private readonly string _scriptsDirectory;

        public PowerShellService(string scriptsDirectory)
        {
            _scriptsDirectory = scriptsDirectory ?? throw new ArgumentNullException(nameof(scriptsDirectory));
        }

        public async Task<IEnumerable<string>> GetAvailableScriptsAsync()
        {
            if (!Directory.Exists(_scriptsDirectory))
            {
                throw new DirectoryNotFoundException($"Scripts directory not found: {_scriptsDirectory}");
            }

            return await Task.Run(() => Directory.GetFiles(_scriptsDirectory, "*.ps1"));
        }

        public async Task<List<(string Name, Type DataType, object? DefaultValue)>> GetScriptParametersAsync(string scriptPath)
        {
            var parameters = new List<(string, Type, object?)>();

            if (!File.Exists(scriptPath))
            {
                return parameters;
            }

            string scriptContent = await File.ReadAllTextAsync(scriptPath);

            Regex regex = new Regex(@"\[\s*(\w+)\s*\]\s*\$(\w+)(?:\s*=\s*['\""]?([^#\r\n'\""]+)['\""]?)?", RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(scriptContent);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    string paramType = match.Groups[1].Value;
                    string paramName = match.Groups[2].Value;
                    string? defaultValue = match.Groups[3].Success ? match.Groups[3].Value.Trim() : null;

                    Type mappedType = MapPowerShellType(paramType);
                    object? parsedDefaultValue = defaultValue != null ? ConvertToType(defaultValue, mappedType) : null;

                    parameters.Add((paramName, mappedType, parsedDefaultValue));
                }
            }

            return parameters;
        }

        public void RunScriptInNewWindow(string scriptPath, Dictionary<string, object> parameters)
        {
            // Convert dictionary to a PowerShell argument string
            string paramString = string.Join(" ", parameters
                .Where(p => !(p.Value is bool) || (bool)p.Value) // Only include switch ps param if true
                .Select(p => p.Value is bool ? $"-{p.Key}" : $"-{p.Key} \"{p.Value}\""));

            // PowerShell command to execute
            string command = $"powershell -NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" {paramString}";

            // Start a new PowerShell process in a separate window
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {command}",  // Opens in a new window
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private Type MapPowerShellType(string powerShellType)
        {
            return powerShellType.ToLower() switch
            {
                "string" => typeof(string),
                "bool" => typeof(bool), // Currently doesn't work as params are passed as strings, use switch instead in PowerShell script
                "switch" => typeof(bool),
                "datetime" => typeof(DateTime),
                "int" => typeof(int),
                "decimal" => typeof(decimal),
                "double" => typeof(double),
                "float" => typeof(float),
                "byte" => typeof(byte),
                "char" => typeof(char),
                "long" => typeof(long),
                _ => typeof(string), // Default to string if no match is found
            };
        }

        private object? ConvertToType(string value, Type targetType)
        {
            try
            {
                value = value.Trim().Trim('\'', '"'); // Remove extra quotes if present

                return targetType == typeof(bool) ? value.Equals("true", StringComparison.OrdinalIgnoreCase) :
                       targetType == typeof(DateTime) ? DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null :
                       Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null; // If conversion fails, return null
            }
        }
    }
}
