using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// Description of a allergen
    /// </summary>
    public class AllergenDescription
    {
        public string id { get; set; }
        public string definition { get; set; }
        public string containedIn { get; set; }
    }
}
