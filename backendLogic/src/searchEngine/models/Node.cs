using System;

namespace backendLogic.src.searchEngine.models
{

    public class Node : ProjectType
    {
     

        public Node()
        {
            _searchString = "package.json";
            _engine = new Engine();
            _searchResults = new List<EverythingResult>();
        }
       
        public void FilterResults()
        {
            List<EverythingResult> filteredResults = new List<EverythingResult>();
            for (int i = 0; i < _searchResults.Count; i++)
            {
                string fullPath = _searchResults[i].FullPath;
                // if (fullPath.Split(Path.DirectorySeparatorChar).Count(part => part == "node_modules") > 1)
                //     continue;
                if (fullPath.Contains("node_modules"))
                    continue;
                //remove any path that is in c/user's or c/programfiles /c/program files (x86) or c/windows
                if (fullPath.StartsWith("C:\\Users") || fullPath.StartsWith("C:\\Program Files") || fullPath.StartsWith("C:\\Program Files (x86)") || fullPath.StartsWith("C:\\Windows"))
                    continue;
                var parentDirectory = Directory.GetParent(fullPath);
                if (parentDirectory == null)
                    continue;
                string nodeModulesPath = Path.Combine(parentDirectory.FullName, "node_modules");

                // Check if the directory exists
                if (!Directory.Exists(nodeModulesPath))
                    continue;
                if (parentDirectory != null)
                {
                    string projectFolder = parentDirectory.FullName;
                    if (!string.IsNullOrEmpty(projectFolder))
                    {
                        filteredResults.Add(_searchResults[i]);
                    }
                }
            }
            _searchResults = filteredResults;
        }


        public override async Task<List<EverythingResult>> RunSearch()
        {
            Console.WriteLine("Running search for: " + _searchString);
            await _engine.StartSearch(_searchString);
            _searchResults = _engine.GetSearchResults();
            Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
            Console.WriteLine("Filtering results...");
            FilterResults();
            Console.WriteLine($"Filtered results. Remaining {_searchResults.Count} results.");
            Console.WriteLine("Calculating folder sizes...");
            for (int i = 0; i < _searchResults.Count; i++)
            {
                EverythingResult result = _searchResults[i];
                result.pathSize = _engine.getFolderSize(result.Path ?? string.Empty);
                _searchResults[i] = result;
            }
            Console.WriteLine("Folder sizes calculated.");
            Console.WriteLine("-------------------");

            for (int i = 0; i < _searchResults.Count; i++)
            {
                _searchResults[i].Print();
            }

            return GetSearchResults();
        }

       
    }
}
