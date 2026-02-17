using HerePlatform.RestClient.Docs.Generator;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- <wwwroot-path>");
    return 1;
}

var wwwrootPath = args[0];
var dataDir = Path.Combine(wwwrootPath, "data");
Directory.CreateDirectory(dataDir);

var outputPath = Path.Combine(dataDir, "rest-api-docs.json");
var generator = new RestApiDocGenerator();
await generator.GenerateAsync(outputPath);

return 0;
