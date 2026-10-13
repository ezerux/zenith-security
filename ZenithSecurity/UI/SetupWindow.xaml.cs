using System.Diagnostics;
using System.Windows;
using ZenithSecurity.Installer;

namespace ZenithSecurity.UI
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            // Show "already installed" notice if installed
            if (InstallerLogic.IsInstalled)
            {
                AlreadyBorder.Visibility = Visibility.Visible;
                AlreadyText.Text = $"Program is already installed at: {InstallerLogic.InstallDir}";
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (RbInstall.IsChecked == true)
            {
                // Install mode
                if (InstallerLogic.IsRunningInstalled)
                {
                    // Already running from install dir — just open main window
                    LaunchMain();
                    return;
                }

                var result = MessageBox.Show(
                    $"Zenith Local Security Dashboard will be installed to:\n{InstallerLogic.InstallDir}\n\nDesktop and Start Menu shortcuts will be created.\n\nContinue?",
                    "Install", MessageBoxButton.OKCancel, MessageBoxImage.Information);

                if (result != MessageBoxResult.OK) return;

                BtnNext.IsEnabled = false;
                BtnNext.Content   = "Installing…";

                bool ok = InstallerLogic.Install();
                if (ok)
                {
                    MessageBox.Show("Installation complete!\n\nClick OK to launch.", "Done",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    // Relaunch from install dir
                    Process.Start(new ProcessStartInfo
                    {
                        FileName  = InstallerLogic.InstalledExe,
                        Arguments = "--installed"
                    });
                    Application.Current.Shutdown(0);
                }
                else
                {
                    MessageBox.Show("Installation failed. Try running as administrator or use Run mode.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    BtnNext.IsEnabled = true;
                    BtnNext.Content   = "Next >";
                }
            }
            else
            {
                // Run portable
                LaunchMain();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void LaunchMain()
        {
            var main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
