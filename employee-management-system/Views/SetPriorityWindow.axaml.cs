using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class SetPriorityWindow : Window
{
    public SetPriorityWindow() => InitializeComponent();

    public SetPriorityWindow(SetPriorityViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
