using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PowerShellScriptRunner.Services
{
    // TODO: Implement default values for boolean (switch) parameters
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

        public static async Task<List<(string Name, Type DataType, object? DefaultValue)>> GetScriptParametersAsync(string scriptPath)
        {
            var parameters = new List<(string, Type, object?)>();

            if (!File.Exists(scriptPath))
            {
                return parameters;
            }

            string scriptContent = await File.ReadAllTextAsync(scriptPath);

            // Extract only the first top-level `param (...)` block before any function definitions
            Match paramBlockMatch = Regex.Match(scriptContent, @"(?s)^\s*param\s*\((.*?)\)(?:\r?\n|$)", RegexOptions.IgnoreCase);
            if (!paramBlockMatch.Success)
            {
                return parameters;
            }

            string paramBlock = paramBlockMatch.Groups[1].Value; // Extract content inside `param (...)`

            Regex paramRegex = new Regex(@"\[\s*(\w+)\s*\]\s*\$(\w+)(?:\s*=\s*(?:(['""])(.*?)\3|([^,\r\n)]+)))?", RegexOptions.Multiline);

            MatchCollection matches = paramRegex.Matches(paramBlock);

            parameters.AddRange(from Match match in matches
                                let paramType = match.Groups[1].Value
                                let paramName = match.Groups[2].Value
                                let defaultValueString = match.Groups[5].Value.Trim()
                                let defaultValue = match.Groups[4].Success ? match.Groups[4].Value.Trim()
                                                    : match.Groups[5].Success ? defaultValueString
                                                    : null
                                let mappedType = MapPowerShellType(paramType)
                                let parsedDefaultValue = mappedType == typeof(bool) && defaultValue == null ? false : ConvertToType(defaultValue, mappedType)
                                select (paramName, mappedType, parsedDefaultValue));

            return parameters;
        }

        public void RunScriptInNewWindow(string scriptPath, Dictionary<string, object> parameters)
        {
            // Convert dictionary to a PowerShell argument string
            string paramString = string.Join(" ", parameters
                .Where(p => !(p.Value is bool) || (bool)p.Value) // Only include switch param if true
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

        private static Type MapPowerShellType(string powerShellType)
        {
            return powerShellType.ToLower() switch
            {
                "string" => typeof(string),
                "bool" => typeof(bool), // Switch param mapped to bool
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

        private static object? ConvertToType(string value, Type targetType)
        {
            try
            {
                value = value.Trim().Trim('\'', '"'); // Remove extra quotes if present

                if (targetType == typeof(bool))
                {
                    return value.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return targetType == typeof(DateTime) ? date :
                       Convert.ChangeType(value, targetType);
                    }
                    else
                    {
                        return targetType == typeof(DateTime) ? null :
                       Convert.ChangeType(value, targetType);
                    }
                }
            }
            catch
            {
                return null; // If conversion fails, return null
            }
        }
    }
}
