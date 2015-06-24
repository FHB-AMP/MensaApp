using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class IngredientsToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            string selectedIngridient = "mit Schweinefleisch";
            return "Assets/Ingredients/" + selectedIngridient + "-240.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
