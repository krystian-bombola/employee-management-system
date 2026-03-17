using Avalonia;
using System;
using System.IO;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.ViewModels;

namespace employee_management_system;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "production.db");
        DatabaseInitializer.Initialize(dbPath);

        using (var db = new DatabaseContext())
        {
            db.Database.EnsureCreated();
        }

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}