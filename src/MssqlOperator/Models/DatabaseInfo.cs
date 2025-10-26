namespace MssqlOperator.Models;

public class DatabaseInfo
{
    public required string Name { get; set; }
    public int DatabaseId { get; set; }
    public DateTime CreateDate { get; set; }
    public int CompatibilityLevel { get; set; }
    public required string CollationName { get; set; }
    public required string UserAccessDesc { get; set; }
    public required string StateDesc { get; set; }
    public required string SnapshotIsolationStateDesc { get; set; }
    public required string RecoveryModelDesc { get; set; }
    public bool IsCdcEnabled { get; set; }
}
