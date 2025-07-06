using System.Runtime.InteropServices.Marshalling;

namespace backendLogic.src.searchEngine.models
{

    public abstract class ProjectType
    {
        protected string _searchString ;

        protected List<EverythingResult> _searchResults;

        protected ProjectType(string searchString ,List<EverythingResult> searchResults)
        {
            _searchString = searchString;
            _searchResults = searchResults;
        }

        public void SetSearchResults(List<EverythingResult> results)
        {
            _searchResults = results;
        }
        public string GetSearchString()
        {
            return _searchString;
        }
        public List<EverythingResult> GetSearchResults()
        {
            List<EverythingResult> _temp = new List<EverythingResult>();
            foreach (var result in _searchResults)
            {
                _temp.Add(result.Clone());
            }
            return  _temp;
        }
        public virtual bool RequirFolderSize()
        {
            return false;
        }
        public abstract void RunLogic();
        public virtual void PrintResults()
        {
            Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
            Console.WriteLine("-------------------");
            foreach (var result in _searchResults)
            {
                result.Print();
            }
        }


    }
}