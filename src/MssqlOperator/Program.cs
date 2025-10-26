using DatabaseMetadataService = MssqlOperator.Services.DatabaseMetadataService;
using OutputFormatter = MssqlOperator.CLI.OutputFormatter;
using DatabaseOptions = MssqlOperator.CLI.DatabaseOptions;
using TableOptions = MssqlOperator.CLI.TableOptions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Microsoft.Extensions.Configuration;
using CommandLine;
using CommandLine.Text;

namespace MssqlOperator;

class Program
{
    static void Main(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = null);
        
        var result = parser.ParseArguments<DatabaseOptions, TableOptions>(args);
        
        result
            .WithParsed<DatabaseOptions>(async (options) =>
            {
            
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
            })
            .WithParsed<TableOptions>((options) =>
            {
                Console.WriteLine($"Tables for database: {options.Database}");
                Console.WriteLine("(Not implemented yet)");
            })
            .WithNotParsed(errs => DisplayHelp(result, errs));
    }
    
    private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        var help = HelpText.AutoBuild(result, h =>
        {
            h.Heading = "MSSQL Operator 0.1.0";
            h.Copyright = "Copyright (c) 2025 Cortland Goffena";
            h.AdditionalNewLineAfterOption = false;
            return h;
        }, e => e);
        Console.WriteLine(help);
    }
}