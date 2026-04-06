using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class ConfirmDeleteWindow : Window
{
    public ConfirmDeleteWindow()
    {
        InitializeComponent();

        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            if (DataContext is not ConfirmDeleteViewModel vm)
                return;

            if (e.Key == Key.Escape)
            {
                vm.CancelCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                vm.ConfirmCommand.Execute(null);
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel);

        Opened += (s, e) => this.Find<Button>("CancelButton")?.Focus();
    }
}
