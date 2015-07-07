using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// root object of the mensa api.
    /// </summary>
    public class ListOfDays
    {
        public List<Day> days { get; set; }
    }
}
