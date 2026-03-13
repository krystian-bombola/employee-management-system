using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using System;
using System.IO;

namespace employee_management_system.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainVm;

    [ObservableProperty]
    private string _identyfikator = string.Empty;

    [ObservableProperty]
    private string _orderId = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorVisible;

    public LoginViewModel()
    {
    }

    public LoginViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;
    }

    [RelayCommand]
    private void Login()
    {
        if (_mainVm is null)
        {
            return;
        }

        var identyfikator = Identyfikator.Trim();
        if (string.IsNullOrWhiteSpace(identyfikator))
        {
            ErrorMessage = "Wpisz identyfikator.";
            IsErrorVisible = true;
            return;
        }

        var orderId = OrderId.Trim();
        if (string.IsNullOrWhiteSpace(orderId))
        {
            ErrorMessage = "Wpisz ID zlecenia.";
            IsErrorVisible = true;
            return;
        }

        var dbPath = Path.Combine(AppContext.BaseDirectory, "produkcja.db");
        var uzytkownik = DatabaseService.FindByIdentyfikator(identyfikator, dbPath);
        if (uzytkownik is null)
        {
            ErrorMessage = "Nie znaleziono użytkownika.";
            IsErrorVisible = true;
            return;
        }

        ErrorMessage = string.Empty;
        IsErrorVisible = false;

        switch (uzytkownik.Identyfikator)
        {
            case "admin":
                _mainVm.CurrentView = new AdminWindowViewModel(_mainVm, uzytkownik);
                break;
            case "user":
                var employeeName = $"{uzytkownik.Imie} {uzytkownik.Nazwisko}".Trim();
                _mainVm.CurrentView = new UserPanelViewModel(_mainVm, employeeName, orderId);
                break;
            default:
                ErrorMessage = "Brak uprawnień dla tego konta.";
                IsErrorVisible = true;
                break;
        }
    }
}