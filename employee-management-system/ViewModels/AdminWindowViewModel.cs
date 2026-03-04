using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _newOperationName;

    public ObservableCollection<string> AvailableOperations { get; } = new()
    {
        "Montaż",
        "Spawanie",
        "Kontrola jakości",
        "Pakowanie",
        "Transport"
    };

    public ObservableCollection<ProductionRecord> ProductionRecords { get; } = new();

    public AdminWindowViewModel()
    {
        // przykładowe dane do podglądu
        ProductionRecords.Add(new ProductionRecord("Jan Kowalski", "Montaż", DateTime.Now.AddHours(-2), DateTime.Now));
    }

    [RelayCommand]
    private void AddOperation()
    {
        if (!string.IsNullOrWhiteSpace(NewOperationName))
        {
            AvailableOperations.Add(NewOperationName);
            NewOperationName = string.Empty;
        }
    }

    [RelayCommand]
    private void RemoveOperation()
    {
        if (!string.IsNullOrWhiteSpace(NewOperationName) && AvailableOperations.Contains(NewOperationName))
        {
            AvailableOperations.Remove(NewOperationName);
            NewOperationName = string.Empty;
        }
    }
}

public record ProductionRecord(string Employee, string Operation, DateTime StartTime, DateTime EndTime)
{
    public string Duration => (EndTime - StartTime).ToString(@"hh\:mm\:ss");
}