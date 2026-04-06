using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;
using Microsoft.EntityFrameworkCore;

namespace employee_management_system.ViewModels;

public partial class AdminProductionTimeSectionViewModel : ViewModelBase
{
    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    public ObservableCollection<JobItemViewModel> Jobs { get; } = new();
    public ObservableCollection<JobItemViewModel> FilteredJobs { get; } = new();

    public void Refresh()
    {
        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));
        Jobs.Clear();
        foreach (var job in jobService.GetAll())
        {
            Jobs.Add(new JobItemViewModel(job));
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();
        FilteredJobs.Clear();
        foreach (var j in Jobs.Where(j => string.IsNullOrEmpty(query) ||
            j.JobName.ToLower().Contains(query) ||
            j.Status.ToLower().Contains(query)))
        {
            FilteredJobs.Add(j);
        }
    }

    [RelayCommand]
    private async Task ShowJobDetails(JobItemViewModel? item)
    {
        if (item is null) return;
        using var db = new DatabaseContext();
        var job = db.Jobs.FirstOrDefault(j => j.Id == item.Id);
        if (job is null) return;

        var jobOperations = new List<JobOperationItemViewModel>();
        var tasks = db.JobTasks.Where(jt => jt.JobId == item.Id).Include(jt => jt.Operation).OrderBy(jt => jt.Order).ToList();
        foreach (var jt in tasks)
            jobOperations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));

        var vm = new JobDetailsViewModel($"Szczegóły zlecenia: {job.JobName}", jobOperations);
        var window = new Views.JobDetailsWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }
    }
}
