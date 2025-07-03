namespace backendLogic.src.searchEngine.models
{

    public abstract class ProjectType
    {
        protected string _searchString;
        protected Engine _engine;
        protected List<EverythingResult> _searchResults;

        public ProjectType()
        {
            _searchString = string.Empty;
            _engine = new Engine();
            _searchResults = new List<EverythingResult>();
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
            return new List<EverythingResult>(_searchResults);
        }
        public abstract Task<List<EverythingResult>> RunSearch();
       

    }
}