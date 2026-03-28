using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Models;
using employee_management_system.Repositories;
using employee_management_system.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace employee_management_system.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainVm;

    private string _loggedUserSurname = string.Empty;
    public string LoggedUserSurname { get => _loggedUserSurname; set => SetProperty(ref _loggedUserSurname, value); }

    private string? _newOperationName;
    public string? NewOperationName { get => _newOperationName; set => SetProperty(ref _newOperationName, value); }

    private string? _newOperationDescription;
    public string? NewOperationDescription { get => _newOperationDescription; set => SetProperty(ref _newOperationDescription, value); }

    private string? _newUserFirstName;
    public string? NewUserFirstName { get => _newUserFirstName; set => SetProperty(ref _newUserFirstName, value); }

    private string? _newUserLastName;
    public string? NewUserLastName { get => _newUserLastName; set => SetProperty(ref _newUserLastName, value); }

    private string? _newUserID;
    public string? NewUserID { get => _newUserID; set => SetProperty(ref _newUserID, value); }

    private string _newUserPassword = string.Empty;
    public string NewUserPassword { get => _newUserPassword; set => SetProperty(ref _newUserPassword, value); }

    private PositionItemViewModel? _selectedNewUserPosition;
    public PositionItemViewModel? SelectedNewUserPosition
    {
        get => _selectedNewUserPosition;
        set => SetProperty(ref _selectedNewUserPosition, value);
    }

    private string _newJobName = string.Empty;
    public string NewJobName { get => _newJobName; set { if (SetProperty(ref _newJobName, value)) { AddJobCommand.NotifyCanExecuteChanged();  } } }

    private string _newJobDescription = string.Empty;
    public string NewJobDescription { get => _newJobDescription; set { if (SetProperty(ref _newJobDescription, value)) { AddJobCommand.NotifyCanExecuteChanged(); } } }

    private string _newJobStatus = "Nowe";
    public string NewJobStatus { get => _newJobStatus; set => SetProperty(ref _newJobStatus, value); }

    private string _newPositionName = string.Empty;
    public string NewPositionName { get => _newPositionName; set => SetProperty(ref _newPositionName, value); }

    private string _newPositionHourlyRate = string.Empty;
    public string NewPositionHourlyRate { get => _newPositionHourlyRate; set => SetProperty(ref _newPositionHourlyRate, value); }

    private string _searchQuery = string.Empty;
    public string SearchQuery { get => _searchQuery; set { if (SetProperty(ref _searchQuery, value)) ApplyFilter(); } }

    private int _userCount;
    public int UserCount { get => _userCount; set => SetProperty(ref _userCount, value); }

    private int _operationCount;
    public int OperationCount { get => _operationCount; set => SetProperty(ref _operationCount, value); }

    private int _jobCount;
    public int JobCount { get => _jobCount; set => SetProperty(ref _jobCount, value); }

    private ViewModelBase? _currentView;
    public ViewModelBase? CurrentView { get => _currentView; set => SetProperty(ref _currentView, value); }

    private int _currentTabIndex = 0;
    public int CurrentTabIndex { get => _currentTabIndex; set { if (SetProperty(ref _currentTabIndex, value)) { OnCurrentTabIndexChanged(); } } }

    public bool IsUsersVisible => CurrentTabIndex == 0;
    public bool IsOperationsVisible => CurrentTabIndex == 1;
    public bool IsJobsVisible => CurrentTabIndex == 2;
    public bool IsPositionsVisible => CurrentTabIndex == 3;
    public bool IsProductionTimeVisible => CurrentTabIndex == 4;

    private void OnCurrentTabIndexChanged()
    {
        OnPropertyChanged(nameof(IsUsersVisible));
        OnPropertyChanged(nameof(IsOperationsVisible));
        OnPropertyChanged(nameof(IsJobsVisible));
        OnPropertyChanged(nameof(IsPositionsVisible));
        OnPropertyChanged(nameof(IsProductionTimeVisible));
        ApplyFilter();
    }

    public ObservableCollection<JobItemViewModel> Jobs { get; } = new();
    public ObservableCollection<OperationItemViewModel> Operations { get; } = new();
    public ObservableCollection<UserItemViewModel> Users { get; } = new();
    public ObservableCollection<PositionItemViewModel> Positions { get; } = new();

    private bool _isAssignOperationsVisible;
    public bool IsAssignOperationsVisible { get => _isAssignOperationsVisible; set => SetProperty(ref _isAssignOperationsVisible, value); }

    public ObservableCollection<OperationItemViewModel> AvailableOperationsForAssign { get; } = new();
    public ObservableCollection<JobOperationItemViewModel> JobOperations { get; } = new();
    public ObservableCollection<OperationItemViewModel> AvailableOperationsForJobEdit { get; } = new();
    public ObservableCollection<string> JobTaskStatuses { get; } = new() { "Nowe", "W trakcie", "Zakończone" };

    private JobItemViewModel? _selectedJobForOperations;
    public JobItemViewModel? SelectedJobForOperations { get => _selectedJobForOperations; set => SetProperty(ref _selectedJobForOperations, value); }

    private OperationItemViewModel? _selectedOperationToAdd;
    public OperationItemViewModel? SelectedOperationToAdd { get => _selectedOperationToAdd; set => SetProperty(ref _selectedOperationToAdd, value); }

    private bool _isJobOperationsVisible;
    public bool IsJobOperationsVisible { get => _isJobOperationsVisible; set => SetProperty(ref _isJobOperationsVisible, value); }

    private bool _isJobDetailsVisible;
    public bool IsJobDetailsVisible { get => _isJobDetailsVisible; set => SetProperty(ref _isJobDetailsVisible, value); }

    private string _jobDetailsTitle = "";
    public string JobDetailsTitle { get => _jobDetailsTitle; set => SetProperty(ref _jobDetailsTitle, value); }

    public ObservableCollection<JobOperationItemViewModel> JobDetailsOperations { get; } = new();

    private string _jobOperationsTitle = "";
    public string JobOperationsTitle { get => _jobOperationsTitle; set => SetProperty(ref _jobOperationsTitle, value); }

    public ObservableCollection<string> JobStatuses { get; } = new();

    public ObservableCollection<JobItemViewModel> FilteredJobs { get; } = new();
    public ObservableCollection<OperationItemViewModel> FilteredOperations { get; } = new();
    public ObservableCollection<UserItemViewModel> FilteredUsers { get; } = new();
    public ObservableCollection<PositionItemViewModel> FilteredPositions { get; } = new();

    public AdminWindowViewModel()
    {
    }

    public AdminWindowViewModel(MainWindowViewModel mainVm, User user)
    {
        _mainVm = mainVm;
        LoggedUserSurname = user.LastName;

        JobStatuses.Add("Nowe");
        JobStatuses.Add("W trakcie");
        JobStatuses.Add("Zatrzymane");
        JobStatuses.Add("Zakończone");

        RefreshAll();
    }

    private void RefreshAll()
    {
        using var db = new DatabaseContext();
        db.Database.EnsureCreated();

        var userService = new UserService(new UserRepository(db));
        Users.Clear();
        foreach (var u in userService.GetAll())
        {
            Users.Add(new UserItemViewModel(u));
        }
        UserCount = Users.Count;

        var operationService = new OperationService(new OperationRepository(db));
        Operations.Clear();
        foreach (var op in operationService.GetAll())
        {
            Operations.Add(new OperationItemViewModel(op));
        }
        OperationCount = Operations.Count;

        var jobService = new JobService(new JobRepository(db));
        Jobs.Clear();
        foreach (var job in jobService.GetAll())
        {
            Jobs.Add(new JobItemViewModel(job));
        }
        JobCount = Jobs.Count;

        var positionService = new PositionService(new PositionRepository(db));
        Positions.Clear();
        foreach (var pos in positionService.GetAll())
            Positions.Add(new PositionItemViewModel(pos));

        // Przywróć zaznaczone stanowisko po odświeżeniu
        if (SelectedNewUserPosition != null)
            SelectedNewUserPosition = Positions.FirstOrDefault(p => p.Id == SelectedNewUserPosition.Id);

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchQuery.ToLower();

        FilteredUsers.Clear();
        foreach (var u in Users.Where(u => string.IsNullOrEmpty(query) ||
            u.FirstName.ToLower().Contains(query) ||
            u.LastName.ToLower().Contains(query) ||
            u.Identifier.ToLower().Contains(query)))
            FilteredUsers.Add(u);

        FilteredOperations.Clear();
        foreach (var op in Operations.Where(o => string.IsNullOrEmpty(query) ||
            o.OperationName.ToLower().Contains(query) ||
            o.Description.ToLower().Contains(query)))
            FilteredOperations.Add(op);

        FilteredJobs.Clear();
        foreach (var j in Jobs.Where(j => string.IsNullOrEmpty(query) ||
            j.JobName.ToLower().Contains(query) ||
            j.Description.ToLower().Contains(query) ||
            j.Status.ToLower().Contains(query)))
            FilteredJobs.Add(j);

        FilteredPositions.Clear();
        foreach (var p in Positions.Where(p => string.IsNullOrEmpty(query) ||
            p.PositionName.ToLower().Contains(query)))
            FilteredPositions.Add(p);
    }

    [RelayCommand]
    private void Logout()
    {
        if (_mainVm is null)
            return;

        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    [RelayCommand]
    private void AddOperation()
    {
        if (string.IsNullOrWhiteSpace(NewOperationName))
            return;

        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));
        operationService.Add(NewOperationName, NewOperationDescription ?? string.Empty);

        NewOperationName = string.Empty;
        NewOperationDescription = string.Empty;
        RefreshAll();
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
        RefreshAll();
    }


    [RelayCommand]
    private void PrepareAddJob()
    {

        AvailableOperationsForAssign.Clear();

        foreach (var existing in Operations)
        {
            var op = new Operation { Id = existing.OperationName.GetHashCode(), OperationName = existing.OperationName, Description = existing.Description, CurrentWorkersCount = existing.CurrentWorkersCount };
            var vm = new OperationItemViewModel(op);
            vm.PropertyChanged += (_, __) => ConfirmAddJobCommand.NotifyCanExecuteChanged();
            AvailableOperationsForAssign.Add(vm);
        }

        IsAssignOperationsVisible = true;
    }

    [RelayCommand(CanExecute = nameof(CanAddJob))]
    private void AddJob()
    {
        PrepareAddJob();
    }
    private bool CanAddJob()
    {
        return !string.IsNullOrWhiteSpace(NewJobName) &&
               !string.IsNullOrWhiteSpace(NewJobDescription);
    }


    [RelayCommand(CanExecute = nameof(CanConfirmAddJob))]
    private void ConfirmAddJob()
    {
        if (string.IsNullOrWhiteSpace(NewJobName))
            return;


        if (!AvailableOperationsForAssign.Any(o => o.IsSelected))
            return;
        using var db = new DatabaseContext();
        var selectedNames = AvailableOperationsForAssign.Where(o => o.IsSelected).Select(o => o.OperationName).ToList();
        var selectedOpIds = db.Operations.Where(o => selectedNames.Contains(o.OperationName)).Select(o => o.Id).ToList();

        var jobService = new JobService(new JobRepository(db));
        jobService.Add(NewJobName, NewJobDescription, selectedOpIds, NewJobStatus);

        IsAssignOperationsVisible = false;
        NewJobName = string.Empty;
        NewJobDescription = string.Empty;
        NewJobStatus = "Nowe";
        RefreshAll();
    }

    private bool CanConfirmAddJob()
    {
        return !string.IsNullOrWhiteSpace(NewJobName) && 
               !string.IsNullOrWhiteSpace(NewJobDescription) && 
               AvailableOperationsForAssign.Any(o => o.IsSelected);
    }

    [RelayCommand]
    private void CancelAddJob()
    {
        IsAssignOperationsVisible = false;
    }

    [RelayCommand]
    private void RemoveJob(JobItemViewModel? item)
    {
        using var db = new DatabaseContext();
        var jobService = new JobService(new JobRepository(db));

        if (item is not null)
        {
            jobService.Remove(item.JobName);
        }
        else
        {
            var selected = Jobs.Where(j => j.IsSelected).ToList();
            foreach (var s in selected)
            {
                jobService.Remove(s.JobName);
            }
        }
        RefreshAll();
    }

    [RelayCommand]
    private void OpenJobOperations(JobItemViewModel? item)
    {
        if (item is null)
            return;

        using var db = new DatabaseContext();
        var job = db.Jobs.FirstOrDefault(j => j.Id == item.Id);
        if (job is null)
            return;

        SelectedJobForOperations = item;
        JobOperationsTitle = $"Operacje dla zlecenia: {job.JobName}";

        AvailableOperationsForJobEdit.Clear();
        foreach (var op in db.Operations.OrderBy(o => o.OperationName).ToList())
        {
            AvailableOperationsForJobEdit.Add(new OperationItemViewModel(op));
        }

        JobOperations.Clear();
        var operations = db.JobTasks
            .Where(jt => jt.JobId == item.Id)
            .Include(jt => jt.Operation)
            .OrderBy(jt => jt.Order)
            .ToList();

        foreach (var jt in operations)
        {
            JobOperations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));
        }

        NormalizeJobOperationOrder();
        SelectedOperationToAdd = null;
        IsJobOperationsVisible = true;
    }

    [RelayCommand]
    private void CloseJobOperations()
    {
        IsJobOperationsVisible = false;
        SelectedJobForOperations = null;
        SelectedOperationToAdd = null;
        JobOperations.Clear();
    }

    [RelayCommand]
    private void ShowJobDetails(JobItemViewModel? item)
    {
        if (item is null) return;
        
        using var db = new DatabaseContext();
        var job = db.Jobs.FirstOrDefault(j => j.Id == item.Id);
        if (job is null) return;

        JobDetailsTitle = $"Szczegóły zlecenia: {job.JobName}";
        JobDetailsOperations.Clear();
        
        var operations = db.JobTasks
            .Where(jt => jt.JobId == item.Id)
            .Include(jt => jt.Operation)
            .OrderBy(jt => jt.Order)
            .ToList();

        foreach (var jt in operations)
        {
            JobDetailsOperations.Add(new JobOperationItemViewModel(jt.Id, jt.OperationId, jt.Operation.OperationName, jt.Operation.Description, jt.Order, jt.Status, jt.ExecutionTime));
        }
        
        IsJobDetailsVisible = true;
    }

    [RelayCommand]
    private void CloseJobDetails()
    {
        IsJobDetailsVisible = false;
        JobDetailsOperations.Clear();
    }

    [RelayCommand]
    private void AddOperationToJob()
    {
        if (SelectedOperationToAdd is null)
            return;

        if (JobOperations.Any(o => o.OperationId == SelectedOperationToAdd.Id))
            return;

        JobOperations.Add(new JobOperationItemViewModel(0, SelectedOperationToAdd.Id, SelectedOperationToAdd.OperationName, SelectedOperationToAdd.Description, JobOperations.Count, "Nowe", TimeSpan.Zero));
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void RemoveOperationFromJob(JobOperationItemViewModel? item)
    {
        if (item is null)
            return;

        JobOperations.Remove(item);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void MoveJobOperationUp(JobOperationItemViewModel? item)
    {
        if (item is null)
            return;

        var index = JobOperations.IndexOf(item);
        if (index <= 0)
            return;

        JobOperations.Move(index, index - 1);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void MoveJobOperationDown(JobOperationItemViewModel? item)
    {
        if (item is null)
            return;

        var index = JobOperations.IndexOf(item);
        if (index < 0 || index >= JobOperations.Count - 1)
            return;

        JobOperations.Move(index, index + 1);
        NormalizeJobOperationOrder();
    }

    [RelayCommand]
    private void SaveJobOperations()
    {
        if (SelectedJobForOperations is null)
            return;

        using var db = new DatabaseContext();
        var existing = db.JobTasks.Where(jt => jt.JobId == SelectedJobForOperations.Id).ToList();
        var selectedOperationIds = JobOperations.Select(o => o.OperationId).ToList();

        var toDelete = existing.Where(jt => !selectedOperationIds.Contains(jt.OperationId)).ToList();
        if (toDelete.Count > 0)
        {
            db.JobTasks.RemoveRange(toDelete);
        }

        for (var i = 0; i < JobOperations.Count; i++)
        {
            var selected = JobOperations[i];
            var existingTask = existing.FirstOrDefault(jt => jt.OperationId == selected.OperationId);

            if (existingTask is null)
            {
                db.JobTasks.Add(new JobTask
                {
                    JobId = SelectedJobForOperations.Id,
                    OperationId = selected.OperationId,
                    Order = i,
                    Status = selected.Status,
                    OperationStart = DateTime.Now,
                    OperationEnd = DateTime.Now,
                    ExecutionTime = TimeSpan.Zero
                });
            }
            else
            {
                existingTask.Order = i;
                existingTask.Status = selected.Status;
            }
        }

        db.SaveChanges();

        // Auto-complete: if all operations are "Zakończone", set job status to "Zakończone"
        var allTasks = db.JobTasks.Where(jt => jt.JobId == SelectedJobForOperations.Id).ToList();
        if (allTasks.Count > 0 && allTasks.All(jt => jt.Status == "Zakończone"))
        {
            var job = db.Jobs.FirstOrDefault(j => j.Id == SelectedJobForOperations.Id);
            if (job is not null)
            {
                job.Status = "Zakończone";
                db.SaveChanges();
            }
        }

        IsJobOperationsVisible = false;
        RefreshAll();
    }

    private void NormalizeJobOperationOrder()
    {
        for (var i = 0; i < JobOperations.Count; i++)
        {
            JobOperations[i].Order = i;
        }
    }

    [RelayCommand]
    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserFirstName) ||
            string.IsNullOrWhiteSpace(NewUserLastName) ||
            string.IsNullOrWhiteSpace(NewUserPassword))
            return;

        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));
        userService.Add(
            NewUserFirstName,
            NewUserLastName,
            NewUserID ?? string.Empty,
            NewUserPassword,
            SelectedNewUserPosition?.Id);

        NewUserFirstName = "";
        NewUserLastName = "";
        NewUserID = "";
        NewUserPassword = "";
        SelectedNewUserPosition = null;
        RefreshAll();
    }

    [RelayCommand]
    private void RemoveUser(UserItemViewModel? item)
    {
        using var db = new DatabaseContext();
        var userService = new UserService(new UserRepository(db));

        if (item is not null)
        {
            userService.Remove(item.FirstName, item.LastName, item.Identifier);
        }
        else
        {
            var selected = Users.Where(u => u.IsSelected).ToList();
            foreach (var s in selected)
            {
                userService.Remove(s.FirstName, s.LastName, s.Identifier);
            }
        }
        RefreshAll();
    }

    [RelayCommand]
    private void AddPosition()
    {
        if (string.IsNullOrWhiteSpace(NewPositionName))
            return;
        if (!decimal.TryParse(NewPositionHourlyRate.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var rate))
            return;

        using var db = new DatabaseContext();
        var positionService = new PositionService(new PositionRepository(db));
        positionService.Add(NewPositionName, rate);

        NewPositionName = string.Empty;
        NewPositionHourlyRate = string.Empty;
        RefreshAll();
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
            var selected = Positions.Where(p => p.IsSelected).ToList();
            foreach (var s in selected)
                positionService.Remove(s.Id);
        }
        RefreshAll();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task EditUser(UserItemViewModel? item)
    {
        if (item is null) return;

        var vm = new EditUserViewModel(item, Positions);
        var window = new Views.EditUserWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        RefreshAll();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task EditPosition(PositionItemViewModel? item)
    {
        if (item is null) return;

        var vm = new EditPositionViewModel(item);
        var window = new Views.EditPositionWindow(vm);

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        RefreshAll();
    }

    [RelayCommand]
    private void ShowUsers() { CurrentTabIndex = 0; RefreshAll(); }

    [RelayCommand]
    private void ShowOperations() { CurrentTabIndex = 1; RefreshAll(); }

    [RelayCommand]
    private void ShowJobs() { CurrentTabIndex = 2; RefreshAll(); }

    [RelayCommand]
    public void ShowPositions() { CurrentTabIndex = 3; RefreshAll(); }

    [RelayCommand]
    public void ShowProductionTime() { CurrentTabIndex = 4; RefreshAll(); }
}

