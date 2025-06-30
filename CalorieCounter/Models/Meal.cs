using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CalorieCounter.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; } 
        public ObservableCollection<FoodEntry> Foods { get; set; } = new();
        public int DayLogId { get; set; } 
    }
} 