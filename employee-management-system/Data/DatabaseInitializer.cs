using System;
using Microsoft.Data.Sqlite;

namespace employee_management_system.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var createUsers = @"
            CREATE TABLE IF NOT EXISTS Uzytkownik (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Imie TEXT,
                Nazwisko TEXT,
                Identyfikator TEXT NOT NULL
            );";

        var createZlecenia = @"
            CREATE TABLE IF NOT EXISTS Zlecenie (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NazwaZlecenia TEXT,
                DataUtworzenia TEXT,
                Status TEXT
            );";

        ExecuteNonQuery(connection, createUsers);
        ExecuteNonQuery(connection, createZlecenia);

        var adminExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Uzytkownik WHERE Identyfikator = 'admin'", connection).ExecuteScalar()!;
        if (adminExists == 0)
        {
            ExecuteNonQuery(connection, "INSERT INTO Uzytkownik (Imie, Nazwisko, Identyfikator) VALUES ('Jan', 'Kowalski', 'admin')");
        }

        var userExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Uzytkownik WHERE Identyfikator = 'user'", connection).ExecuteScalar()!;
        if (userExists == 0)
        {
            ExecuteNonQuery(connection, "INSERT INTO Uzytkownik (Imie, Nazwisko, Identyfikator) VALUES ('Anna', 'Nowak', 'user')");
        }

        var orderExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Zlecenie WHERE NazwaZlecenia = 'aaaa'", connection).ExecuteScalar()!;
        if (orderExists == 0)
        {
            ExecuteNonQuery(connection, $"INSERT INTO Zlecenie (NazwaZlecenia, DataUtworzenia, Status) VALUES ('aaaa', '{DateTime.Now:yyyy-MM-dd}', 'Nowe')");
        }
    }

    private static void ExecuteNonQuery(SqliteConnection conn, string sql)
    {
        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}
