using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class AdminUsersSectionViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    [ObservableProperty] private UserItemViewModel? _selectedUser;



    public ObservableCollection<UserItemViewModel> Users { get; } = new();
    public ObservableCollection<UserItemViewModel> FilteredUsers { get; } = new();
    public ObservableCollection<PositionItemViewModel> Positions { get; } = new();

    public void Refresh(string? targetIdentifier = null)
    {
        using var db = new DatabaseContext();
        db.Database.EnsureCreated();

        var positionService = new PositionService(new PositionRepository(db));
        Positions.Clear();
        foreach (var pos in positionService.GetAll())
            Positions.Add(new PositionItemViewModel(pos));

        var userService = new UserService(new UserRepository(db));
        Users.Clear();
        foreach (var u in userService.GetAll())
        {
            Users.Add(new UserItemViewModel(u));
        }

        ApplyFilter();

        if (!string.IsNullOrEmpty(targetIdentifier))
        {
            SelectedUser = FilteredUsers.FirstOrDefault(u => u.Identifier == targetIdentifier);
        }
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();
        FilteredUsers.Clear();
        foreach (var u in Users.Where(u => string.IsNullOrEmpty(query) ||
            u.FirstName.ToLower().Contains(query) ||
            u.LastName.ToLower().Contains(query) ||
            u.Identifier.ToLower().Contains(query)))
        {
            FilteredUsers.Add(u);
        }
    }

    [RelayCommand]
    private async Task AddUser()
    {
        var vm = new EditUserViewModel(Positions);
        var window = new Views.EditUserWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        Refresh();
    }

    [RelayCommand]
    private async Task RemoveUser(UserItemViewModel? item)
    {
        var target = item ?? SelectedUser;
        if (target is null) return;
        var confirmed = await DialogService.ShowDeleteConfirmationAsync(
            $"Czy na pewno chcesz usunąć użytkownika {target.FirstName} {target.LastName}?");
        if (!confirmed) return;

        // Zapamiętaj ID użytkownik poniżej przed usunięciem
        string? nextTargetId = null;
        var index = FilteredUsers.IndexOf(target);
        if (index >= 0)
        {
            if (index + 1 < FilteredUsers.Count)
                nextTargetId = FilteredUsers[index + 1].Identifier;
            else if (index - 1 >= 0)
                nextTargetId = FilteredUsers[index - 1].Identifier;
        }

        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));
        if (!userService.TryRemove(target.FirstName, target.LastName, target.Identifier, out var errorMessage))
        {
            await DialogService.ShowMessageAsync(errorMessage, "Nie można usunąć użytkownika");
            return;
        }
        
        Refresh(nextTargetId);
    }

    [RelayCommand]
    private async Task EditUser(UserItemViewModel? item)
    {
        var target = item ?? SelectedUser;
        if (target is null) return;

        var vm = new EditUserViewModel(target, Positions);
        var window = new Views.EditUserWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        Refresh(target.Identifier);
    }
}
