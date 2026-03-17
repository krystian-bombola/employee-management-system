using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using employee_management_system.Models;

namespace employee_management_system.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Job> Jobs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Operation> Operations { get; set; }
    public DbSet<JobTask> JobTasks { get; set; }
    public DbSet<WorkLog> WorkLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "produkcja.db");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Job>().ToTable("Jobs");
        modelBuilder.Entity<Operation>().ToTable("Operations");
        modelBuilder.Entity<JobTask>().ToTable("JobTasks");
        modelBuilder.Entity<WorkLog>().ToTable("WorkLogs");

        modelBuilder.Entity<JobTask>()
            .HasOne(jt => jt.Job)
            .WithMany(j => j.JobTasks)
            .HasForeignKey(jt => jt.JobId);

        modelBuilder.Entity<JobTask>()
            .HasOne(jt => jt.Operation)
            .WithMany(o => o.JobTasks)
            .HasForeignKey(jt => jt.OperationId);

        modelBuilder.Entity<WorkLog>()
            .HasOne(wl => wl.JobTask)
            .WithMany(jt => jt.WorkLogs)
            .HasForeignKey(wl => wl.JobTaskId);

        modelBuilder.Entity<WorkLog>()
            .HasOne(wl => wl.User)
            .WithMany(u => u.WorkLogs)
            .HasForeignKey(wl => wl.UserId);
    }
}