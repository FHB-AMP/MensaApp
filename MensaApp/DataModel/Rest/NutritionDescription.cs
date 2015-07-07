using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// Description of a kind of nutrition
    /// </summary>
    public class NutritionDescription
    {
        public string id;
        public string name;
        public string definition;
        public List<string> excludedSymbols;
        public List<string> excludedAdditives;
        public List<string> excludedAllergens;
    }
}
