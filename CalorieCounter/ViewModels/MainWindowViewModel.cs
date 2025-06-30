using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using CalorieCounter.Models;
using Avalonia.Controls;
using CalorieCounter.Views;
using System.Globalization;
using System.Collections.Generic;

namespace CalorieCounter.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<DayLog> DayLogs { get; } = new();
    public ObservableCollection<DayLog> WeekLogs { get; } = new();
    public ObservableCollection<Meal> MealTemplates { get; } = new();

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

    private DayLog? _selectedDayLog;
    public DayLog? SelectedDayLog
    {
        get => _selectedDayLog;
        set
        {
            // Если value не из WeekLogs, ищем по дате
            var fromWeek = value != null ? WeekLogs.FirstOrDefault(d => d.Date.Date == value.Date.Date) : null;
            var newValue = fromWeek ?? value;
            if (_selectedDayLog == newValue) return;
            _selectedDayLog = newValue;
            if (_selectedDayLog != null && _selectedDayLog.Meals.Count == 0)
                _selectedDayLog.Meals.Add(new Meal { Name = "Meal", Date = _selectedDayLog.Date });
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalCaloriesForDay));
            OnPropertyChanged(nameof(FoodsForSelectedDay));
        }
    }

    private string _newMealName = string.Empty;
    public string NewMealName
    {
        get => _newMealName;
        set { _newMealName = value; OnPropertyChanged(); }
    }

    public ObservableCollection<FoodEntry> NewMealFoods { get; } = new();

    private Meal? _selectedMealTemplate;
    public Meal? SelectedMealTemplate
    {
        get => _selectedMealTemplate;
        set { _selectedMealTemplate = value; OnPropertyChanged(); }
    }

    public double TotalCalories => Products.Sum(p => p.Calories);
    public double TotalProtein => Products.Sum(p => p.Protein);
    public double TotalFat => Products.Sum(p => p.Fat);
    public double TotalCarbs => Products.Sum(p => p.Carbs);
    public double TotalCaloriesForDay => SelectedDayLog?.TotalCalories ?? 0;

    public ObservableCollection<FoodEntry> FoodsForSelectedDay
    {
        get
        {
            if (SelectedDayLog != null && SelectedDayLog.Meals.Count > 0)
                return SelectedDayLog.Meals.SelectMany(m => m.Foods).ToObservableCollection();
            return new ObservableCollection<FoodEntry>();
        }
    }

    public ICommand AddProductCommand { get; }
    public ICommand AddProductToDayCommand { get; }
    public ICommand CreateOrSelectDayCommand { get; }
    public ICommand AddMealTemplateCommand { get; }
    public ICommand AddFoodToNewMealCommand { get; }
    public ICommand AddMealTemplateToDayCommand { get; }

    public MainWindowViewModel()
    {
        AddProductCommand = new RelayCommand(AddProduct);
        Products.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalCalories));
        AddProductToDayCommand = new RelayCommand(AddProductToDay);
        CreateOrSelectDayCommand = new RelayCommand(CreateOrSelectDay);
        AddMealTemplateCommand = new RelayCommand(AddMealTemplate);
        AddFoodToNewMealCommand = new RelayCommand(AddFoodToNewMeal);
        AddMealTemplateToDayCommand = new RelayCommand(AddMealTemplateToDay);
        InitWeek();
    }

    private void InitWeek()
    {
        WeekLogs.Clear();
        var today = DateTime.Today;
        // Найти понедельник текущей недели
        int delta = DayOfWeek.Monday - today.DayOfWeek;
        if (delta > 0) delta -= 7; // если сегодня воскресенье
        var monday = today.AddDays(delta);
        for (int i = 0; i < 7; i++)
        {
            var date = monday.AddDays(i);
            var log = new DayLog { Date = date };
            log.Meals.Add(new Meal { Name = "Meal", Date = date });
            WeekLogs.Add(log);
        }
        SelectedDayLog = WeekLogs.FirstOrDefault(w => w.Date.Date == today) ?? WeekLogs[0];
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

    private void AddProductToDay()
    {
        if (SelectedDayLog == null) return;
        if (string.IsNullOrWhiteSpace(ProductName)) return;
        if (!double.TryParse(ProductCalories, out var cal) || cal < 0) return;
        double protein = 0, fat = 0, carbs = 0;
        double.TryParse(ProductProtein, out protein);
        double.TryParse(ProductFat, out fat);
        double.TryParse(ProductCarbs, out carbs);

        // Найти или создать Meal
        var meal = SelectedDayLog.Meals.FirstOrDefault();
        if (meal == null)
        {
            meal = new Meal { Name = "Meal", Date = SelectedDayLog.Date };
            SelectedDayLog.Meals.Add(meal);
        }
        meal.Foods.Add(new FoodEntry
        {
            Name = ProductName,
            Calories = cal,
            Protein = protein,
            Fat = fat,
            Carbs = carbs
        });
        // Очистить поля
        ProductName = string.Empty;
        ProductCalories = string.Empty;
        ProductProtein = string.Empty;
        ProductFat = string.Empty;
        ProductCarbs = string.Empty;
        OnPropertyChanged(nameof(TotalCaloriesForDay));
        OnPropertyChanged(nameof(SelectedDayLog));
        OnPropertyChanged(nameof(FoodsForSelectedDay));
    }

    private void CreateOrSelectDay()
    {
        var today = SelectedDayLog?.Date.Date ?? DateTime.Now.Date;
        var log = DayLogs.FirstOrDefault(d => d.Date.Date == today);
        if (log == null)
        {
            log = new DayLog { Date = today };
            // Гарантируем хотя бы один Meal
            log.Meals.Add(new Meal { Name = "Meal", Date = today });
            DayLogs.Add(log);
        }
        else if (log.Meals.Count == 0)
        {
            log.Meals.Add(new Meal { Name = "Meal", Date = today });
        }
        SelectedDayLog = log;
        OnPropertyChanged(nameof(SelectedDayLog));
        OnPropertyChanged(nameof(TotalCaloriesForDay));
    }

    private void AddMealTemplate()
    {
        if (string.IsNullOrWhiteSpace(NewMealName) || NewMealFoods.Count == 0) return;
        var meal = new Meal { Name = NewMealName, Foods = new ObservableCollection<FoodEntry>(NewMealFoods.Select(f => new FoodEntry {
            Name = f.Name,
            Calories = f.Calories,
            Protein = f.Protein,
            Fat = f.Fat,
            Carbs = f.Carbs
        })) };
        MealTemplates.Add(meal);
        NewMealName = string.Empty;
        NewMealFoods.Clear();
    }

    private void AddFoodToNewMeal()
    {
        if (string.IsNullOrWhiteSpace(ProductName)) return;
        if (!double.TryParse(ProductCalories, out var cal) || cal < 0) return;
        double protein = 0, fat = 0, carbs = 0;
        double.TryParse(ProductProtein, out protein);
        double.TryParse(ProductFat, out fat);
        double.TryParse(ProductCarbs, out carbs);
        NewMealFoods.Add(new FoodEntry
        {
            Name = ProductName,
            Calories = cal,
            Protein = protein,
            Fat = fat,
            Carbs = carbs
        });
        ProductName = string.Empty;
        ProductCalories = string.Empty;
        ProductProtein = string.Empty;
        ProductFat = string.Empty;
        ProductCarbs = string.Empty;
    }

    private void AddMealTemplateToDay()
    {
        if (SelectedDayLog == null || SelectedMealTemplate == null) return;
        var meal = new Meal(SelectedMealTemplate) { Date = SelectedDayLog.Date };
        SelectedDayLog.Meals.Add(meal);
        OnPropertyChanged(nameof(FoodsForSelectedDay));
        OnPropertyChanged(nameof(SelectedDayLog));
        OnPropertyChanged(nameof(TotalCaloriesForDay));
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

public static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        => new ObservableCollection<T>(source);
}
