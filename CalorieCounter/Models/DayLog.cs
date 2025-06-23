using System;
using System.Collections.Generic;

namespace CalorieCounter.Models
{
    public class DayLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<Meal> Meals { get; set; } = new();
    }
} 