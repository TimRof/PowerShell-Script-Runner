using System.Security.Principal;

namespace PowerShellScriptRunner.Services
{
    public static class AdminCheckerService
    {
        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
