using System.Diagnostics;
using System.Text.Json;

using Docfx;
using Docfx.Dotnet;

// Run this project to build and deploy the documentation website to preview on localhost. Website generated with DocFX https://dotnet.github.io/docfx/
// The API reference section is defined by yaml files in the /api directory - These files are generated from XML documentation in the code and should not be manually modified! (therefore are gitignored)
// The Articles section is defined by markdown files in the /articles directory.

// If localhost returns 404, try running `dotnet tool restore` from the project directory.

const string
	siteDirEnv = "SiteDir",
	libPathEnv = "LibPath";

#region Initialize
string?
	siteDir = Environment.GetEnvironmentVariable(siteDirEnv),
	libPath = Environment.GetEnvironmentVariable(libPathEnv);

if (!ValidateEnv(siteDirEnv, siteDir))
	return -1;

if (!ValidateEnv(siteDirEnv, siteDir))
	return -1;

string
	configPath	 = siteDir + "docfx.json",
	siteBuildDir = siteDir + "_site";

if (!File.Exists(configPath))
{
	Console.WriteLine($"Required file `{configPath}` is missing");
	return -1;
}
#endregion

#region Purge
if (Directory.Exists(siteBuildDir))
{
	PrintStatus("Purging files from previous build");
	Directory.Delete(siteBuildDir, true);
}
#endregion

#region Analyze
if (!File.Exists(libPath))
{
	// Shouldn't happen - Docs is configured to depend on the lib
	Console.WriteLine($"Assembly `{libPath}` missng.");
	return -1;
}

string cachePath         = "cache.json";
bool shouldAnalyze       = true;
DateTime libLastModified = File.GetLastWriteTime(libPath);

if (File.Exists(cachePath))
{
	string json;

	try { json = File.ReadAllText(cachePath); }
	catch (Exception ex)
	{
		Console.WriteLine("Error reading build cache:");
		Console.WriteLine(ex);

		return -1;
	}

	BuildCache? cache;

	try { cache = JsonSerializer.Deserialize<BuildCache>(File.ReadAllText(cachePath)); }
	catch (Exception ex)
	{
		Console.WriteLine("Error parsing build cache:");
		Console.WriteLine(ex);

		return -1;
	}

	if (cache is not null && cache.LibLastModified >= libLastModified)
		shouldAnalyze = false;
}

if (shouldAnalyze)
{
	File.WriteAllText(cachePath, JsonSerializer.Serialize(new BuildCache(libLastModified)));

	PrintStatus("Analysing assembly with DocFx");

	try { await DotnetApiCatalog.GenerateManagedReferenceYamlFiles(configPath); }
	catch (Exception ex)
	{
		Console.WriteLine("Analysis error:");
		Console.WriteLine(ex);

		return -1;
	}

	PrintStatus("Analysis done");
}
else
	PrintStatus("Assembly unchanged since last build, skipping analysis");
#endregion

#region Build
PrintStatus("Building site with DocFx");

try { await Docset.Build(configPath); }
catch (Exception ex)
{
	Console.WriteLine("Build error:");
	Console.Write(ex);

	return -1;
}

PrintStatus("Build done");
#endregion

#region Serve
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
#endregion

return 0;

static void PrintStatus(string status)
{
	Console.WriteLine($"------- {status} -------");
	Console.WriteLine();
}

static bool ValidateEnv(string env, string? value)
{
	if (string.IsNullOrEmpty(value))
	{
		Console.WriteLine($"Required environment variable `{env}` is misconfigured in launchsettings.json");
		return false;
	}

	return true;
}

record class BuildCache(DateTime LibLastModified);
