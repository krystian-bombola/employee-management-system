using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Models;
using System.Collections.ObjectModel;

namespace employee_management_system.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainVm;

    [ObservableProperty] private string _loggedUserSurname = string.Empty;

    public AdminUsersSectionViewModel UsersViewModel { get; } = new();
    public AdminOperationsSectionViewModel OperationsViewModel { get; } = new();
    public AdminJobsSectionViewModel JobsViewModel { get; } = new();
    public AdminPositionsSectionViewModel PositionsViewModel { get; } = new();
    public AdminProductionTimeSectionViewModel ProductionTimeViewModel { get; } = new();

    private int _currentTabIndex = 0;
    public int CurrentTabIndex
    {
        get => _currentTabIndex;
        set
        {
            if (SetProperty(ref _currentTabIndex, value))
            {
                OnCurrentTabIndexChanged();
            }
        }
    }

    public bool IsUsersVisible => CurrentTabIndex == 0;
    public bool IsOperationsVisible => CurrentTabIndex == 1;
    public bool IsJobsVisible => CurrentTabIndex == 2;
    public bool IsPositionsVisible => CurrentTabIndex == 3;
    public bool IsProductionTimeVisible => CurrentTabIndex == 4;

    public AdminWindowViewModel() { }

    public AdminWindowViewModel(MainWindowViewModel mainVm, User user)
    {
        _mainVm = mainVm;
        LoggedUserSurname = user.LastName;

        // Initialize and load all data
        RefreshAllSections();
    }

    private void OnCurrentTabIndexChanged()
    {
        OnPropertyChanged(nameof(IsUsersVisible));
        OnPropertyChanged(nameof(IsOperationsVisible));
        OnPropertyChanged(nameof(IsJobsVisible));
        OnPropertyChanged(nameof(IsPositionsVisible));
        OnPropertyChanged(nameof(IsProductionTimeVisible));
        
        // Odśwież widok przy każdym wejściu w zakładkę (poza logowaniem, żeby dane zawsze były spójne)
        switch(CurrentTabIndex)
        {
            case 0: UsersViewModel.Refresh(); break;
            case 1: OperationsViewModel.Refresh(); break;
            case 2: JobsViewModel.Refresh(); break;
            case 3: PositionsViewModel.Refresh(); break;
            case 4: ProductionTimeViewModel.Refresh(); break;
        }
    }

    private void RefreshAllSections()
    {
        UsersViewModel.Refresh();
        OperationsViewModel.Refresh();
        JobsViewModel.Refresh();
        PositionsViewModel.Refresh();
        ProductionTimeViewModel.Refresh();
    }

    [RelayCommand]
    private void Logout()
    {
        if (_mainVm is null)
            return;

        _mainVm.CurrentView = new LoginViewModel(_mainVm);
    }

    [RelayCommand] private void ShowUsers() { CurrentTabIndex = 0; }
    [RelayCommand] private void ShowOperations() { CurrentTabIndex = 1; }
    [RelayCommand] private void ShowJobs() { CurrentTabIndex = 2; }
    [RelayCommand] private void ShowPositions() { CurrentTabIndex = 3; }
    [RelayCommand] private void ShowProductionTime() { CurrentTabIndex = 4; }
}
