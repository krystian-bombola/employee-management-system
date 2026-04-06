using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class AdminOperationsSectionViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    public ObservableCollection<OperationItemViewModel> Operations { get; } = new();
    public ObservableCollection<OperationItemViewModel> FilteredOperations { get; } = new();

    public void Refresh()
    {
        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));
        Operations.Clear();
        foreach (var op in operationService.GetAll())
        {
            Operations.Add(new OperationItemViewModel(op));
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();
        FilteredOperations.Clear();
        foreach (var op in Operations.Where(o => string.IsNullOrEmpty(query) ||
            o.OperationName.ToLower().Contains(query) ||
            o.Description.ToLower().Contains(query)))
        {
            FilteredOperations.Add(op);
        }
    }

    [RelayCommand]
    private async Task AddOperation()
    {
        var vm = new AddOperationViewModel();
        var window = new Views.AddOperationWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        Refresh();
    }

    [RelayCommand]
    private void RemoveOperation(OperationItemViewModel? item)
    {
        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));

        if (item is not null)
        {
            operationService.Remove(item.OperationName);
        }
        else
        {
            var selected = Operations.Where(o => o.IsSelected).ToList();
            foreach (var s in selected)
            {
                operationService.Remove(s.OperationName);
            }
        }
        Refresh();
    }
}
