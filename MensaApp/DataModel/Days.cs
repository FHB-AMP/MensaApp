using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel
{
    public class Meal1
    {
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }

    public class Meal2
    {
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }

    public class Meal3
    {
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }

    public class Meal4
    {
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }

    public class Day
    {
        public string date { get; set; }
        public Meal1 meal1 { get; set; }
        public Meal2 meal2 { get; set; }
        public Meal3 meal3 { get; set; }
        public Meal4 meal4 { get; set; }
    }

    public class RootObjectDays
    {
        public List<Day> days { get; set; }
    }
}
