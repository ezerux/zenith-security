using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace ZenithSecurity.Installer
{
    public static class InstallerLogic
    {
        private const string AppName    = "Zenith Local Security Dashboard";
        private const string AppVersion = "4.0";
        private const string RegKey     = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZenithLocalSecurity";

        public static string InstallDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "ZenithSecurity");

        public static string ExeName => "Zenith_Local_Security_Dashboard.exe";
        public static string InstalledExe => Path.Combine(InstallDir, ExeName);

        /// <summary>True if the app is already installed in AppData.</summary>
        public static bool IsInstalled =>
            File.Exists(InstalledExe) &&
            Registry.CurrentUser.OpenSubKey(RegKey) != null;

        /// <summary>True if this process IS the installed copy (running from InstallDir).</summary>
        public static bool IsRunningInstalled
        {
            get
            {
                var me = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                return me.StartsWith(InstallDir, StringComparison.OrdinalIgnoreCase);
            }
        }

        // ── Install ────────────────────────────────────────────────────────────
        public static bool Install()
        {
            try
            {
                Directory.CreateDirectory(InstallDir);

                var src = Process.GetCurrentProcess().MainModule!.FileName;
                var dst = InstalledExe;

                // Copy EXE
                File.Copy(src, dst, overwrite: true);

                // Write uninstaller registry entry
                using var key = Registry.CurrentUser.CreateSubKey(RegKey)!;
                key.SetValue("DisplayName",    AppName);
                key.SetValue("DisplayVersion", AppVersion);
                key.SetValue("Publisher",      "Zenith Security");
                key.SetValue("InstallLocation",InstallDir);
                key.SetValue("UninstallString",$"\"{dst}\" --uninstall");
                key.SetValue("DisplayIcon",    dst);
                key.SetValue("NoModify",       1, RegistryValueKind.DWord);
                key.SetValue("NoRepair",       1, RegistryValueKind.DWord);
                key.SetValue("EstimatedSize",  (int)(new FileInfo(dst).Length / 1024),
                             RegistryValueKind.DWord);

                // Desktop shortcut
                CreateShortcut(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{AppName}.lnk"),
                    dst);

                // Start menu shortcut
                var startMenu = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "Zenith Security");
                Directory.CreateDirectory(startMenu);
                CreateShortcut(Path.Combine(startMenu, $"{AppName}.lnk"), dst);

                return true;
            }
            catch { return false; }
        }

        // ── Uninstall ──────────────────────────────────────────────────────────
        public static void Uninstall()
        {
            try
            {
                // Remove shortcuts
                TryDelete(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{AppName}.lnk"));

                var smDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "Zenith Security");
                if (Directory.Exists(smDir)) Directory.Delete(smDir, true);

                // Remove registry entry
                Registry.CurrentUser.DeleteSubKey(RegKey, throwOnMissingSubKey: false);

                // Schedule self-delete via cmd (can't delete running EXE directly)
                var bat = Path.Combine(Path.GetTempPath(), "zenith_uninstall.bat");
                File.WriteAllText(bat, $"""
@echo off
timeout /t 2 /nobreak >nul
rmdir /s /q "{InstallDir}"
del "%~f0"
""");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{bat}\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            catch { /* best effort */ }
        }

        // ── Shortcut helper (uses WScript.Shell COM) ──────────────────────────
        private static void CreateShortcut(string lnkPath, string targetPath)
        {
            try
            {
                var wsh = Type.GetTypeFromProgID("WScript.Shell")!;
                dynamic shell = Activator.CreateInstance(wsh)!;
                var sc = shell.CreateShortcut(lnkPath);
                sc.TargetPath = targetPath;
                sc.WorkingDirectory = Path.GetDirectoryName(targetPath);
                sc.Description = AppName;
                sc.Save();
            }
            catch { /* non-fatal */ }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        }
    }
}
