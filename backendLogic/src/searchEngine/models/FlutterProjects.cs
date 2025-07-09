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
            if(fullPath.Contains("cache") || fullPath.Contains("temp"))
                continue;
            filteredResults.Add(result);
        }
        _searchResults = filteredResults;
    }

    public override void RunLogic()
    {
        Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
        Console.WriteLine("Filtering results...");
        FilterResults();
        Console.WriteLine($"Filtered results count: {_searchResults.Count}");
    }
}
