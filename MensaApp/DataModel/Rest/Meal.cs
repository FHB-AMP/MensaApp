using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// defines a specific meal offer of the mensa.
    /// </summary>
    public class Meal
    {
        public int mealNumber { get; set; }
        public string name { get; set; }
        public List<string> symbols { get; set; }
        public List<string> additives { get; set; }
        public List<string> allergens { get; set; }
    }
}
