using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using employee_management_system.Data;
using employee_management_system.Repositories;
using employee_management_system.Services;

namespace employee_management_system.ViewModels;

public partial class EditPositionViewModel : ObservableObject
{
    private readonly int _positionId;
    public System.Action? CloseAction { get; set; }

    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _windowTitle = string.Empty;
    [ObservableProperty] private string _saveButtonText = string.Empty;

    private string _positionName = string.Empty;
    public string PositionName { get => _positionName; set => SetProperty(ref _positionName, value); }

    private string _hourlyRate = string.Empty;
    public string HourlyRate { get => _hourlyRate; set => SetProperty(ref _hourlyRate, value); }

    public EditPositionViewModel() { }

    public EditPositionViewModel(bool isAddMode)
    {
        IsEditMode = !isAddMode;
        WindowTitle = "Dodaj stanowisko";
        SaveButtonText = "Dodaj";
    }

    public EditPositionViewModel(PositionItemViewModel position)
    {
        IsEditMode = true;
        WindowTitle = "Edytuj stanowisko";
        SaveButtonText = "Zapisz zmiany";
        _positionId = position.Id;
        PositionName = position.PositionName;
        HourlyRate = position.HourlyRate.ToString("F2");
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(PositionName)) return;
        if (!decimal.TryParse(HourlyRate.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var rate)) return;

        using var db = new DatabaseContext();
        var service = new PositionService(new PositionRepository(db));

        if (IsEditMode)
        {
            service.Update(_positionId, PositionName, rate);
        }
        else
        {
            service.Add(PositionName, rate);
        }

        CloseAction?.Invoke();
    }

    [RelayCommand]
    private void Cancel() => CloseAction?.Invoke();
}
