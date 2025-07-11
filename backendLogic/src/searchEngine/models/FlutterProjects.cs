using System;

namespace backendLogic.src.searchEngine.models;

public class FlutterProjects : ProjectType
{
    public FlutterProjects() : base("pubsp*c.yaml", new List<EverythingResult>())
    {
    }
    private void FilterResults()
    {
        List<EverythingResult> filteredResults = new List<EverythingResult>();
        foreach (var result in _searchResults)
        {
            string fullPath = result.FullPath;
            if (fullPath.StartsWith("C:\\Users") || fullPath.StartsWith("C:\\Program Files (x86)") || fullPath.StartsWith("C:\\Windows"))
                continue;
            if (fullPath.Contains("cache") || fullPath.Contains("temp"))
                continue;

            if (IsFlutterInstalledPath(fullPath))
                continue; // or continue; depending on logic




            filteredResults.Add(result);
        }
        _searchResults = filteredResults;
    }
    private bool IsFlutterInstalledPath(string path)
    {
        int index = 0;
        while ((index = path.IndexOf("\\flutter\\", index, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            string flutterPathCandidate = path.Substring(0, index + "\\flutter\\".Length);
            index += "\\flutter\\".Length;
            if (File.Exists(Path.Combine(flutterPathCandidate, "bin", "flutter.bat")) ||
                File.Exists(Path.Combine(flutterPathCandidate, "bin", "flutter")))
                return true; // Found a valid Flutter installation path

        }
        return false; // No valid Flutter installation path found

    }

    public override bool RequirFolderSize()
    {
        return true;
    }

    public bool IsGamePath(string path)
    {
        var gameExe = Directory.EnumerateFiles(path, "*.exe", SearchOption.TopDirectoryOnly)
            .Any(f => Path.GetFileName(f).ToLower().Contains("game") || Path.GetFileName(f).ToLower().Contains("launcher"));

        var hasEngineFolders = Directory.Exists(Path.Combine(path, "Binaries")) ||
                               Directory.Exists(Path.Combine(path, "Content")) ||
                               Directory.Exists(Path.Combine(path, "Engine"));

        var hasUnityFiles = File.Exists(Path.Combine(path, "UnityPlayer.dll")) ||
                            Directory.EnumerateFiles(path, "*_Data", SearchOption.TopDirectoryOnly).Any();

        var hasGodot = File.Exists(Path.Combine(path, "project.godot"));
        var hasUnreal = Directory.EnumerateFiles(path, "*.uproject").Any();

        return gameExe || hasEngineFolders || hasUnityFiles || hasGodot || hasUnreal;
    }
    private bool IsPlugin(string fullPath)
    {


        // Read the file line by line
        foreach (var line in File.ReadLines(fullPath))
        {
            // Detect plugin by matching "flutter:" followed by indentation and "plugin:"
            if (line.Trim() == "plugin:" && WasPrecededByFlutter(fullPath, line))
            {
                return true;

            }

            // Optional shortcut: detect on one line (for weird YAMLs)
            if (line.Trim().StartsWith("plugin:") && line.Contains(":"))
            {
                return true;
            }
        }

        return false;


    }
    static bool WasPrecededByFlutter(string filePath, string pluginLine)
    {
        var lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() == "plugin:" && i > 0)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    var prevLine = lines[j].Trim();
                    if (string.IsNullOrEmpty(prevLine)) continue;

                    if (prevLine == "flutter:")
                        return true;
                    else
                        break;
                }
            }
        }
        return false;
    }
    public override void RunLogic()
    {
        Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
        Console.WriteLine("Filtering results...");
        FilterResults();
        Console.WriteLine($"Filtered results count: {_searchResults.Count}");
    }
}
