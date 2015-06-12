using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel
{

    public class Additive
    {
        public string id { get; set; }
        public string definition { get; set; }
        public string meaning { get; set; }
    }

    public class Allergen
    {
        public string id { get; set; }
        public string definition { get; set; }
        public string containedIn { get; set; }
    }

    public class RootObjectAdditives
    {
        public List<Additive> additives { get; set; }
        public List<Allergen> allergens { get; set; }
    }

}
