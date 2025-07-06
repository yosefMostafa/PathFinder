
namespace backendLogic.src.searchEngine.models
{
    public class PDF : ProjectType
    {
        public PDF() : base("*.pdf", new List<EverythingResult>()) { }
        public override void RunLogic()
        {
            Console.WriteLine($"Search completed. Found {_searchResults.Count} results.");
            Console.WriteLine("-------------------");

        }
    }
}
