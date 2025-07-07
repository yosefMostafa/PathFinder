using System;
using System.Diagnostics;
using System.Text.Json;

namespace backendLogic.src.searchEngine.models
{

    public class Node : ProjectType
    {


        public Node() : base("package.json", new List<EverythingResult>())
        {

        }

        private void FilterResults()
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
                if (!FolderType( _searchResults[i]))
                    continue;
                
                // if (!Directory.Exists(nodeModulesPath))
                //     continue;
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
        private bool FolderType( EverythingResult result)
        {
            string json;
            JsonDocument doc;
            JsonElement root;
            try
            {
                json = File.ReadAllText(result.FullPath);
                doc = JsonDocument.Parse(json);
                root = doc.RootElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading or parsing {result.FullPath}: {ex.Message}");
                return false;
            }

            root.TryGetProperty("dependencies", out JsonElement dependencies);
            root.TryGetProperty("scripts", out JsonElement scripts);
            // root.TryGetProperty("devDependencies", out JsonElement devDependencies);
            if (!dependencies.ValueKind.Equals(JsonValueKind.Object) || !scripts.ValueKind.Equals(JsonValueKind.Object))
            {
                return false;
            }
            if (dependencies.TryGetProperty("react", out _) || scripts.ToString().Contains("react-scripts"))
            {
               result.FolderType = "React";
                result.FolderIcon = "./assets/react-icon.png"; // Example icon, replace with actual path
            }
            else if (dependencies.TryGetProperty("@angular/core", out _))
            {
                result.FolderType = "Angular";
                result.FolderIcon = "./assets/angular-icon.png"; // Example icon, replace with actual path
            }
            else if (dependencies.TryGetProperty("vue", out _))
            {
                result.FolderType = "Vue";
                result.FolderIcon = "./assets/vue-icon.png"; // Example icon, replace with actual path
            }
            else if (dependencies.TryGetProperty("express", out _))
            {
                result.FolderType = "Express";
                result.FolderIcon = "./assetsreact-icon.png"; // Example icon, replace with actual path
            }
            else if (dependencies.TryGetProperty("next", out _))
            {
                result.FolderType = "Next.js";
                result.FolderIcon = "./assets/nextjs-icon.png"; // Example icon, replace with actual

            }
            else
            {
                Debug.WriteLine($"No recognized framework found in {result.FullPath}");
                return false;
            }
            return true;
        }
        public override bool RequirFolderSize()
        {
            return true; // Node.js projects often require folder size calculations
        }
        public override void RunLogic()
        {
            Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
            Console.WriteLine("Filtering results...");
            FilterResults();
            Console.WriteLine($"Filtered results. Remaining {_searchResults.Count} results.");
            Console.WriteLine("-------------------");
        }


    }
}
