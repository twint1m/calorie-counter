using System;

namespace CalorieCounter.Models
{
    public class FoodEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }
        public double Protein { get; set; }
        public int MealId { get; set; }
    }
} 