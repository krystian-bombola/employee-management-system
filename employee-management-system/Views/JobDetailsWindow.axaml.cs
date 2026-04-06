using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class JobDetailsWindow : Window
{
    public JobDetailsWindow()
    {
        InitializeComponent();

        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                Close();
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel);
    }

    public JobDetailsWindow(JobDetailsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is JobDetailsViewModel vm)
        {
            vm.Close();
            return;
        }

        Close();
    }
}
