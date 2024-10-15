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

var generator = new NSwagGenerator(server, targetFolder);

Console.WriteLine("Generating C# client...");
await generator.GenerateCSharp();

Console.WriteLine("Generating TypeScript types...");
await generator.GenerateTypescript();

Console.WriteLine("DONE");
