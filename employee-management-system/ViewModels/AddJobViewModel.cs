using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace employee_management_system.ViewModels;

public partial class AddJobViewModel : ViewModelBase
{
    [ObservableProperty] private string _jobName = string.Empty;
    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private string _windowTitle = "Nowe Zlecenie";
    [ObservableProperty] private string _headerText = "Krok 1: Dodaj nazwę i opis";
    [ObservableProperty] private string _confirmButtonText = "Dalej";

    public Action? CloseAction { get; set; }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(JobName)) return;
        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        JobName = string.Empty; // Reset to indicate cancellation
        CloseAction?.Invoke();
    }
}