public partial class UserItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Identifier { get; }
    public string EmploymentDate { get; }
    public string PositionName { get; }

    public UserItemViewModel(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Identifier = user.Identifier;
        EmploymentDate = user.EmploymentDate;
        PositionName = user.Position?.PositionName ?? "—";
    }
}

public partial class OperationItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string OperationName { get; }
    public string Description { get; }
    public int CurrentWorkersCount { get; }

    public OperationItemViewModel(Operation op)
    {
        Id = op.Id;
        OperationName = op.OperationName;
        Description = op.Description;
        CurrentWorkersCount = op.CurrentWorkersCount;
    }
}

public partial class JobOperationItemViewModel : ObservableObject
{
    public int JobTaskId { get; }
    public int OperationId { get; }
    public string OperationName { get; }
    public string Description { get; }

    [ObservableProperty] private int _order;
    [ObservableProperty] private string _status;
    
    public TimeSpan ExecutionTime { get; }
    public double ExecutionTimeHours => ExecutionTime.TotalHours;

    public JobOperationItemViewModel(int jobTaskId, int operationId, string operationName, string description, int order, string status, TimeSpan executionTime = default)
    {
        JobTaskId = jobTaskId;
        OperationId = operationId;
        OperationName = operationName;
        Description = description;
        _order = order;
        _status = status;
        ExecutionTime = executionTime;
    }
}
public partial class OperationSelectionItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string OperationName { get; }
    public string Description { get; }

    public OperationSelectionItemViewModel(Operation op)
    {
        Id = op.Id;
        OperationName = op.OperationName;
        Description = op.Description;
    }
}

public partial class PositionItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string PositionName { get; }
    public decimal HourlyRate { get; }

    public PositionItemViewModel(Position position)
    {
        Id = position.Id;
        PositionName = position.PositionName;
        HourlyRate = position.HourlyRate;
    }
}

public partial class JobItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public int Id { get; }
    public string JobName { get; }
    public string Description { get; }
    public string Status { get; }
    public DateTime CreatedAt { get; }
    public double TotalExecutionTimeHours { get; }

    public JobItemViewModel(Job job)
    {
        Id = job.Id;
        JobName = job.JobName;
        Description = job.Description;
        Status = job.Status;
        CreatedAt = job.CreatedAt;
        TotalExecutionTimeHours = job.JobTasks?.Sum(jt => jt.ExecutionTime.TotalHours) ?? 0;
    }
}
