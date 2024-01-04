using System.Diagnostics;

const string path = @"..\..\..\_site";

Console.WriteLine(Environment.CurrentDirectory);

using Process cmd = new()
{
    StartInfo = new("docfx", $"serve {path}")
    {
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        UseShellExecute = false
    }
};

// Requires docfx as a global tool. Install with "dotnet tool install -g docfx", otherwise will throw a file not found exception
Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true });

cmd.Start();

string? line = null;

while ((line = cmd.StandardOutput.ReadLine()) is not null)
    Console.WriteLine(line);

cmd.WaitForExit();
