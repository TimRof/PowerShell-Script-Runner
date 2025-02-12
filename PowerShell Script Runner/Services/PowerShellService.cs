using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PowerShellScriptRunner.Services
{
    class PowerShellService
    {
        private readonly string _scriptsDirectory;

        public PowerShellService(string scriptsDirectory)
        {
            _scriptsDirectory = scriptsDirectory;
        }

        public async Task<IEnumerable<string>> GetAvailableScriptsAsync()
        {
            if (!Directory.Exists(_scriptsDirectory))
            {
                throw new DirectoryNotFoundException($"Scripts directory not found: {_scriptsDirectory}");
            }

            return await Task.Run(() => Directory.GetFiles(_scriptsDirectory, "*.ps1"));
        }

        public async Task<List<(string Name, Type DataType)>> GetScriptParametersAsync(string scriptPath)
        {
            List<(string, Type)> parameters = new List<(string, Type)>();

            if (!File.Exists(scriptPath))
            {
                return parameters;
            }

            string scriptContent = await File.ReadAllTextAsync(scriptPath);

            Regex regex = new Regex(@"\[\s*(\w+)\s*\]\s*\$(\w+)", RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(scriptContent);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    string paramType = match.Groups[1].Value;
                    string paramName = match.Groups[2].Value;
                    Type mappedType = MapPowerShellType(paramType);
                    parameters.Add((paramName, mappedType));
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

                var results = await Task.Run(() => ps.Invoke());
                return string.Join(Environment.NewLine, results);
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
                _ => typeof(string), // Default to string if no match is found
            };
        }
    }
}
