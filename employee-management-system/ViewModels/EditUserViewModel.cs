using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Models;
using employee_management_system.Repositories;
using employee_management_system.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace employee_management_system.ViewModels;

public partial class EditUserViewModel : ObservableObject
{
    private readonly int _userId;
    public System.Action? CloseAction { get; set; }

    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _windowTitle = string.Empty;
    [ObservableProperty] private string _saveButtonText = string.Empty;
    [ObservableProperty] private string _passwordLabel = string.Empty;
    [ObservableProperty] private bool _isAdmin;

    public ObservableCollection<PositionItemViewModel> Positions { get; } = new();

    private string _firstName = string.Empty;
    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

    private string _lastName = string.Empty;
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

    private string _identifier = string.Empty;
    public string Identifier { get => _identifier; set => SetProperty(ref _identifier, value); }

    private string _newPassword = string.Empty;
    public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

    private PositionItemViewModel? _selectedPosition;
    public PositionItemViewModel? SelectedPosition
    {
        get => _selectedPosition;
        set => SetProperty(ref _selectedPosition, value);
    }

    public EditUserViewModel() { }

    // Constructor for Add mode
    public EditUserViewModel(ObservableCollection<PositionItemViewModel> positions)
    {
        IsEditMode = false;
        WindowTitle = "Dodaj użytkownika";
        SaveButtonText = "Dodaj";
        PasswordLabel = "Hasło";

        foreach (var p in positions)
            Positions.Add(p);
    }

    // Constructor for Edit mode
    public EditUserViewModel(UserItemViewModel user, ObservableCollection<PositionItemViewModel> positions)
    {
        IsEditMode = true;
        WindowTitle = "Edytuj użytkownika";
        SaveButtonText = "Zapisz zmiany";
        PasswordLabel = "Nowe hasło (pozostaw puste, aby nie zmieniać)";

        _userId = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Identifier = user.Identifier;
        IsAdmin = user.IsAdmin;

        foreach (var p in positions)
            Positions.Add(p);

        SelectedPosition = Positions.FirstOrDefault(p => p.PositionName == user.PositionName);
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            return;

        if (!IsEditMode && string.IsNullOrWhiteSpace(NewPassword))
            return;

        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));

        if (IsEditMode)
        {
            userService.Update(_userId, FirstName, LastName, Identifier, NewPassword, SelectedPosition?.Id, IsAdmin);
        }
        else
        {
            userService.Add(FirstName, LastName, Identifier, NewPassword, SelectedPosition?.Id, IsAdmin);
        }

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }
}
