using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MensaApp.Converter
{
    class IngredientsToImagePathConverter : IValueConverter
    {
        private List<string> _wellKnownIngredients;

        public IngredientsToImagePathConverter()
        {
            string[] ingredients = { "mensaVital", "mit Fisch", "mit Geflügelfleisch", "mit Lamm", "mit Rindfleisch", "mit Schweinefleisch", "ovo-lacto-vegetabil", "vegan" };
            _wellKnownIngredients = new List<string>();
            _wellKnownIngredients.AddRange(ingredients);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            ObservableCollection<string> ingredients = value as ObservableCollection<string>;
            string defaultImagePath = "Assets/Ingredients/unkown-240.png";

            if (ingredients != null) 
            {
                foreach (string ingredient in ingredients)
                {
                    if (_wellKnownIngredients.Contains(ingredient))
                    {
                        return "Assets/Ingredients/" + ingredient + "-240.png";
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
