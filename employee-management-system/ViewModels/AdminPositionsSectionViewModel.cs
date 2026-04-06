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

public partial class AdminPositionsSectionViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    public ObservableCollection<PositionItemViewModel> Positions { get; } = new();
    public ObservableCollection<PositionItemViewModel> FilteredPositions { get; } = new();

    public void Refresh()
    {
        using var db = new DatabaseContext();
        var positionService = new PositionService(new PositionRepository(db));
        Positions.Clear();
        foreach (var pos in positionService.GetAll())
            Positions.Add(new PositionItemViewModel(pos));

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();
        FilteredPositions.Clear();
        foreach (var p in Positions.Where(p => string.IsNullOrEmpty(query) ||
            p.PositionName.ToLower().Contains(query)))
        {
            FilteredPositions.Add(p);
        }
    }

    [RelayCommand]
    private async Task AddPosition()
    {
        var vm = new EditPositionViewModel(isAddMode: true);
        var window = new Views.EditPositionWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        Refresh();
    }

    [RelayCommand]
    private async Task RemovePosition(PositionItemViewModel? item)
    {
        var confirmed = false;

        if (item is not null)
        {
            confirmed = await DialogService.ShowDeleteConfirmationAsync(
                $"Czy na pewno chcesz usunąć stanowisko {item.PositionName}?");
        }
        else
        {
            var selected = Positions.Where(p => p.IsSelected).ToList();
            if (selected.Count == 0)
                return;

            confirmed = await DialogService.ShowDeleteConfirmationAsync(
                $"Czy na pewno chcesz usunąć zaznaczone stanowiska ({selected.Count})?");
        }

        if (!confirmed) return;

        using var db = new DatabaseContext();
        var positionService = new PositionService(new PositionRepository(db));

        if (item is not null)
        {
            if (!positionService.TryRemove(item.Id, out var errorMessage))
            {
                await DialogService.ShowMessageAsync(errorMessage, "Nie można usunąć stanowiska");
                return;
            }
        }
        else
        {
            foreach (var s in Positions.Where(p => p.IsSelected).ToList())
            {
                if (!positionService.TryRemove(s.Id, out var errorMessage))
                {
                    await DialogService.ShowMessageAsync(errorMessage, "Nie można usunąć stanowiska");
                    return;
                }
            }
        }

        Refresh();
    }

    [RelayCommand]
    private async Task EditPosition(PositionItemViewModel? item)
    {
        if (item is null) return;

        var vm = new EditPositionViewModel(item);
        var window = new Views.EditPositionWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        Refresh();
    }

}
