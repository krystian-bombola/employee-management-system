using System;
using Microsoft.Data.Sqlite;
using employee_management_system.Services;

namespace employee_management_system.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var createUsers = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT,
                LastName TEXT,
                Identifier TEXT NOT NULL,
                PasswordHash TEXT NOT NULL DEFAULT '',
                PasswordSalt TEXT NOT NULL DEFAULT '',
                IsAdmin INTEGER NOT NULL DEFAULT 0,
                EmploymentDate TEXT NOT NULL DEFAULT ''
            );";

        var createJobs = @"
            CREATE TABLE IF NOT EXISTS Jobs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                JobName TEXT,
                Description TEXT NOT NULL DEFAULT '',
                CreatedAt TEXT,
                Status TEXT
            );";

        var createOperations = @"
            CREATE TABLE IF NOT EXISTS Operations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OperationName TEXT NOT NULL,
                Description TEXT NOT NULL DEFAULT ''
            );";

        var createJobTasks = @"
            CREATE TABLE IF NOT EXISTS JobTasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                JobId INTEGER NOT NULL,
                OperationId INTEGER NOT NULL,
                [Order] INTEGER NOT NULL DEFAULT 0,
                OperationStart TEXT,
                OperationEnd TEXT,
                ExecutionTime TEXT,
                FOREIGN KEY (JobId) REFERENCES Jobs(Id),
                FOREIGN KEY (OperationId) REFERENCES Operations(Id)
            );";

        var createWorkLogs = @"
            CREATE TABLE IF NOT EXISTS WorkLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                JobTaskId INTEGER NOT NULL,
                WorkStart TEXT,
                WorkEnd TEXT,
                FOREIGN KEY (UserId) REFERENCES Users(Id),
                FOREIGN KEY (JobTaskId) REFERENCES JobTasks(Id)
            );";

        var createPositions = @"
            CREATE TABLE IF NOT EXISTS Positions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PositionName TEXT NOT NULL,
                HourlyRate REAL NOT NULL DEFAULT 0
            );";

        ExecuteNonQuery(connection, createUsers);
        ExecuteNonQuery(connection, createJobs);
        ExecuteNonQuery(connection, createOperations);
        ExecuteNonQuery(connection, createJobTasks);
        ExecuteNonQuery(connection, createWorkLogs);
        ExecuteNonQuery(connection, createPositions);


        var today = DateTime.Now.ToString("yyyy-MM-dd");
        EnsureColumnExists(connection, "Users", "EmploymentDate", $"TEXT NOT NULL DEFAULT '{today}'");
        EnsureColumnExists(connection, "Jobs", "Description", "TEXT NOT NULL DEFAULT ''");
        EnsureColumnExists(connection, "Operations", "Description", "TEXT NOT NULL DEFAULT ''");
        EnsureColumnExists(connection, "Operations", "CurrentWorkersCount", "INTEGER NOT NULL DEFAULT 0");

        ExecuteNonQuery(connection, $"UPDATE Users SET EmploymentDate = '{today}' WHERE EmploymentDate = '' OR EmploymentDate IS NULL;");

        var adminExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Identifier = 'admin'", connection).ExecuteScalar()!;
        if (adminExists == 0)
        {
            var salt = PasswordService.GenerateSalt();
            var hash = PasswordService.HashPassword("admin123", salt);
            ExecuteNonQuery(connection, $"INSERT INTO Users (FirstName, LastName, Identifier, PasswordHash, PasswordSalt, IsAdmin, EmploymentDate) VALUES ('Jan', 'Kowalski', 'admin', '{hash}', '{salt}', 1, '{DateTime.Now:yyyy-MM-dd}')");
        }

        var userExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Identifier = 'user'", connection).ExecuteScalar()!;
        if (userExists == 0)
        {
            var salt = PasswordService.GenerateSalt();
            var hash = PasswordService.HashPassword("user123", salt);
            ExecuteNonQuery(connection, $"INSERT INTO Users (FirstName, LastName, Identifier, PasswordHash, PasswordSalt, IsAdmin, EmploymentDate) VALUES ('Anna', 'Nowak', 'user', '{hash}', '{salt}', 0, '{DateTime.Now:yyyy-MM-dd}')");
        }

        var orderExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Jobs WHERE JobName = 'aaaa'", connection).ExecuteScalar()!;
        if (orderExists == 0)
            ExecuteNonQuery(connection, $"INSERT INTO Jobs (JobName, Description, CreatedAt, Status) VALUES ('aaaa', 'Opis zlecenia aaaa', '{DateTime.Now:yyyy-MM-dd}', 'New')");
    }

    private static void ExecuteNonQuery(SqliteConnection conn, string sql)
    {
        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    private static void EnsureColumnExists(SqliteConnection conn, string table, string columnName, string columnDef)
    {

        var pragma = new SqliteCommand($"PRAGMA table_info({table});", conn);
        using var reader = pragma.ExecuteReader();
        var exists = false;
        while (reader.Read())
        {
            var name = reader.GetString(1);
            if (name == columnName) { exists = true; break; }
        }

        if (!exists)
        {
            ExecuteNonQuery(conn, $"ALTER TABLE {table} ADD COLUMN {columnName} {columnDef};");
        }
    }
}