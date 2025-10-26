using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;
using DatabaseInfo = MssqlOperator.Models.DatabaseInfo;

namespace MssqlOperator.Services;

public class DatabaseMetadataService
{
    private const string GetDatabasesQuery = @"
        SELECT
            d.name,
            d.database_id,
            d.create_date,
            d.compatibility_level,
            d.collation_name,
            d.user_access_desc,
            d.state_desc,
            d.snapshot_isolation_state_desc,
            d.recovery_model_desc,
            d.is_cdc_enabled,
            ct.is_auto_cleanup_on,
            ct.retention_period,
            ct.retention_period_units_desc
        FROM sys.databases d
        LEFT JOIN sys.change_tracking_databases ct 
            ON d.database_id = ct.database_id";

    public async Task<List<DatabaseInfo>> GetDatabasesAsync(string connectionString, int maxRetries = 3, int retryDelayMs = 1000)
    {
        return await RetryHelper.ExecuteWithRetryAsync(async () =>
        {
            return await GetDatabasesInternalAsync(connectionString);
        }, 
        maxRetries: maxRetries,
        delayMs: retryDelayMs);
    }

    private async Task<List<DatabaseInfo>> GetDatabasesInternalAsync(string connectionString)
    {
        var databases = new List<DatabaseInfo>();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(GetDatabasesQuery, connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            try
            {
                databases.Add(new DatabaseInfo
                {
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    DatabaseId = reader.GetInt32(reader.GetOrdinal("database_id")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("create_date")),
                    CompatibilityLevel = reader.GetByte(reader.GetOrdinal("compatibility_level")),
                    CollationName = reader.GetString(reader.GetOrdinal("collation_name")),
                    UserAccessDesc = reader.GetString(reader.GetOrdinal("user_access_desc")),
                    StateDesc = reader.GetString(reader.GetOrdinal("state_desc")),
                    SnapshotIsolationStateDesc = reader.GetString(reader.GetOrdinal("snapshot_isolation_state_desc")),
                    RecoveryModelDesc = reader.GetString(reader.GetOrdinal("recovery_model_desc")),
                    IsCdcEnabled = reader.GetBoolean(reader.GetOrdinal("is_cdc_enabled")),
                    IsChangeTrackingEnabled = reader.IsDBNull(reader.GetOrdinal("is_auto_cleanup_on")) ? null : reader.GetBoolean(reader.GetOrdinal("is_auto_cleanup_on")),
                    IsChangeTrackingAutoCleanupOn = reader.IsDBNull(reader.GetOrdinal("is_auto_cleanup_on")) ? null : reader.GetBoolean(reader.GetOrdinal("is_auto_cleanup_on")),
                    ChangeTrackingRetentionPeriod = reader.IsDBNull(reader.GetOrdinal("retention_period")) ? null : reader.GetInt32(reader.GetOrdinal("retention_period")),
                    ChangeTrackingRetentionPeriodUnitsDesc = reader.IsDBNull(reader.GetOrdinal("retention_period_units_desc")) ? null : reader.GetString(reader.GetOrdinal("retention_period_units_desc"))
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading database metadata for row {databases.Count + 1}: {ex.Message}", ex);
            }
        }

        return databases;
    }
}