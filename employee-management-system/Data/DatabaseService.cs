using Microsoft.Data.Sqlite;

namespace employee_management_system.Data;

public static class DatabaseService
{
    public static User? FindByIdentifier(string identifier, string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var cmd = new SqliteCommand(
            "SELECT Id, FirstName, LastName, Identifier FROM Users WHERE Identifier = @id",
            connection);

        cmd.Parameters.AddWithValue("@id", identifier);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Identifier = reader.GetString(3)
            };
        }

        return null;
    }
}