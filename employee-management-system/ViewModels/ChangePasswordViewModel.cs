using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly int _userId;
    public System.Action? CloseAction { get; set; }

    private string _currentPassword = string.Empty;
    public string CurrentPassword { get => _currentPassword; set => SetProperty(ref _currentPassword, value); }

    private string _newPassword = string.Empty;
    public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }

    private string _errorMessage = string.Empty;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private bool _isErrorVisible;
    public bool IsErrorVisible { get => _isErrorVisible; set => SetProperty(ref _isErrorVisible, value); }

    public ChangePasswordViewModel() { }

    public ChangePasswordViewModel(int userId)
    {
        _userId = userId;
    }

    [RelayCommand]
    private void Save()
    {
        IsErrorVisible = false;

        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            ErrorMessage = "Podaj aktualne hasło.";
            IsErrorVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Podaj nowe hasło.";
            IsErrorVisible = true;
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Nowe hasła nie są identyczne.";
            IsErrorVisible = true;
            return;
        }

        using var db = new DatabaseContext();
        var repo = new UserRepository(db);
        var user = repo.GetAll().Find(u => u.Id == _userId);

        if (user is null)
        {
            ErrorMessage = "Nie znaleziono użytkownika.";
            IsErrorVisible = true;
            return;
        }

        var currentHash = PasswordService.HashPassword(CurrentPassword, user.PasswordSalt);
        if (currentHash != user.PasswordHash)
        {
            ErrorMessage = "Aktualne hasło jest nieprawidłowe.";
            IsErrorVisible = true;
            return;
        }

        var service = new UserService(repo);
        service.Update(_userId, user.FirstName, user.LastName, user.Identifier, NewPassword, user.PositionId, user.IsAdmin);

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel() => CloseAction?.Invoke();
}
