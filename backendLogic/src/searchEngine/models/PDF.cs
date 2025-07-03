
namespace backendLogic.src.searchEngine.models
{
    public class PDF : ProjectType
    {
        public PDF()
        {
            _searchString = "*.pdf";
            _engine = new Engine();
            _searchResults = new List<EverythingResult>();
        }
        public override async Task<List<EverythingResult>> RunSearch()
        {
            Console.WriteLine("Running search for: " + _searchString);
            await _engine.StartSearch(_searchString);
            _searchResults = _engine.GetSearchResults();
            Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Console.WriteLine("Filtering results...");
            // // FilterResults();
            // Console.WriteLine($"Filtered results. Remaining {_searchResults.Count} results.");
            // Console.WriteLine("Calculating folder sizes...");
            // for (int i = 0; i < _searchResults.Count; i++)
            // {
            //     EverythingResult result = _searchResults[i];
            //     result.pathSize = _engine.getFolderSize(result.Path ?? string.Empty);
            //     _searchResults[i] = result;
            // }
            // Console.WriteLine("Folder sizes calculated.");
            Console.WriteLine("-------------------");

            for (int i = 0; i < _searchResults.Count; i++)
            {
                _searchResults[i].Print();
            }

            return GetSearchResults();
        }
    }
}
