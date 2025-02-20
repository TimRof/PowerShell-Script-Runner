using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PowerShellScriptRunner.Services
{
    /// <summary>
    /// Service for running PowerShell scripts.
    /// </summary>
    class PowerShellService
    {
        private readonly string _scriptsDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellService"/> class.
        /// </summary>
        /// <param name="scriptsDirectory">The directory where PowerShell scripts are located.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="scriptsDirectory"/> is null.</exception>
        public PowerShellService(string scriptsDirectory)
        {
            _scriptsDirectory = scriptsDirectory ?? throw new ArgumentNullException(nameof(scriptsDirectory));
        }

        /// <summary>
        /// Gets the available PowerShell scripts in the specified directory.
        /// </summary>
        /// <returns>A list of available script file paths.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the scripts directory does not exist.</exception>
        public async Task<IEnumerable<string>> GetAvailableScriptsAsync()
        {
            if (!Directory.Exists(_scriptsDirectory))
            {
                throw new DirectoryNotFoundException($"Scripts directory not found: {_scriptsDirectory}");
            }

            return await Task.Run(() => Directory.GetFiles(_scriptsDirectory, "*.ps1"));
        }

        /// <summary>
        /// Gets the parameters of a PowerShell script.
        /// </summary>
        /// <param name="scriptPath">The path to the PowerShell script.</param>
        /// <returns>A list of tuples containing parameter name, data type, and default value.</returns>
        public static async Task<List<(string Name, Type DataType, object? DefaultValue)>> GetScriptParametersAsync(string scriptPath)
        {
            var parameters = new List<(string, Type, object?)>();

            if (!File.Exists(scriptPath))
            {
                return parameters;
            }

            string scriptContent = await File.ReadAllTextAsync(scriptPath);

            // Extract only the first top-level `param (...)` block
            Match paramBlockMatch = Regex.Match(scriptContent, @"(?s)^\s*param\s*\((.*?)\)(?:\r?\n|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (!paramBlockMatch.Success)
            {
                return parameters;
            }

            string paramBlock = paramBlockMatch.Groups[1].Value; // Extract content inside `param (...)` block

            Regex paramRegex = new Regex(@"\[\s*(\w+)\s*\]\s*\$(\w+)(?:\s*=\s*(?:(['""])(.*?)\3|([^,\r\n)]+)))?", RegexOptions.Multiline | RegexOptions.Compiled);

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

        /// <summary>
        /// Runs a PowerShell script in a new window.
        /// </summary>
        /// <param name="scriptPath">The path to the PowerShell script.</param>
        /// <param name="parameters">The parameters to pass to the script.</param>
        public void RunScriptInNewWindow(string scriptPath, Dictionary<string, object> parameters)
        {
            string paramString = ConvertParametersToString(parameters);
            string command = BuildPowerShellCommand(scriptPath, paramString);
            StartPowerShellProcess(command);
        }

        private static string ConvertParametersToString(Dictionary<string, object> parameters)
        {
            // TODO: Changes needed for Boolean parameters
            return string.Join(" ", parameters
                .Where(p => !(p.Value is bool) || (bool)p.Value) // Only include switch param if true
                .Select(p => p.Value is bool ? $"-{p.Key}" : $"-{p.Key} \"{p.Value}\""));
        }

        private static string BuildPowerShellCommand(string scriptPath, string paramString)
        {
            return $"powershell -NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" {paramString}";
        }

        private static void StartPowerShellProcess(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C start {command}",  // Opens in a new window
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        /// <summary>
        /// Maps a PowerShell type to a .NET type.
        /// </summary>
        /// <param name="powerShellType">The PowerShell type as a string.</param>
        /// <returns>The corresponding .NET type.</returns>
        private static Type MapPowerShellType(string powerShellType)
        {
            return powerShellType.ToLower() switch
            {
                "string" => typeof(string),
                "bool" => typeof(bool),
                "switch" => typeof(bool), // Switch param mapped to bool
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

        /// <summary>
        /// Converts a string value to a specified .NET type.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The target .NET type.</param>
        /// <returns>The converted value, or null if conversion fails.</returns>
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
