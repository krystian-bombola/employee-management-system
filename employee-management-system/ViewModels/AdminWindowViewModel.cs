
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Models;
using employee_management_system.Repositories;
using employee_management_system.Services;
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

        using var db = new DatabaseContext();
        db.Database.EnsureCreated();

        var userService = new UserService(new UserRepository(db));
        foreach (var u in userService.GetAll())
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
        var operationService = new OperationService(new OperationRepository(db));
        operationService.Add(NewOperationName);

        Operations.Add(new Operation { OperationName = NewOperationName });
        NewOperationName = string.Empty;
    }

    [RelayCommand]
    private void RemoveOperation()
    {
        if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));
        operationService.Remove(NewOperationName);

        var local = Operations.FirstOrDefault(o => o.OperationName == NewOperationName);
        if (local != null)
            Operations.Remove(local);

        NewOperationName = string.Empty;
    }

    [RelayCommand]
    private void AddJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        jobService.Add(NewJobName);

        Jobs.Add(new Job { JobName = NewJobName, CreatedAt = DateTime.Now, Status = "New" });
        NewJobName = string.Empty;
    }

    [RelayCommand]
    private void RemoveJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        jobService.Remove(NewJobName);

        var local = Jobs.FirstOrDefault(j => j.JobName == NewJobName);
        if (local != null)
            Jobs.Remove(local);

        NewJobName = string.Empty;
    }

    [RelayCommand]
    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserFirstName) ||
            string.IsNullOrWhiteSpace(NewUserLastName))
            return;

        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));
        userService.Add(NewUserFirstName, NewUserLastName, NewUserID ?? string.Empty);

        ProductionRecords.Add(new ProductionRecord(
            NewUserID ?? string.Empty,
            $"{NewUserFirstName} {NewUserLastName}",
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
        var userService = new UserService(new UserRepository(db));
        userService.Remove(NewUserFirstName ?? string.Empty, NewUserLastName ?? string.Empty, NewUserID ?? string.Empty);

        var record = ProductionRecords.FirstOrDefault(r => r.EmployeeId == NewUserID);
        if (record != null)
            ProductionRecords.Remove(record);

        NewUserFirstName = "";
        NewUserLastName = "";
        NewUserID = "";
    }
}

public record ProductionRecord(string EmployeeId, string Employee, string Operation, DateTime StartTime, DateTime EndTime)
{
    public string Duration => (EndTime - StartTime).ToString(@"hh\:mm\:ss");
}