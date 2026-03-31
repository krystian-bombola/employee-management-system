using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class AdminJobsSectionView : UserControl
{
    public AdminJobsSectionView()
    {
        InitializeComponent();

        // Używamy Tunelu, aby przechwycić klawisze zanim przyciski je obsłużą
        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var focusManager = topLevel?.FocusManager;
            var currentFocus = focusManager?.GetFocusedElement() as Control;

            if (DataContext is AdminJobsSectionViewModel vm)
            {
                // Obsługa popupa usuwania
                if (vm.IsDeleteConfirmationVisible)
                {
                    if (e.Key == Key.Escape)
                    {
                        vm.CancelDeleteCommand.Execute(null);
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Enter)
                    {
                        vm.ConfirmDeleteCommand.Execute(null);
                        e.Handled = true;
                    }
                    return;
                }

                // Obsługa skoku do listy przy strzałkach (jeśli nie jesteśmy w liście)
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    var listBox = this.Find<ListBox>("JobsListBox");
                    bool isFocusInList = listBox != null && currentFocus != null && listBox.IsVisualAncestorOf(currentFocus);

                    if (!isFocusInList)
                    {
                        if (listBox != null && listBox.ItemCount > 0)
                        {
                            if (listBox.SelectedItem == null)
                            {
                                listBox.SelectedIndex = 0;
                            }
                            
                            listBox.Focus();
                            e.Handled = true;
                        }
                    }
                }
            }
        }, RoutingStrategies.Tunnel);

        DataContextChanged += (s, e) =>
        {
            if (DataContext is AdminJobsSectionViewModel vm)
            {
                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(AdminJobsSectionViewModel.IsDeleteConfirmationVisible))
                    {
                        if (vm.IsDeleteConfirmationVisible)
                        {
                            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                            {
                                var button = this.Find<Button>("CancelDeleteButton");
                                button?.Focus();
                            }, Avalonia.Threading.DispatcherPriority.Input);
                        }
                        else
                        {
                            // Powrót fokusu na listę po zamknięciu okna potwierdzenia
                            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                            {
                                this.Find<ListBox>("JobsListBox")?.Focus();
                            }, Avalonia.Threading.DispatcherPriority.Input);
                        }
                    }
                    else if (pe.PropertyName == nameof(AdminJobsSectionViewModel.SelectedJob) && vm.SelectedJob != null)
                    {
                        // Upewnij się, że element jest widoczny i sfokusowany po zmianie
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            var listBox = this.Find<ListBox>("JobsListBox");
                            if (listBox != null && listBox.SelectedItem != null)
                            {
                                listBox.Focus();
                            }
                        }, Avalonia.Threading.DispatcherPriority.Input);
                    }
                };
            }
        };
    }
}
