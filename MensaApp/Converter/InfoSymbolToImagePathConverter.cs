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
    class InfoSymbolToImagePathConverter : IValueConverter
    {
        private List<string> _wellKnownInfoSymbols;

        public InfoSymbolToImagePathConverter()
        {
            string[] infoSymbolsWithImages = { "mensaVital", "mit Fisch", "mit Geflügelfleisch", "mit Lamm", "mit Rindfleisch", "mit Schweinefleisch", "ovo-lacto-vegetabil", "vegan" };
            _wellKnownInfoSymbols = new List<string>();
            _wellKnownInfoSymbols.AddRange(infoSymbolsWithImages);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            ObservableCollection<InfoSymbolViewModel> infoSymbols = value as ObservableCollection<InfoSymbolViewModel>;
            string defaultImagePath = "Assets/Ingredients/unkown-240.png";

            if (infoSymbols != null) 
            {
                foreach (InfoSymbolViewModel infoSymbol in infoSymbols)
                {
                    if (_wellKnownInfoSymbols.Contains(infoSymbol.Id))
                    {
                        return "Assets/Ingredients/" + infoSymbol.Id + "-240.png";
                    }
                }
            }
            return defaultImagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
