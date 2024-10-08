class Solution
{
    public List<string> Sources { get; set; } = [];
    public List<Project> Projects { get; set; } = [];
}

class Project
{
    public string DotnetVersion { get; set; } = string.Empty;
    public List<Package> Packages { get; set; } = [];
    public List<Warning> Warnings { get; set; } = [];
}

class Warning
{
    public string Identifier { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

class Package
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Vulnerability> Vulnerabilitys { get; set; } = [];
    public Outdated Outdated { get; set; } = new();
}

class Vulnerability
{
    public string Resolved { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string AdvisoryURL { get; set; } = string.Empty;
}

class Outdated
{
    public string Requested { get; set; } = string.Empty;
    public string Resolved { get; set; } = string.Empty;
    public string Latest { get; set; } = string.Empty;
}