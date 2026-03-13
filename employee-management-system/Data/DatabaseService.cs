using Microsoft.Data.Sqlite;

namespace employee_management_system.Data;

public static class DatabaseService
{
    public static Uzytkownik? FindByIdentyfikator(string identyfikator, string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var cmd = new SqliteCommand(
            "SELECT Id, Imie, Nazwisko, Identyfikator FROM Uzytkownik WHERE Identyfikator = @id",
            connection);

        cmd.Parameters.AddWithValue("@id", identyfikator);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Uzytkownik
            {
                Id = reader.GetInt32(0),
                Imie = reader.GetString(1),
                Nazwisko = reader.GetString(2),
                Identyfikator = reader.GetString(3)
            };
        }

        return null;
    }
}
