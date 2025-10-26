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
            Console.WriteLine($"  Collation: {db.Collation}");
            Console.WriteLine($"  Compatibility: {db.CompatibilityLevel}");
            Console.WriteLine();
        }
    }
}
