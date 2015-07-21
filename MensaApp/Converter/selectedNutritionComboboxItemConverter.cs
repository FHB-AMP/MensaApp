using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class SelectedNutritionComboboxItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ObservableCollection<NutritionViewModel> nutritionViewModels = value as ObservableCollection<NutritionViewModel>;
            NutritionViewModel resultNutritionViewModel = null;

            if (nutritionViewModels != null && nutritionViewModels.Count > 0)
            {
                resultNutritionViewModel = nutritionViewModels.ElementAt(0);
                // Setzen der ausgewaehlten Ernaehrungsweise im ComboBox-Menue
                foreach (NutritionViewModel nutritionViewModel in nutritionViewModels)
                {
                    if (nutritionViewModel.IsSelectedNutrition)
                    {
                        resultNutritionViewModel = nutritionViewModel;
                    }
                }
            }

            return resultNutritionViewModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
