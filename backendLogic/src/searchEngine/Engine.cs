

using System.Threading.Tasks;
using backendLogic.src.searchEngine.models;
using System.Security.Principal;


namespace backendLogic.src.searchEngine
{
    public class Engine(ProjectTypeEnum projectType)
    {

        private EverythingRepo everythingRepo = new EverythingRepo();
        private ProjectType ProjectType = new ProjectBuilder(projectType).Project;

        private async Task StartSearch(string search)
        {
            await everythingRepo.SetSearch(search); // 🔍 search term

        }
        public List<EverythingResult> GetSearchResults()
        {

            return everythingRepo.GetSearchResults();
        }
        public List<EverythingResult> GetRunResult()
        {
            return ProjectType.GetSearchResults();
        }
        public async Task Run()
        {
            Console.WriteLine("Running search for: " + ProjectType.GetSearchString());
            await StartSearch(ProjectType.GetSearchString());
            List<EverythingResult> _searchResults = everythingRepo.GetSearchResults();
            ProjectType.SetSearchResults(_searchResults.Select(result => result.Clone()).ToList());
            ProjectType.RunLogic();
            _searchResults = ProjectType.GetSearchResults();
            if (ProjectType.RequirFolderSize())
            {
                Console.WriteLine("Calculating folder sizes...");
                for (int i = 0; i < _searchResults.Count; i++)
                {
                    EverythingResult result = _searchResults[i];
                    result.pathSize = getFolderSize(result.Path ?? string.Empty);
                    _searchResults[i] = result;
                }
                ProjectType.SetSearchResults(_searchResults.Select(result => result.Clone()).ToList());
                Console.WriteLine("Folder sizes calculated.");
            }
            ProjectType.PrintResults();

            Console.WriteLine("-------------------");

        }
        public string getFolderSize(string folderPath)
        {
            if (!folderPath.EndsWith("\\"))
            {
                folderPath = folderPath + "\\";
            }

            return everythingRepo.GetFolderSize(folderPath);
        }

    }
}
