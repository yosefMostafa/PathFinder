// See https://aka.ms/new-console-template for more information


using backendLogic.src.searchEngine;
using backendLogic.src.searchEngine.models;

Console.WriteLine("🚀 PathFinder - Everything SDK Metadata Test");
Console.WriteLine("=" + new string('=', 50));

Engine engine = new Engine(ProjectTypeEnum.Node);
await engine.Run();
 
// string name = typeof(Program).Namespace ?? "None!";
// Console.WriteLine($"Namespace: {name}");