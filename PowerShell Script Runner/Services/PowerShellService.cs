using System.Globalization;
using System.Management.Automation;
using System.Text;
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

        public async Task<string> ExecuteScriptAsync(string scriptPath, Dictionary<string, object> parameters)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Script not found: {scriptPath}");
            }

            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript(await File.ReadAllTextAsync(scriptPath));

                foreach (var param in parameters)
                {
                    ps.AddParameter(param.Key, param.Value);
                }

                StringBuilder output = new StringBuilder();
                var results = await Task.Run(() => ps.Invoke());

                if (ps.HadErrors)
                {
                    var errors = string.Join(Environment.NewLine, ps.Streams.Error.Select(e => e.ToString()));
                    throw new InvalidOperationException($"PowerShell script execution failed:\n{errors}");
                }

                foreach (var result in results)
                {
                    output.AppendLine(result.ToString());
                }

                return output.ToString().Trim();
            }
        }

        private Type MapPowerShellType(string powerShellType)
        {
            return powerShellType.ToLower() switch
            {
                "string" => typeof(string),
                "bool" => typeof(bool),
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
