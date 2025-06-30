using System;
using System.Collections.Generic;
using System.Linq;

namespace CalorieCounter.Models
{
    public class DayLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<Meal> Meals { get; set; } = new();

        public double TotalCalories => Meals?.Sum(meal => meal.Foods?.Sum(food => food.Calories) ?? 0) ?? 0;
    }
} 