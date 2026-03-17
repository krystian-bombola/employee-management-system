
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainVm;

    [ObservableProperty]
    private string _identifier = string.Empty;

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
            return;

        var identifier = Identifier.Trim();
        if (string.IsNullOrWhiteSpace(identifier))
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

        using var db = new DatabaseContext();
        var authService = new AuthService(new UserRepository(db));
        var user = authService.Login(identifier);

        if (user is null)
        {
            ErrorMessage = "Nie znaleziono użytkownika.";
            IsErrorVisible = true;
            return;
        }

        ErrorMessage = string.Empty;
        IsErrorVisible = false;

        if (user.IsAdmin)
        {
            _mainVm.CurrentView = new AdminWindowViewModel(_mainVm, user);
        }
        else
        {
            var employeeName = $"{user.FirstName} {user.LastName}".Trim();
            _mainVm.CurrentView = new UserPanelViewModel(_mainVm, employeeName, orderId);
        }
    }
}