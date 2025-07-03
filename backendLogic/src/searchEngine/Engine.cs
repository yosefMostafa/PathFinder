

using System.Threading.Tasks;
using backendLogic.src.searchEngine.models;
using System.Security.Principal;


namespace backendLogic.src.searchEngine
{
    public class Engine
    {

        private EverythingRepo everythingRepo = new EverythingRepo();
        public Engine()
        {
        }


        public async Task StartSearch(string search)
        {
            await everythingRepo.SetSearch(search); // 🔍 search term

        }
        public List<EverythingResult> GetSearchResults()
        {
            return everythingRepo.GetSearchResults();
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
