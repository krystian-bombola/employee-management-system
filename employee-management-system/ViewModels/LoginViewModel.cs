using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using Microsoft.EntityFrameworkCore;
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
        using var db = new DatabaseContext();
        // TODO: zastąp właściwą logiką uwierzytelniania
        if (EmployeeId == "admin")
        {
            ErrorMessage = string.Empty;
            IsErrorVisible = false;

            _mainVm.CurrentView = new AdminWindowViewModel();
            return;
        }
        var user = await db.Uzytkownicy.FirstOrDefaultAsync(u => u.Identyfikator == EmployeeId);
        var job = await db.Zlecenia.FirstOrDefaultAsync(z => z.NazwaZlecenia == OrderId);

        if (user != null && job !=null)
        {
            ErrorMessage = string.Empty;
            IsErrorVisible = false;

            
            _mainVm.CurrentView = new UserPanelViewModel(EmployeeId, OrderId)
            {
                OrderId = job.NazwaZlecenia,
            };
        }
        else
        {
            ErrorMessage = "Nieprawidłowe ID pracownika lub ID zlecenia.";
            IsErrorVisible = true;
        }
    }
}