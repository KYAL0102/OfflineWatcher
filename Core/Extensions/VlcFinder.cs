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
            Path.Combine(ImportController.RootDirectory, "vlc-3.0.21_windows", "vlc.exe"),
            @"C:\Program Files\VideoLAN\VLC\vlc.exe",
            @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe"
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"Found vlc.exe at '{path}'.");
                return path;
            }
        }

        return null;
    }

    private static string? FindVlcInstallationLinux()
    {
        // Check common installation directories on Linux
        string[] possiblePaths =
        {
            Path.Combine(ImportController.RootDirectory, "vlc_linux", "vlc"),
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