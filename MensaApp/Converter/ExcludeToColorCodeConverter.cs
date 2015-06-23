using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class ExcludeToColorCode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isExcluded = (bool)value;

            if (isExcluded)
            {
                //red color
                return "#ffed5959";
            }
            else
            {
                //none color
                return "#00000000";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
