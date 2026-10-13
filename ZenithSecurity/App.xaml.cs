using System.Windows;

namespace ZenithSecurity
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Check if running as installer or portable
            if (e.Args.Length > 0 && e.Args[0] == "--uninstall")
            {
                Installer.InstallerLogic.Uninstall();
                Shutdown(0);
                return;
            }
        }
    }
}
