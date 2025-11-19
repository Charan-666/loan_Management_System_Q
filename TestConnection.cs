using Microsoft.Data.SqlClient;

public class ConnectionTest
{
    public static async Task TestConnection()
    {
        var connectionString = "Server=172.10.30.165,1433;Database=LMPSharedDB;User Id=SharedUser;Password=LMPSharedSQL@123;TrustServerCertificate=True;Connection Timeout=30;";
        
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine("✅ Connection successful!");
            
            using var command = new SqlCommand("SELECT @@VERSION", connection);
            var version = await command.ExecuteScalarAsync();
            Console.WriteLine($"SQL Server Version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Connection failed: {ex.Message}");
        }
    }
}