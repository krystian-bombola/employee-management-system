using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class EditPositionWindow : Window
{
    public EditPositionWindow()
    {
        InitializeComponent();

        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (DataContext is EditPositionViewModel vm)
                {
                    vm.SaveCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }, RoutingStrategies.Tunnel);

        Opened += (s, e) =>
        {
            var positionNameTextBox = this.Find<TextBox>("PositionNameTextBox");
            positionNameTextBox?.Focus();
        };
    }

    public EditPositionWindow(EditPositionViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
