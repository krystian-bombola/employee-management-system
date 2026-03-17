
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace employee_management_system.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainVm;

    [ObservableProperty]
    private string _loggedUserSurname = string.Empty;

    [ObservableProperty]
    private string? _newOperationName;

    [ObservableProperty]
    private string? _newUserFirstName;

    [ObservableProperty]
    private string? _newUserLastName;

    [ObservableProperty]
    private string? _newUserID;

    [ObservableProperty]
    private string _newJobName = string.Empty;

    public ObservableCollection<Job> Jobs { get; } = new();
    public ObservableCollection<Operation> Operations { get; } = new();
    public ObservableCollection<ProductionRecord> ProductionRecords { get; } = new();

    public AdminWindowViewModel()
    {
    }

    public AdminWindowViewModel(MainWindowViewModel mainVm, User user)
    {
        _mainVm = mainVm;
        LoggedUserSurname = user.LastName;

        Jobs.Add(new Job { Id = 1, JobName = "Test job", CreatedAt = DateTime.Now, Status = "New" });

        using var db = new DatabaseContext();
        db.Database.EnsureCreated();

        foreach (var u in db.Users.ToList())
        {
            ProductionRecords.Add(new ProductionRecord(
                u.Identifier,
                $"{u.FirstName} {u.LastName}",
                "No operation",
                DateTime.Now,
                DateTime.Now));
        }
    }

    [RelayCommand]
    private void Logout()
    {
        if (_mainVm is null)
            return;

        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    [RelayCommand]
    private void AddOperation()
    {
        if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();
        var operation = new Operation { OperationName = NewOperationName };
        db.Operations.Add(operation);
        db.SaveChanges();

        Operations.Add(operation);
        NewOperationName = string.Empty;
    }

    [RelayCommand]
    private void RemoveOperation()
    {
        if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();
        var operationToRemove = db.Operations.FirstOrDefault(o => o.OperationName == NewOperationName);

        if (operationToRemove != null)
        {
            db.Operations.Remove(operationToRemove);
            db.SaveChanges();

            Operations.Remove(operationToRemove);
            NewOperationName = string.Empty;
        }
    }

    [RelayCommand]
    private void AddJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();
        var job = new Job
        {
            JobName = NewJobName,
            CreatedAt = DateTime.Now,
            Status = "New"
        };
        db.Jobs.Add(job);
        db.SaveChanges();

        Jobs.Add(job);
        NewJobName = string.Empty;
    }

    [RelayCommand]
    private void RemoveJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();
        var jobToRemove = db.Jobs.FirstOrDefault(j => j.JobName == NewJobName);

        if (jobToRemove != null)
        {
            db.Jobs.Remove(jobToRemove);
            db.SaveChanges();

            Jobs.Remove(jobToRemove);
            NewJobName = string.Empty;
        }
    }

    [RelayCommand]
    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserFirstName) ||
            string.IsNullOrWhiteSpace(NewUserLastName))
            return;

        using var db = new DatabaseContext();
        var user = new User
        {
            FirstName = NewUserFirstName,
            LastName = NewUserLastName,
            Identifier = NewUserID ?? string.Empty
        };
        db.Users.Add(user);
        db.SaveChanges();

        ProductionRecords.Add(new ProductionRecord(
            user.Identifier,
            $"{user.FirstName} {user.LastName}",
            "No operation",
            DateTime.Now,
            DateTime.Now));

        NewUserFirstName = "";
        NewUserLastName = "";
        NewUserID = "";
    }

    [RelayCommand]
    private void RemoveUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserFirstName) &&
            string.IsNullOrWhiteSpace(NewUserLastName) &&
            string.IsNullOrWhiteSpace(NewUserID))
            return;

        using var db = new DatabaseContext();
        var userToDelete = db.Users.FirstOrDefault(u =>
            u.FirstName == NewUserFirstName &&
            u.LastName == NewUserLastName &&
            u.Identifier == NewUserID);

        if (userToDelete != null)
        {
            db.Users.Remove(userToDelete);
            db.SaveChanges();

            var record = ProductionRecords.FirstOrDefault(r =>
                r.EmployeeId == userToDelete.Identifier);
            if (record != null)
                ProductionRecords.Remove(record);

            NewUserFirstName = "";
            NewUserLastName = "";
            NewUserID = "";
        }
    }
}

public record ProductionRecord(string EmployeeId, string Employee, string Operation, DateTime StartTime, DateTime EndTime)
{
    public string Duration => (EndTime - StartTime).ToString(@"hh\:mm\:ss");
}