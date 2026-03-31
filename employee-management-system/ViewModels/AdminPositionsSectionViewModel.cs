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

    [ObservableProperty] private string _newPositionName = string.Empty;
    [ObservableProperty] private string _newPositionHourlyRate = string.Empty;

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
    private void AddPosition()
    {
        if (string.IsNullOrWhiteSpace(NewPositionName)) return;
        if (!decimal.TryParse(NewPositionHourlyRate.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var rate)) return;

        using var db = new DatabaseContext();
        var positionService = new PositionService(new PositionRepository(db));
        positionService.Add(NewPositionName, rate);

        NewPositionName = string.Empty;
        NewPositionHourlyRate = string.Empty;
        Refresh();
    }

    [RelayCommand]
    private void RemovePosition(PositionItemViewModel? item)
    {
        using var db = new DatabaseContext();
        var positionService = new PositionService(new PositionRepository(db));

        if (item is not null)
        {
            positionService.Remove(item.Id);
        }
        else
        {
            foreach (var s in Positions.Where(p => p.IsSelected).ToList())
                positionService.Remove(s.Id);
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
