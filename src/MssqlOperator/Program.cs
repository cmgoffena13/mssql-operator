using DatabaseMetadataService = MssqlOperator.Services.DatabaseMetadataService;
using OutputFormatter = MssqlOperator.CLI.OutputFormatter;
using DatabaseOptions = MssqlOperator.CLI.DatabaseOptions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Microsoft.Extensions.Configuration;
using CommandLine;

namespace MssqlOperator;

class Program
{
    static async Task Main(string[] args)
    {
        // Show help if no arguments provided
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }
        
        var result = Parser.Default.ParseArguments<DatabaseOptions>(args);
        
        if (result.Tag == ParserResultType.Parsed)
        {
            var options = ((Parsed<DatabaseOptions>)result).Value;
            
            try
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

                var service = new DatabaseMetadataService();
                var databases = await service.GetDatabasesAsync(connectionString, maxRetries: 3, retryDelayMs: 1000);
                
                if (options.ShowAll)
                {
                    // Show all databases without selection
                    OutputFormatter.DisplayDatabases(databases);
                }
                else if (!string.IsNullOrEmpty(options.Database))
                {
                    // Show specific database
                    var selectedDb = databases.FirstOrDefault(db => 
                        string.Equals(db.Name, options.Database, StringComparison.OrdinalIgnoreCase));
                    
                    if (selectedDb != null)
                    {
                        Console.WriteLine($"Database: {selectedDb.Name}");
                        Console.WriteLine("==============================");
                        Console.WriteLine($"ID: {selectedDb.DatabaseId}");
                        Console.WriteLine($"Created: {selectedDb.CreateDate:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"Compatibility: {selectedDb.CompatibilityLevel}");
                        Console.WriteLine($"Collation: {selectedDb.CollationName}");
                        Console.WriteLine($"User Access: {selectedDb.UserAccessDesc}");
                        Console.WriteLine($"State: {selectedDb.StateDesc}");
                        Console.WriteLine($"Snapshot Isolation: {selectedDb.SnapshotIsolationStateDesc}");
                        Console.WriteLine($"Recovery Model: {selectedDb.RecoveryModelDesc}");
                        Console.WriteLine($"CDC Enabled: {selectedDb.IsCdcEnabled}");
                        Console.WriteLine($"Change Tracking: {(selectedDb.IsChangeTrackingEnabled == true ? "Enabled" : "Disabled")}");
                        if (selectedDb.IsChangeTrackingEnabled == true)
                        {
                            Console.WriteLine($"  Auto Cleanup: {selectedDb.IsChangeTrackingAutoCleanupOn}");
                            Console.WriteLine($"  Retention: {selectedDb.ChangeTrackingRetentionPeriod} {selectedDb.ChangeTrackingRetentionPeriodUnitsDesc}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Database '{options.Database}' not found.");
                        Console.WriteLine("Available databases:");
                        foreach (var db in databases)
                        {
                            Console.WriteLine($"  - {db.Name}");
                        }
                    }
                }
                else
                {
                    // Interactive selection (default behavior)
                    var selectedDb = OutputFormatter.SelectDatabase(databases);
                    if (selectedDb != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Selected: {selectedDb.Name}");
                        Console.WriteLine("==============================");
                        Console.WriteLine($"ID: {selectedDb.DatabaseId}");
                        Console.WriteLine($"Created: {selectedDb.CreateDate:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"Compatibility: {selectedDb.CompatibilityLevel}");
                        Console.WriteLine($"Collation: {selectedDb.CollationName}");
                        Console.WriteLine($"User Access: {selectedDb.UserAccessDesc}");
                        Console.WriteLine($"State: {selectedDb.StateDesc}");
                        Console.WriteLine($"Snapshot Isolation: {selectedDb.SnapshotIsolationStateDesc}");
                        Console.WriteLine($"Recovery Model: {selectedDb.RecoveryModelDesc}");
                        Console.WriteLine($"CDC Enabled: {selectedDb.IsCdcEnabled}");
                        Console.WriteLine($"Change Tracking: {(selectedDb.IsChangeTrackingEnabled == true ? "Enabled" : "Disabled")}");
                        if (selectedDb.IsChangeTrackingEnabled == true)
                        {
                            Console.WriteLine($"  Auto Cleanup: {selectedDb.IsChangeTrackingAutoCleanupOn}");
                            Console.WriteLine($"  Retention: {selectedDb.ChangeTrackingRetentionPeriod} {selectedDb.ChangeTrackingRetentionPeriodUnitsDesc}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No database selected. Goodbye!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }
        else
        {
            // Show help when parsing fails
            ShowHelp();
        }
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("MSSQL Operator");
        Console.WriteLine("==============");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  mssql-operator database [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  database    Database information");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -n, --name <database>    Specific database name to show details for");
        Console.WriteLine("  -a, --all                Show all databases without selection menu");
        Console.WriteLine("  -h, --help               Show help information");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  mssql-operator database                    # Interactive selection");
        Console.WriteLine("  mssql-operator database --all             # Show all databases");
        Console.WriteLine("  mssql-operator database --name master      # Show specific database");
    }
}