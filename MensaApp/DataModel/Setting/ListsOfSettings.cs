using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel.Setting
{
    /// <summary>
    /// contains lists of settings.
    /// </summary>
    public class ListsOfSettings
    {
        public ListsOfSettings()
        {
            this.additivSettings = new List<AdditiveSetting>();
            this.allergenSettings = new List<AllergenSetting>();
        }

        public NutritionSetting nutritionSetting;
        public List<AdditiveSetting> additivSettings;
        public List<AllergenSetting> allergenSettings;
    }
}
