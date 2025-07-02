

using System.Threading.Tasks;

namespace backendLogic.src.searchEngine
{
    public class Engine
    {
        private string searchString = string.Empty;
        private EverythingApi everythingApi = new EverythingApi();
    public Engine()
        {
      }

        public void SetSearchString(string search)
        {
            searchString = search;
        }

        public string GetSearchString()
        {
            return searchString;
        }

        public async Task<HashSet<string>> StartSearch()
        {
            await everythingApi.EnsureIntialized();
            everythingApi.Main(); // 🔍 search term
            // Logic to start the search using the searchString
            Console.WriteLine($"Starting search for: {searchString}");
            // Call to Everything API or other search logic would go here
            return new HashSet<string>(); // Return results as a HashSet
        }

    }
}
