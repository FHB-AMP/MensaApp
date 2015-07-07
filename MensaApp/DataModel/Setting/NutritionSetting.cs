using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Setting
{
    /// <summary>
    /// Defines the settings of nutrition the user has made;
    /// </summary>
    public class NutritionSetting
    {
        public NutritionSetting() { }

        public NutritionSetting(string id, bool isSelected)
        {
            this.id = id;
            this.isSelected = isSelected;
        }

        public string id;
        public bool isSelected;
    }
}
