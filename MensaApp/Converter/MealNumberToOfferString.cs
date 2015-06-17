using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class MealNumberToOfferString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int mealNumber = (int)value;
            return String.Format("Angebot {0:0}", mealNumber);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
