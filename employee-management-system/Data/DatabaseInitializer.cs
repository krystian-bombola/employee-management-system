
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
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT,
                LastName TEXT,
                Identifier TEXT NOT NULL,
                PasswordHash TEXT NOT NULL DEFAULT '',
                PasswordSalt TEXT NOT NULL DEFAULT '',
                IsAdmin INTEGER NOT NULL DEFAULT 0
            );";

        var createJobs = @"
            CREATE TABLE IF NOT EXISTS Jobs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                JobName TEXT,
                CreatedAt TEXT,
                Status TEXT
            );";

        var createOperations = @"
            CREATE TABLE IF NOT EXISTS Operations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OperationName TEXT NOT NULL
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

        ExecuteNonQuery(connection, createUsers);
        ExecuteNonQuery(connection, createJobs);
        ExecuteNonQuery(connection, createOperations);
        ExecuteNonQuery(connection, createJobTasks);
        ExecuteNonQuery(connection, createWorkLogs);

        var adminExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Identifier = 'admin'", connection).ExecuteScalar()!;
        if (adminExists == 0)
            ExecuteNonQuery(connection, "INSERT INTO Users (FirstName, LastName, Identifier, IsAdmin) VALUES ('Jan', 'Kowalski', 'admin', 1)");

        var userExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Identifier = 'user'", connection).ExecuteScalar()!;
        if (userExists == 0)
            ExecuteNonQuery(connection, "INSERT INTO Users (FirstName, LastName, Identifier, IsAdmin) VALUES ('Anna', 'Nowak', 'user', 0)");

        var orderExists = (long)new SqliteCommand("SELECT COUNT(*) FROM Jobs WHERE JobName = 'aaaa'", connection).ExecuteScalar()!;
        if (orderExists == 0)
            ExecuteNonQuery(connection, $"INSERT INTO Jobs (JobName, CreatedAt, Status) VALUES ('aaaa', '{DateTime.Now:yyyy-MM-dd}', 'New')");
    }

    private static void ExecuteNonQuery(SqliteConnection conn, string sql)
    {
        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}