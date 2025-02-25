﻿using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PowerShellScriptRunner.Services
{
    /// <summary>
    /// Service for running PowerShell scripts.
    /// </summary>
    partial class PowerShellService
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
        public static async Task<(List<(string Name, Type DataType, object? DefaultValue)>, Dictionary<string, string>)> GetScriptParametersAsync(string scriptPath)
        {
            var parameters = new List<(string, Type, object?)>();
            var dependencies = new Dictionary<string, string>();

            if (!File.Exists(scriptPath))
            {
                return (parameters, dependencies);
            }

            string scriptContent = await File.ReadAllTextAsync(scriptPath);

            // Extract only the first top-level `param (...)` block
            Match paramBlockMatch = ParamBlockRegex().Match(scriptContent);
            if (!paramBlockMatch.Success)
            {
                return (parameters, dependencies);
            }

            string paramBlock = paramBlockMatch.Groups[1].Value; // Extract content inside `param (...)` block

            Regex paramRegex = ParametersRegex();


            MatchCollection matches = paramRegex.Matches(paramBlock);

            foreach (Match match in matches)
            {
                // Extract the dependency if it exists
                string dependency = match.Groups[6].Value.Trim();

                if (!string.IsNullOrEmpty(dependency))
                {
                    dependencies[match.Groups[2].Value] = dependency;
                }

                if (match.Groups[1].Value == "bool")
                {
                    throw new ArgumentException("Booleans are not supported, use Switch instead.");
                }
            }

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

            return (parameters, dependencies);
        }

        /// <summary>
        /// Runs a PowerShell script in a new window.
        /// </summary>
        /// <param name="scriptPath">The path to the PowerShell script.</param>
        /// <param name="parameters">The parameters to pass to the script.</param>
        public static void RunScriptInNewWindow(string scriptPath, Dictionary<string, object> parameters)
        {
            string paramString = ConvertParametersToString(parameters);
            string command = BuildPowerShellCommand(scriptPath, paramString);
            StartPowerShellProcess(command);
        }

        private static string ConvertParametersToString(Dictionary<string, object> parameters)
        {
            // TODO: Changes needed for Boolean parameters
            return string.Join(" ", parameters
                .Where(p => p.Value is not bool || (bool)p.Value) // Only include switch param if true
                .Select(p => p.Value is bool ? $"-{p.Key}" : $"-{p.Key} \"{p.Value}\""));
        }

        private static string BuildPowerShellCommand(string scriptPath, string paramString)
        {
            return $"powershell -NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" {paramString}";
        }

        private static void StartPowerShellProcess(string command)
        {
            ProcessStartInfo psi = new()
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
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
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

        [GeneratedRegex(@"
    \[\s*(\w+)\s*\]              # Match type: [string], [datetime], etc.
    \s*\$(\w+)                   # Match parameter name: $StartDate, $EndDate
    (?:\s*=\s*                   # Start of optional default value section
        (?:(['""])(.*?)\1 |       # Match quoted values
        ([^,#\r\n\)]+))           # Match non-quoted values
    )?                            # End of optional default value section
    (?:\s*,\s*)?                  # Match optional comma
    (?:\s*\#\s*DependsOn:\s*(\w+))? # Match optional #DependsOn: Dependency
", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
        private static partial Regex ParametersRegex();
        [GeneratedRegex(@"(?s)^\s*param\s*\((.*?)\)(?:\r?\n|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-NL")]
        private static partial Regex ParamBlockRegex();
    }
}
