// See https://aka.ms/new-console-template for more information


using backendLogic.src.searchEngine;

Console.WriteLine("Hello, World!");
Engine search = new Engine();
await search.StartSearch();
 
// string name = typeof(Program).Namespace ?? "None!";
// Console.WriteLine($"Namespace: {name}");