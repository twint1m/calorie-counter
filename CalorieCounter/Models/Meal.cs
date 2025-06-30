using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CalorieCounter.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; } 
        public ObservableCollection<FoodEntry> Foods { get; set; } = new();
        public int DayLogId { get; set; } 

        public Meal() {}
        public Meal(Meal other)
        {
            Name = other.Name;
            Date = other.Date;
            Foods = new ObservableCollection<FoodEntry>(other.Foods.Select(f => new FoodEntry {
                Name = f.Name,
                Calories = f.Calories,
                Protein = f.Protein,
                Fat = f.Fat,
                Carbs = f.Carbs
            }));
        }
    }
} 