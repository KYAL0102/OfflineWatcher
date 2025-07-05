using Microsoft.Win32;

namespace Core;

public static class VlcFinder
{
    public static string? FindVlcInstallation()
    {
        if (OperatingSystem.IsWindows()) return FindVlcInstallationWindows();
        return OperatingSystem.IsLinux() ? FindVlcInstallationLinux() : null;
    }

    private static string? FindVlcInstallationWindows()
    {
        // Check common installation directories
        string[] possiblePaths =
        {
            @"C:\Program Files\VideoLAN\VLC\vlc.exe",
            @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe"
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        // Check the Windows Registry
        #if WINDOWS
        using (Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
        {
            if (key != null)
            {
                foreach (string subkeyName in key.GetSubKeyNames())
                {
                    using (Microsoft.Win32.RegistryKey? subkey = key.OpenSubKey(subkeyName))
                    {
                        if (subkey != null)
                        {
                            string? displayName = subkey.GetValue("DisplayName") as string;
                            if (displayName != null && displayName.Contains("VLC media player"))
                            {
                                string? installLocation = subkey.GetValue("InstallLocation") as string;
                                if (installLocation != null)
                                {
                                    string vlcExePath = Path.Combine(installLocation, "vlc.exe");
                                    if (File.Exists(vlcExePath))
                                    {
                                        return vlcExePath;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    #endif

        return null;
    }

    private static string? FindVlcInstallationLinux()
    {
        // Check common installation directories on Linux
        string[] possiblePaths =
        {
            "/usr/bin/vlc",
            "/usr/local/bin/vlc",
            "/opt/vlc/vlc"
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        // Use the 'which' command to find VLC
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = "vlc",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (!string.IsNullOrEmpty(result))
            {
                return result.Trim();
            }
        }
        catch
        {
            // Handle exception if needed
        }

        return null;
    }
}