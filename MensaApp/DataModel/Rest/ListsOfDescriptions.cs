using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Rest
{
    /// <summary>
    /// Contains lists of additives, allergens and nutrition descriptions.
    /// </summary>
    public class ListsOfDescriptions
    {

        public ListsOfDescriptions()
        {
            this.nutritions = new List<NutritionDescription>();
            this.additives = new List<AdditiveDescription>();
            this.allergens = new List<AllergenDescription>();
            this.infoSymbols = new List<InfoSymbolDescription>();
        }

        public List<NutritionDescription> nutritions { get; set; }
        public List<AdditiveDescription> additives { get; set; }
        public List<AllergenDescription> allergens { get; set; }
        public List<InfoSymbolDescription> infoSymbols { get; set; }
    }

}
