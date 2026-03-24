using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class EditPositionWindow : Window
{
    public EditPositionWindow()
    {
        InitializeComponent();
    }

    public EditPositionWindow(EditPositionViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}