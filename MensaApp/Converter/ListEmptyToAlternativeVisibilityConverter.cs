using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    /// <summary>
    /// Shows the alternative content if the given list is empty
    /// </summary>
    class ListEmptyToAlternativeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int? itemCount = value as int?;
            if (itemCount.HasValue)
            {
                return itemCount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
