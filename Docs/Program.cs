using System.Diagnostics;

using Docfx;
using Docfx.Dotnet;

var dir = Environment.GetEnvironmentVariable("SiteDir");
var config = dir + @"\docfx.json";

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

Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true });

cmd.Start();

string? line = null;

while ((line = cmd.StandardOutput.ReadLine()) is not null)
    Console.WriteLine(line);

cmd.WaitForExit();
