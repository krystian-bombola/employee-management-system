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

    public EditUserViewModel(UserItemViewModel user, ObservableCollection<PositionItemViewModel> positions)
    {
        _userId = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Identifier = user.Identifier;

        foreach (var p in positions)
            Positions.Add(p);

        SelectedPosition = Positions.FirstOrDefault(p => p.PositionName == user.PositionName);
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            return;

        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));
        userService.Update(_userId, FirstName, LastName, Identifier, NewPassword, SelectedPosition?.Id);

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }
}