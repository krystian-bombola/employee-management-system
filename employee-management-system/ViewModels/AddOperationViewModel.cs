using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class AddOperationViewModel : ObservableObject
{
    private readonly int _operationId;
    public System.Action? CloseAction { get; set; }

    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _windowTitle = string.Empty;
    [ObservableProperty] private string _saveButtonText = string.Empty;

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

    public AddOperationViewModel()
    {
        IsEditMode = false;
        WindowTitle = "Dodaj operację";
        SaveButtonText = "Dodaj";
    }

    public AddOperationViewModel(OperationItemViewModel operation)
    {
        IsEditMode = true;
        WindowTitle = "Edytuj operację";
        SaveButtonText = "Zapisz zmiany";
        _operationId = operation.Id;
        OperationName = operation.OperationName;
        Description = operation.Description;
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(OperationName))
            return;

        using var db = new DatabaseContext();
        var operationService = new OperationService(new OperationRepository(db));

        if (IsEditMode)
        {
            operationService.Update(_operationId, OperationName, Description);
        }
        else
        {
            operationService.Add(OperationName, Description);
        }

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke();
    }
}
