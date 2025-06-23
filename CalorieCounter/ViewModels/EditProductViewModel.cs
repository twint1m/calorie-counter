using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalorieCounter.Models;

namespace CalorieCounter.ViewModels;

public class EditProductViewModel : INotifyPropertyChanged
{
    private string _name;
    private string _calories;
    private string _protein;
    private string _fat;
    private string _carbs;

    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
    public string Calories { get => _calories; set { _calories = value; OnPropertyChanged(); } }
    public string Protein { get => _protein; set { _protein = value; OnPropertyChanged(); } }
    public string Fat { get => _fat; set { _fat = value; OnPropertyChanged(); } }
    public string Carbs { get => _carbs; set { _carbs = value; OnPropertyChanged(); } }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler? OnSave;
    public event EventHandler? OnCancel;

    public EditProductViewModel(Product product)
    {
        Name = product.Name;
        Calories = product.Calories.ToString();
        Protein = product.Protein.ToString();
        Fat = product.Fat.ToString();
        Carbs = product.Carbs.ToString();
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Save()
    {
        OnSave?.Invoke(this, EventArgs.Empty);
    }
    private void Cancel()
    {
        OnCancel?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
} 