// Make the parsing work correctly (warnings)
// Store the values in the db (MSSQL)
// Set up the docker compose and adjust the paths for the image and the paths

var solution = Parse.Solution(GetVulnerabilityText(), GetOutdatedText(), "/home/sn/Code/NetAnalyzer");

Console.ReadLine();

string GetVulnerabilityText() => File.ReadAllText("vulnerable_packages.test.txt");

string GetOutdatedText() => File.ReadAllText("outdated_packages.test.txt");






