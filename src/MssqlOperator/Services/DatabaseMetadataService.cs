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
            collation_name,
            compatibility_level
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
                Collation = reader.GetString(reader.GetOrdinal("collation_name")),
                CompatibilityLevel = Convert.ToInt32(reader["compatibility_level"])
            });
        }
        
        return databases;
    }
}
