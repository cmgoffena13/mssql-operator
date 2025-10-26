using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;
using DatabaseInfo = MssqlOperator.Models.DatabaseInfo;

namespace MssqlOperator.Services;

public class DatabaseMetadataService
{
    private const string GetDatabasesQuery = @"
        SELECT
            name,
            database_id,
            create_date,
            compatibility_level,
            collation_name,
            user_access_desc,
            state_desc,
            snapshot_isolation_state_desc,
            recovery_model_desc,
            is_cdc_enabled
        FROM sys.databases";

    public async Task<List<DatabaseInfo>> GetDatabasesAsync(string connectionString)
    {
        var databases = new List<DatabaseInfo>();
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        using var command = new SqlCommand(GetDatabasesQuery, connection);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            databases.Add(new DatabaseInfo
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                DatabaseId = Convert.ToInt32(reader["database_id"]),
                CreateDate = reader.GetDateTime(reader.GetOrdinal("create_date")),
                CompatibilityLevel = Convert.ToInt32(reader["compatibility_level"]),
                CollationName = reader.GetString(reader.GetOrdinal("collation_name")),
                UserAccessDesc = reader.GetString(reader.GetOrdinal("user_access_desc")),
                StateDesc = reader.GetString(reader.GetOrdinal("state_desc")),
                SnapshotIsolationStateDesc = reader.GetString(reader.GetOrdinal("snapshot_isolation_state_desc")),
                RecoveryModelDesc = reader.GetString(reader.GetOrdinal("recovery_model_desc")),
                IsCdcEnabled = Convert.ToBoolean(reader["is_cdc_enabled"])
            });
        }
        
        return databases;
    }
}
