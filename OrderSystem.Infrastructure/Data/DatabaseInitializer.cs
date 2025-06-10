using Dapper;
using Microsoft.Data.SqlClient;

public static class DatabaseInitializer
{
    public static void InitializeDatabase(string connectionString)
    {
        string scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "DatabaseSetup.sql");

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"SQL script not found at: {scriptPath}");
        }

        string script = File.ReadAllText(scriptPath);
    }
}
