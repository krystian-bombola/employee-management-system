using CommunityToolkit.Mvvm.ComponentModel;

namespace employee_management_system.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        _currentView = new LoginViewModel(this);
    }
}