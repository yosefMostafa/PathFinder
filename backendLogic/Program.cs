// See https://aka.ms/new-console-template for more information


using backendLogic.src.searchEngine.models;

Console.WriteLine("🚀 PathFinder - Everything SDK Metadata Test");
Console.WriteLine("=" + new string('=', 50));

ProjectType search = new PDF();
await search.RunSearch();
 
// string name = typeof(Program).Namespace ?? "None!";
// Console.WriteLine($"Namespace: {name}");