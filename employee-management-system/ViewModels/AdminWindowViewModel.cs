using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace employee_management_system.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty]
    private string? _newOperationName;

    [ObservableProperty]
    private string? _newUserFirstName;

    [ObservableProperty]
    private string? _newUserLastName;

    [ObservableProperty]
    private string? _newUserID;

    [ObservableProperty]
    private string _newJobName;

    public ObservableCollection<Zlecenie> Jobs { get; } = new();

    public ObservableCollection<Operacja> Operations { get; } = new();

    public ObservableCollection<ProductionRecord> ProductionRecords { get; } = new();

    public AdminWindowViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        Jobs.Add(new Zlecenie { Id = 1, NazwaZlecenia = "Testowe zlecenie", DataUtworzenia = DateTime.Now, Status = "Nowe" });
        using (var db = new DatabaseContext())
        {
            db.Database.EnsureCreated();

            var pracownicy = db.Uzytkownicy.ToList();
            foreach (var p in pracownicy)
            {
                ProductionRecords.Add(new ProductionRecord(
                    $"{p.Imie} {p.Nazwisko}", "Brak operacji", DateTime.Now, DateTime.Now));
            }
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    [RelayCommand]
    private void AddOperation()
    {
        if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();

        var operacja = new Operacja
        {
            NazwaOperacji = NewOperationName,
        };

        db.Operacja.Add(operacja);
        db.SaveChanges();

        Operations.Add(operacja);

        NewOperationName = string.Empty;
    }

    [RelayCommand]
    private void RemoveOperation()
    { 
    if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();

        var OperationToRemove = db.Operacja.FirstOrDefault(z => z.NazwaOperacji == NewOperationName);
        db.Operacja.Remove(OperationToRemove);

        if (OperationToRemove != null)
        {
            db.Operacja.Remove(OperationToRemove);
            db.SaveChanges();

            Operations.Remove(OperationToRemove);

            NewOperationName = string.Empty;
        }
    }

    [RelayCommand]
    private void AddJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();

        var zlecenie = new Zlecenie
        {
            NazwaZlecenia = NewJobName,
            DataUtworzenia = DateTime.Now,
            Status = "Nowe"
        };

        db.Zlecenia.Add(zlecenie);
        db.SaveChanges();

        Jobs.Add(zlecenie);

        NewJobName = string.Empty;
    }

    [RelayCommand]
    private void RemoveJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;

        using var db = new DatabaseContext();

        var jobToRemove = db.Zlecenia.FirstOrDefault(z => z.NazwaZlecenia == NewJobName);

        if (jobToRemove != null)
        {
            db.Zlecenia.Remove(jobToRemove);
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

        var pracownik = new Uzytkownik
        {
            Imie = NewUserFirstName,
            Nazwisko = NewUserLastName,
            Identyfikator = NewUserID
        };

        db.Uzytkownicy.Add(pracownik);
        db.SaveChanges();

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
        var userToDelete = db.Uzytkownicy.FirstOrDefault(u => u.Imie == NewUserFirstName && u.Nazwisko == NewUserLastName && u.Identyfikator == NewUserID);
        if (userToDelete != null)
        {
            db.Uzytkownicy.Remove(userToDelete);
            db.SaveChanges();

            NewUserFirstName = "";
            NewUserLastName = "";
            NewUserID = "";
        }
    }
    
}

public record ProductionRecord(string Employee, string Operation, DateTime StartTime, DateTime EndTime)
{
    public string Duration => (EndTime - StartTime).ToString(@"hh\:mm\:ss");
}