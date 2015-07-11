using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MensaApp.Selector
{
    class SettingsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SettingAdditiveEnabledTemplate { get; set; }
        public DataTemplate SettingAdditiveDisabledTemplate { get; set; }
        public DataTemplate SettingAllergenEnabledTemplate { get; set; }
        public DataTemplate SettingAllergenDisabledTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {

            if (item.GetType() == typeof(AdditiveViewModel))
            {
                AdditiveViewModel additive = item as AdditiveViewModel;
                if (additive.IsDisabled)
                {
                    return SettingAdditiveDisabledTemplate;
                }
                else
                {
                    return SettingAdditiveEnabledTemplate;
                }
            }
            else if (item.GetType() == typeof(AllergenViewModel))
            {
                AllergenViewModel allergen = item as AllergenViewModel;
                if (allergen.IsDisabled)
                {
                    return SettingAllergenDisabledTemplate;
                }
                else
                {
                    return SettingAllergenEnabledTemplate;
                }
            }
            return base.SelectTemplateCore(item);
        }
    }
}
