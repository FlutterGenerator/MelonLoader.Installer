﻿using Semver;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace MelonLoader.Installer;

public class MLVersion
{
    public string? DownloadUrlWin { get; init; }
    public string? DownloadUrlWinX86 { get; init; }
    public string? DownloadUrlLinux { get; init; }
    public required SemVersion Version { get; init; }
    public bool IsLocalPath { get; init; }

    public static SemVersion? GetMelonLoaderVersion(string gameDir, out Architecture architecture)
    {
        architecture = Architecture.Unknown;

        var mlDir = Path.Combine(gameDir, "MelonLoader");
        if (!Directory.Exists(mlDir))
            return null;

        var mlAssemblyPath = Path.Combine(mlDir, "MelonLoader.dll");
        if (!File.Exists(mlAssemblyPath))
        {
            mlAssemblyPath = Path.Combine(mlDir, "MelonLoader.ModHandler.dll");
            if (!File.Exists(mlAssemblyPath))
            {
                mlAssemblyPath = Path.Combine(mlDir, "net35", "MelonLoader.dll");
                if (!File.Exists(mlAssemblyPath))
                {
                    mlAssemblyPath = Path.Combine(mlDir, "net6", "MelonLoader.dll");
                    if (!File.Exists(mlAssemblyPath))
                        mlAssemblyPath = null;
                }
            }
        }
        if (mlAssemblyPath == null)
            return null;

        SemVersion version;
        try
        {
            var fileVersionRaw = FileVersionInfo.GetVersionInfo(mlAssemblyPath).FileVersion!;
            var fileVersion = System.Version.Parse(fileVersionRaw);
            version = SemVersion.ParsedFrom(fileVersion.Major, fileVersion.Minor, fileVersion.Build,
                fileVersion.Revision <= 0 ? string.Empty : $"ci.{fileVersion.Revision}");
        }
        catch
        {
            return null;
        }

        var proxyPath = MLManager.proxyNames.FirstOrDefault(x => File.Exists(Path.Combine(gameDir, x)));
        if (proxyPath == null)
            return null;

        proxyPath = Path.Combine(gameDir, proxyPath);
        if (proxyPath.EndsWith(".so"))
        {
            architecture = Architecture.LinuxX64;
            return version;
        }

        try
        {
            using var proxyStr = File.OpenRead(proxyPath);
            var pe = new PEReader(proxyStr);
            architecture = pe.PEHeaders.CoffHeader.Machine == Machine.Amd64 ? Architecture.WindowsX64 : Architecture.WindowsX86;
            return version;
        }
        catch
        {
            return null;
        }
    }

    public override string ToString()
    {
        var name = Version.ToString();
        if (IsLocalPath)
            name += " (Local Build)";

        return name;
    }
}
