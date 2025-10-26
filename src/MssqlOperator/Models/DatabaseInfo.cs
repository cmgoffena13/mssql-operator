namespace MssqlOperator.Models;

public class DatabaseInfo
{
    public required string Name { get; set; }
    public int DatabaseId { get; set; }
    public required string Collation { get; set; }
    public int CompatibilityLevel { get; set; }
}
