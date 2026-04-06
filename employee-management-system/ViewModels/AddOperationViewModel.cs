using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class AddOperationViewModel : ObservableObject
{
    public System.Action? CloseAction { get; set; }

    private string _operationName = string.Empty;
    public string OperationName
    {
        get => _operationName;
        set => SetProperty(ref _operationName, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(OperationName))
            return;

        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));
        operationService.Add(OperationName, Description);

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }
}
