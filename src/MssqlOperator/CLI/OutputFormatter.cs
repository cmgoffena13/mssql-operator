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
            Console.WriteLine();
        }
    }
}
