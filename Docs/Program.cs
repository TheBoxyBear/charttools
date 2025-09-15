using System.Diagnostics;

using Docfx;
using Docfx.Dotnet;

// Run this project to build and deploy the documentation website over localhost. Website generated with DocFX https://dotnet.github.io/docfx/
// The API reference section is defined by yaml files in the /api directory - These files are generated from XML documentation in the code and should not be manually modified! (therefore are gitignored)
// The Articles section is defined by markdown files in the /articles directory.

// This project is currently non-functional following the bump to .NET 9. An issue has been raised with the DocFX tea: https://github.com/dotnet/docfx/issues/10811

string? dir = Environment.GetEnvironmentVariable("SiteDir");
string? config = dir + @"\docfx.json";

var content = File.ReadAllText(config);

Console.WriteLine("------- Building site with DocFx -------");

// TODO Only build api if the assembly is more recent than the last site build
await DotnetApiCatalog.GenerateManagedReferenceYamlFiles(config);

await Docset.Build(config);

Console.WriteLine("------- Build done -------");
Console.WriteLine();

using Process cmd = new()
{
	StartInfo = new("dotnet", @$"docfx serve {dir}\_site")
	{
		RedirectStandardInput = true,
		RedirectStandardOutput = true,
		CreateNoWindow = true,
		UseShellExecute = false
	}
};

// Process must be closed with Ctrl-C or will remain open in the background blocking port 8080.
// If this happens (Windows):
// netstat -aof | findstr :8080
// taskkill / f / pid <PID>
Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true });

cmd.Start();

string? line = null;

while ((line = cmd.StandardOutput.ReadLine()) is not null)
	Console.WriteLine(line);

cmd.WaitForExit();
