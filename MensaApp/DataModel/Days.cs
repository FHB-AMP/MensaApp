using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel
{
    public class Meal
    {
        public int mealNumber { get; set; }
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }

    public class Day
    {
        public string date { get; set; }
        public List<Meal> meals { get; set; }
    }

    public class RootObjectDays
    {
        public List<Day> days { get; set; }
    }
}
