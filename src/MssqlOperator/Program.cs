using Microsoft.Data.SqlClient;

namespace MssqlOperator;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("MSSQL Operator - Database Connection Test");
        Console.WriteLine("==========================================");
        
        // Connection string for Docker SQL Server
        string connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;";
        
        try
        {
            Console.WriteLine("Attempting to connect to SQL Server...");
            
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            
            Console.WriteLine("✅ Successfully connected to SQL Server!");
            Console.WriteLine($"Server Version: {connection.ServerVersion}");
            Console.WriteLine($"Database: {connection.Database}");
            
            // Test a simple query
            using var command = new SqlCommand("SELECT @@VERSION", connection);
            var version = await command.ExecuteScalarAsync();
            Console.WriteLine($"SQL Server Version: {version}");
            
            Console.WriteLine("\n🎉 Database connection test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Connection failed: {ex.Message}");
            Console.WriteLine("\nTroubleshooting tips:");
            Console.WriteLine("1. Make sure SQL Server container is running: docker ps");
            Console.WriteLine("2. Check if port 1433 is available");
            Console.WriteLine("3. Verify the connection string");
        }
    }
}
