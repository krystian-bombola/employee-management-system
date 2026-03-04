using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty]
    private string _employeeId = string.Empty;

    [ObservableProperty]
    private string _orderId = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorVisible;

    public LoginViewModel()
    {
        _mainVm = null!;
    }

    public LoginViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(EmployeeId) || string.IsNullOrWhiteSpace(OrderId))
        {
            ErrorMessage = "Wypełnij oba pola!";
            IsErrorVisible = true;
            return;
        }

        await Task.Delay(200); // symulacja operacji asynchronicznej

        // TODO: zastąp właściwą logiką uwierzytelniania
        if (EmployeeId == "admin" && OrderId == "1234")
        {
            ErrorMessage = string.Empty;
            IsErrorVisible = false;

            _mainVm.CurrentView = new UserPanelViewModel(EmployeeId, OrderId);
        }
        else
        {
            ErrorMessage = "Nieprawidłowe ID pracownika lub ID zlecenia.";
            IsErrorVisible = true;
        }
    }
}