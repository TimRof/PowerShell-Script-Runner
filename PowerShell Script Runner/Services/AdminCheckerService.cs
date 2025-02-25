using System.Security.Principal;

namespace PowerShell_Script_Runner.Services
{
    static class AdminCheckerService
    {
        public static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
