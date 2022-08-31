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

Process.Start(new ProcessStartInfo("http://localhost:8080") { UseShellExecute = true });

cmd.Start();

string? line = null;

while ((line = cmd.StandardOutput.ReadLine()) is not null)
    Console.WriteLine(line);

cmd.WaitForExit();
