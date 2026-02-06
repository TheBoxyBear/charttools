using System.Diagnostics;

using Docfx;
using Docfx.Dotnet;

// Run this project to build and deploy the documentation website to preview on localhost. Website generated with DocFX https://dotnet.github.io/docfx/
// The API reference section is defined by yaml files in the /api directory - These files are generated from XML documentation in the code and should not be manually modified! (therefore are gitignored)
// The Articles section is defined by markdown files in the /articles directory.

// If localhost returns 404, try running `dotnet tool restore` from the project directory.

string
	siteDir      = Environment.GetEnvironmentVariable("SiteDir")!,
	configPath   = siteDir + "docfx.json",
	siteBuildDir = siteDir + "_site";

if (Directory.Exists(siteBuildDir))
{
	PrintStatus("Purging files from previous build");
	Directory.Delete(siteBuildDir, true);
}

PrintStatus("Analysing assembly with DocFx");

// TODO Only build api if the assembly is more recent than the last site build
await DotnetApiCatalog.GenerateManagedReferenceYamlFiles(configPath);

PrintStatus("Analysis done");

PrintStatus("Building site with DocFx");

await Docset.Build(configPath);

PrintStatus("Build done");

using Process cmd = new()
{
	StartInfo = new("dotnet ", @$"docfx serve {siteBuildDir}")
	{
		RedirectStandardInput  = true,
		RedirectStandardOutput = true,
		UseShellExecute        = false
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

static void PrintStatus(string status)
{
	Console.WriteLine($"------- {status} -------");
	Console.WriteLine();
}
