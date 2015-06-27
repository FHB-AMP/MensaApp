using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class SuitableAllergensToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isSuitable = (bool)value;

            if (isSuitable)
            {
                return "assets/icon/allergens-240.png";
            }
            else
            {
                return "assets/icon/allergens-240.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
