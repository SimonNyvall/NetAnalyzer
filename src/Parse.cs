using System.Text.RegularExpressions;
using System.Diagnostics;

class Parse
{
    public static Solution Solution(string vulnerabilityText, string outdatedText, string path)
    {
        var solution = new Solution();
        var projects = new List<Project>();
        var solutionText = outdatedText;

        string[] lines = solutionText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        solution.Sources = ParseSouces(lines);

        Project? currentProject = null;

        foreach (string line in lines)
        {
            if (line.Contains("[net"))
            {
                if (currentProject != null)
                {
                    projects.Add(currentProject);
                }

                currentProject = new()
                {
                    DotnetVersion = line
                        .Trim()
                        .Replace("[", "")
                        .Replace("]", "")
                        .Replace(":", "")
                };
            }
        }

        if (currentProject != null)
        {
            currentProject.Warnings = ParseWarnings(path);
            projects.Add(currentProject);
        }

        solution.Projects = projects;

        foreach (var project in solution.Projects)
        {
            var packages = ParsePackages(vulnerabilityText, outdatedText);
            project.Packages.AddRange(packages);
        }

        return solution;
    }

    private static List<string> ParseSouces(string[] lines)
    {
        int projectIndex = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            projectIndex++;

            if (lines[i].Contains("Project")) break;
        }

        List<string> sources = [.. lines.Take(projectIndex)];

        return [.. sources.TakeLast(sources.Count - 1).Select(x => x.Trim())];
    }

    private static List<Warning> ParseWarnings(string projectPath)
    {
        var warnings = new List<Warning>();

        using (Process process = new())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "build";
            process.StartInfo.WorkingDirectory = projectPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            string fullOutput = output + "\n" + error;

            string warningPattern = @"(.*?):\s*warning\s+(CS\d+):\s*(.*)";
            string[] lines = fullOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, warningPattern);

                if (match.Success)
                {
                    warnings.Add(new Warning
                    {
                        Identifier = match.Groups[2].Value,
                        Text = match.Groups[3].Value
                    });
                }
            }
        }

        return warnings;
    }

    private static Package[] ParsePackages(string vulnerabilityText, string outdatedText)
    {
        List<Package> packages = [];

        var outdatedPackages = ParseOutdatedPackages(outdatedText);

        var vulnerabilities = ParseVulnerabilities(vulnerabilityText);

        foreach (var outdatedPackage in outdatedPackages)
        {
            var package = new Package
            {
                Name = outdatedPackage.Name,
                Outdated = outdatedPackage.Outdated,
                Vulnerabilitys = vulnerabilities.FindAll(v => v.Resolved == outdatedPackage.Outdated.Resolved)
            };
            packages.Add(package);
        }

        return [.. packages];
    }

    private static List<Package> ParseOutdatedPackages(string text)
    {
        var outdatedPackages = new List<Package>();

        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string outdatedPattern = @">\s*([A-Za-z0-9.\-]+)\s+([0-9.]+)\s+([0-9.]+)\s+([0-9.]+)";
            Match match = Regex.Match(line, outdatedPattern);

            if (!match.Success) continue;

            Package package = new()
            {
                Name = match.Groups[1].Value,
                Outdated = new()
                {
                    Requested = match.Groups[2].Value,
                    Resolved = match.Groups[3].Value,
                    Latest = match.Groups[4].Value
                }
            };

            outdatedPackages.Add(package);
        }

        return outdatedPackages;
    }

    private static List<Vulnerability> ParseVulnerabilities(string text)
    {
        var vulnerabilities = new List<Vulnerability>();

        string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string vulnerabilityPattern = @">\s*([A-Za-z0-9.\-]+)\s+([0-9.]+)\s+(High|Medium|Low|Critical)\s+(https?://[^\s]+)";
            Match match = Regex.Match(line, vulnerabilityPattern);

            if (!match.Success) continue;

            vulnerabilities.Add(new()
            {
                Resolved = match.Groups[2].Value,
                Severity = match.Groups[3].Value,
                AdvisoryURL = match.Groups[4].Value
            });
        }

        return vulnerabilities;
    }
}