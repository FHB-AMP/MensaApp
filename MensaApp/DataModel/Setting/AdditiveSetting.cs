using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Setting
{
    /// <summary>
    /// Defines the settings of an additive the user has made;
    /// </summary>
    public class AdditiveSetting
    {        
        public AdditiveSetting() { }

        public AdditiveSetting(string id, bool isExcluded)
        {
            this.id = id;
            this.isExcluded = isExcluded;
        }
        public string id;
        public bool isExcluded;
    }
}
