using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using CalorieCounter.Models;
using Avalonia.Controls;
using CalorieCounter.Views;

namespace CalorieCounter.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<Product> Products { get; } = new();

    private string _productName = string.Empty;
    public string ProductName
    {
        get => _productName;
        set { _productName = value; OnPropertyChanged(); }
    }

    private string _productCalories = string.Empty;
    public string ProductCalories
    {
        get => _productCalories;
        set { _productCalories = value; OnPropertyChanged(); }
    }

    private string _productProtein = string.Empty;
    public string ProductProtein
    {
        get => _productProtein;
        set { _productProtein = value; OnPropertyChanged(); }
    }

    private string _productFat = string.Empty;
    public string ProductFat
    {
        get => _productFat;
        set { _productFat = value; OnPropertyChanged(); }
    }

    private string _productCarbs = string.Empty;
    public string ProductCarbs
    {
        get => _productCarbs;
        set { _productCarbs = value; OnPropertyChanged(); }
    }

    public double TotalCalories => Products.Sum(p => p.Calories);
    public double TotalProtein => Products.Sum(p => p.Protein);
    public double TotalFat => Products.Sum(p => p.Fat);
    public double TotalCarbs => Products.Sum(p => p.Carbs);

    public ICommand AddProductCommand { get; }

    public MainWindowViewModel()
    {
        AddProductCommand = new RelayCommand(AddProduct);
        Products.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalCalories));
    }

    private void AddProduct()
    {
        if (string.IsNullOrWhiteSpace(ProductName)) return;
        if (!double.TryParse(ProductCalories, out var cal) || cal < 0) return;
        double protein = 0, fat = 0, carbs = 0;
        double.TryParse(ProductProtein, out protein);
        double.TryParse(ProductFat, out fat);
        double.TryParse(ProductCarbs, out carbs);
        Products.Add(new Product { Name = ProductName, Calories = cal, Protein = protein, Fat = fat, Carbs = carbs });
        ProductName = string.Empty;
        ProductCalories = string.Empty;
        ProductProtein = string.Empty;
        ProductFat = string.Empty;
        ProductCarbs = string.Empty;
        OnPropertyChanged(nameof(TotalCalories));
        OnPropertyChanged(nameof(TotalProtein));
        OnPropertyChanged(nameof(TotalFat));
        OnPropertyChanged(nameof(TotalCarbs));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    public RelayCommand(Action execute) => _execute = execute;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute();
}
