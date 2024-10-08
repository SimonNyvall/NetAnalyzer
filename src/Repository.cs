using Microsoft.Data.SqlClient;

class Repository
{
    private readonly string ConnectionString = "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;";

    public async Task AddSolution(Solution solution)
    {
        using SqlConnection connection = new(ConnectionString);

        await connection.OpenAsync();

        string inserSolutionQuery = "INSERT INTO Solutions (Source) OUTPUT INSERTED.Id VALUES (@Source)";
        int solutionId;


        
    }

    private async Task InsertSources()
    {

    }

    private async Task InsertProjects()
    {

    }

    private async Task InsertPackages()
    {

    }

    private async Task InsertVulnerabilities()
    {

    }

    private async Task InsertOutdatedPackages()
    {

    }
}