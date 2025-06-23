using Avalonia.Controls;
using CalorieCounter.ViewModels;
using CalorieCounter.Models;

namespace CalorieCounter.Views;

public partial class EditProductWindow : Window
{
    public EditProductWindow(EditProductViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        vm.OnSave += (_, __) => { this.Close(true); };
        vm.OnCancel += (_, __) => { this.Close(false); };
    }
} 