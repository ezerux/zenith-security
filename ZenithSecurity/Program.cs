using System;
using System.Windows;
using ZenithSecurity.UI;

namespace ZenithSecurity
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();

            // If launched from installer (--installed) skip setup wizard
            bool skipSetup = args.Length > 0 &&
                (args[0] == "--installed" || args[0] == "--run");

            if (skipSetup || Installer.InstallerLogic.IsRunningInstalled)
            {
                var main = new MainWindow();
                app.Run(main);
            }
            else
            {
                var setup = new SetupWindow();
                app.Run(setup);
            }
        }
    }
}
