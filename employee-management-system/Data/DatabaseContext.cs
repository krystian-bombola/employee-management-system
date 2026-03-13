using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace employee_management_system.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Zlecenie> Zlecenia { get; set; }
    public DbSet<Uzytkownik> Uzytkownicy { get; set; }
    public DbSet<Operacja> Operacja { get; set; }
    public DbSet<OperacjaPrzypisana> OperacjePrzypisane { get; set; }
    public DbSet<ZarzadzanieProdukcja> ZarzadzanieProdukcja { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "produkcja.db");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Uzytkownik>().ToTable("Uzytkownik");
        modelBuilder.Entity<Zlecenie>().ToTable("Zlecenie");

        modelBuilder.Entity<OperacjaPrzypisana>().HasOne(o => o.Zlecenie).WithMany(z => z.OperacjePrzypisane).HasForeignKey(o => o.IdZlecenia);
        modelBuilder.Entity<OperacjaPrzypisana>().HasOne(o => o.Operacja).WithMany(o => o.OperacjePrzypisane).HasForeignKey(o => o.IdOperacji);
        modelBuilder.Entity<ZarzadzanieProdukcja>().HasOne(z => z.OperacjaPrzypisana).WithMany(o => o.ZarzadzanieProdukcja).HasForeignKey(z => z.IdOperacjiPrzypisanej);
        modelBuilder.Entity<ZarzadzanieProdukcja>().HasOne(z => z.Uzytkownicy).WithMany(p => p.ZarzadzanieProdukcja).HasForeignKey(z => z.IdPracownika);
    }
}