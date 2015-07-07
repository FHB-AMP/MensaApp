using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// day included a date and a list of meals
    /// </summary>
    public class Day
    {
        public string date { get; set; }
        public List<Meal> meals { get; set; }
    }
}
