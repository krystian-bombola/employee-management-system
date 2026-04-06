using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class EditJobOperationsWindow : Window
{
    public EditJobOperationsWindow()
    {
        InitializeComponent();
    }

    public EditJobOperationsWindow(EditJobOperationsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
