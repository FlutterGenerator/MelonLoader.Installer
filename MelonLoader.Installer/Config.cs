﻿namespace MelonLoader.Installer;

internal static class Config
{
    public static string DotnetRuntimeX64Download { get; private set; } = "https://aka.ms/dotnet/6.0/dotnet-runtime-win-x64.exe";
    public static string DotnetRuntimeX86Download { get; private set; } = "https://aka.ms/dotnet/6.0/dotnet-runtime-win-x86.exe";
    public static string MelonLoaderReleasesApi { get; private set; } = "https://api.github.com/repos/LavaGang/MelonLoader/releases";
    public static string MelonLoaderReleaseDownload { get; private set; } = "https://github.com/LavaGang/MelonLoader/releases/download";
    public static string MelonLoaderBuildWorkflowApi { get; private set; } = "https://api.github.com/repos/LavaGang/MelonLoader/actions/workflows/5411546/runs?branch=alpha-development&event=push&status=success&per_page=5";
    public static string ConfigsDir { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MelonLoader Installer");
    public static string GameListPath { get; private set; } = Path.Combine(ConfigsDir, "games.txt");

    public static string[] LoadGameList()
    {
        if (!File.Exists(GameListPath))
            return [];

        return File.ReadAllLines(GameListPath);
    }

    public static void SaveGameList(IEnumerable<string> gamePaths)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(GameListPath)!);
        File.WriteAllLines(GameListPath, gamePaths);
    }
}
