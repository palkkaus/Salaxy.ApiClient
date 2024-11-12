using Microsoft.Extensions.Configuration;
using Tools.ApiCodegen;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
string? server = config.GetSection("apiServer")?.Value;
if (string.IsNullOrEmpty(server))
{
    Console.WriteLine("apiServer is not configured in appsettings.json");
    return;
}
string targetFolder = config.GetSection("targetFolder")?.Value ?? "../../../../Client/codegen";


Console.WriteLine($"Start code generation from server '{server}' to 'targetFolder'");

Console.WriteLine("Generating Main C# client...");
var generator = new NSwagGenerator(server, targetFolder);

string? importServerUrl = config.GetSection("importServerUrl")?.Value;
Console.WriteLine($"Generating Import C# client from {importServerUrl}...");
if (string.IsNullOrEmpty(importServerUrl))
{
    Console.WriteLine("importServerUrl is not configured in appsettings.json");
    return;
}
else
{
    await generator.GenerateImportCSharp(importServerUrl);
}


// Console.WriteLine("Generating TypeScript types...");
// await generator.GenerateTypescript();

Console.WriteLine("DONE");
