using DatabaseInfo = MssqlOperator.Models.DatabaseInfo;

namespace MssqlOperator.CLI;

public class OutputFormatter
{
    public static void DisplayDatabases(List<DatabaseInfo> databases)
    {
        Console.WriteLine($"Found {databases.Count} databases:");
        Console.WriteLine();

        foreach (var db in databases)
        {
            Console.WriteLine($"Database: {db.Name}");
            Console.WriteLine($"  ID: {db.DatabaseId}");
            Console.WriteLine($"  Created: {db.CreateDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Compatibility: {db.CompatibilityLevel}");
            Console.WriteLine($"  Collation: {db.CollationName}");
            Console.WriteLine($"  User Access: {db.UserAccessDesc}");
            Console.WriteLine($"  State: {db.StateDesc}");
            Console.WriteLine($"  Snapshot Isolation: {db.SnapshotIsolationStateDesc}");
            Console.WriteLine($"  Recovery Model: {db.RecoveryModelDesc}");
            Console.WriteLine($"  CDC Enabled: {db.IsCdcEnabled}");
            Console.WriteLine($"  Change Tracking: {(db.IsChangeTrackingEnabled == true ? "Enabled" : "Disabled")}");
            if (db.IsChangeTrackingEnabled == true)
            {
                Console.WriteLine($"    Auto Cleanup: {db.IsChangeTrackingAutoCleanupOn}");
                Console.WriteLine($"    Retention: {db.ChangeTrackingRetentionPeriod} {db.ChangeTrackingRetentionPeriodUnitsDesc}");
            }
            Console.WriteLine();
        }
    }

    public static void DisplayDatabaseList(List<DatabaseInfo> databases)
    {
        Console.WriteLine($"Found {databases.Count} databases:");
        Console.WriteLine();

        for (int i = 0; i < databases.Count; i++)
        {
            var db = databases[i];
            var ctStatus = db.IsChangeTrackingEnabled == true ? "✓" : "✗";
            var cdcStatus = db.IsCdcEnabled ? "✓" : "✗";
            Console.WriteLine($"{i + 1}. {db.Name} (ID: {db.DatabaseId}) - {db.StateDesc} - CT: {ctStatus} CDC: {cdcStatus}");
        }
        Console.WriteLine();
    }

    public static DatabaseInfo? SelectDatabase(List<DatabaseInfo> databases)
    {
        while (true)
        {
            DisplayDatabaseList(databases);
            
            Console.Write("Select database (number) or 'ctrl + c' to quit: ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No selection made. Exiting.");
                return null;
            }
                
            if (int.TryParse(input, out int selection))
            {
                if (selection >= 1 && selection <= databases.Count)
                {
                    return databases[selection - 1];
                }
            }
            
            Console.WriteLine("Invalid selection. Please try again.");
            Console.WriteLine();
        }
    }
}
