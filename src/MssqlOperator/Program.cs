using DatabaseMetadataService = MssqlOperator.Services.DatabaseMetadataService;
using OutputFormatter = MssqlOperator.CLI.OutputFormatter;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Microsoft.Extensions.Configuration;

namespace MssqlOperator;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("MSSQL Operator - Database List");
        Console.WriteLine("==============================");
        
        // Build configuration - look in multiple locations
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("src/MssqlOperator/appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string from configuration
        string connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found in configuration");
        
        try
        {
            var service = new DatabaseMetadataService();
            var databases = await service.GetDatabasesAsync(connectionString);
            OutputFormatter.DisplayDatabases(databases);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
