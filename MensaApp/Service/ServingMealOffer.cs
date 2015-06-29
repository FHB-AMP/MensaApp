using MensaApp.DataModel;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace MensaApp.Service
{

    class ServingMealOffer
    {
        // Abspeichern und Lesen des JSON-Files
        private StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        private SerializeSettings _serializeSettings;

        private string _symbolInfosFilename;
        private string _additivesFilename;
        private string _allergensFilename;

        public ServingMealOffer()
        {
            _serializeSettings = new SerializeSettings();

            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            _symbolInfosFilename = MensaRestApiResource.GetString("SettingsSymbolInfoJSONFile");
            _additivesFilename = MensaRestApiResource.GetString("SettingsAdditivesJSONFile");
            _allergensFilename = MensaRestApiResource.GetString("SettingsAllergenesJSONFile");
        }

        /// <summary>
        /// Stelle die Mahlzeit-Daten für eine bestimmte Anzahl an Tagen bereit.
        /// </summary>
        /// <param name="forecast">Anzahl der Tage einschließlich des aktuellen</param>
        /// <returns>Liste aus DayViewModellen, fuer jeden Tag ein DayViewModel, es sei denn es ist Samstag oder Sonntag</returns>
        public async Task<List<DayViewModel>> FindMealOffersForCertainAmountOfDays(int amountOfDays)
        {
            List<DayViewModel> resultDays = new List<DayViewModel>();

            string data = await ReadSavedJSON();
            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectDays>(data);

            NutritionViewModel nutrition = new NutritionViewModel();
            // Alle InfoSymbole aus den Settings 
            ObservableCollection<InfoSymbolViewModel> deserializedInfoSymbols = new ObservableCollection<InfoSymbolViewModel>();
            // Alle Zusatzstoffe aus den Settings
            ObservableCollection<AdditiveViewModel> deserializedAdditives = await _serializeSettings.deserializeAdditives(_additivesFilename);
            // Alle Allergene aus den Settings
            ObservableCollection<AllergenViewModel> deserializedAllergens = await _serializeSettings.deserializeAllergenes(_allergensFilename);

            for (int i = 0; i < amountOfDays; i++)
            {
                // setze das datum für den gesuchten Tag.
                DateTime requiredDate = DateTime.Today.AddDays(i);

                DayViewModel resultDay = new DayViewModel();
                resultDay.Date = requiredDate;
                // Mahlzeit (Objekt) des gewuenschten Tages finden
                foreach (Day day in rootObject.days)
                {
                    // Parse von String zu DateTime
                    DateTime DateOfMealDay = parseDateTimeFromJsonString(day.date);
                    // finde den gewuenschten Tag
                    if (DateOfMealDay.CompareTo(requiredDate) == 0)
                    {
                        foreach (Meal meal in day.meals)
                        {
                            List<string> symbolIds = meal.symbols;
                            List<string> additivesIds = meal.additives;
                            List<string> allergenIds = meal.allergens;

                            ObservableCollection<InfoSymbolViewModel> resultInfoSymbols = MatchInfoSymbolIdsWithInfoSymbolsFromSettings(symbolIds, deserializedInfoSymbols);
                            ObservableCollection<AdditiveViewModel> resultAdditives = MatchAdditiveIdsWithAdditivesFromSettings(additivesIds, deserializedAdditives);
                            ObservableCollection<AllergenViewModel> resultAllergens = MatchAllergenIdsWithAllergensFromSettings(allergenIds, deserializedAllergens);

                            bool suitableNutrition = EvaluateIsSuitableNutrition(nutrition, resultInfoSymbols, resultAdditives, resultAllergens);
                            bool suitableAdditives = EvaluateIsSuitableAdditives(resultAdditives);
                            bool suitableAllergens = EvaluateIsSuitableAllergens(resultAllergens);
                            bool suitableMeal = EvaluateSuitableMeal(suitableNutrition, suitableAdditives, suitableAllergens);

                            resultDay.Meals.Add(new MealViewModel(meal.mealNumber, meal.name, resultInfoSymbols, resultAdditives, resultAllergens, suitableMeal, suitableNutrition, suitableAdditives, suitableAllergens));
                        }
                        resultDays.Add(resultDay);
                        // Der gewünschte Tag wurde gefunden. Darum muss an dieser Stelle nicht weiter danach gesucht werden. -> break; (Schmidt will hate me. xD)
                        break;
                    }
                }
            }
            return resultDays;
        }

        /// <summary>
        /// evaluates whether the meal suitable at all.
        /// </summary>
        /// <param name="suitableNutrition"></param>
        /// <param name="suitableAdditives"></param>
        /// <param name="suitableAllergens"></param>
        /// <returns></returns>
        private bool EvaluateSuitableMeal(bool suitableNutrition, bool suitableAdditives, bool suitableAllergens)
        {
            if (!suitableNutrition || !suitableAdditives || !suitableAllergens)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// evaluates whether the meal is suitable to the nutrition from the settings.
        /// </summary>
        /// <param name="nutrition"></param>
        /// <param name="resultInfoSymbols"></param>
        /// <param name="resultAdditives"></param>
        /// <param name="resultAllergens"></param>
        /// <returns></returns>
        private bool EvaluateIsSuitableNutrition(NutritionViewModel nutrition, ObservableCollection<InfoSymbolViewModel> resultInfoSymbols, 
            ObservableCollection<AdditiveViewModel> resultAdditives, ObservableCollection<AllergenViewModel> resultAllergens)
        {
            //TODO Implementation
            return true;
        }

        /// <summary>
        /// evaluate whether the collection of allergens contains any excluded.
        /// </summary>
        /// <param name="resultAllergens"></param>
        /// <returns></returns>
        private static bool EvaluateIsSuitableAllergens(ObservableCollection<AllergenViewModel> resultAllergens)
        {
            foreach (AllergenViewModel allergen in resultAllergens)
            {
                if (allergen.IsExcluded)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// evaluate whether the collection of additives contains any excluded.
        /// </summary>
        /// <param name="resultAdditives"></param>
        /// <returns></returns>
        private static bool EvaluateIsSuitableAdditives(ObservableCollection<AdditiveViewModel> resultAdditives)
        {
            foreach (AdditiveViewModel additives in resultAdditives)
            {
                if (additives.IsExcluded)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Matchs the allergen ids of the meal with the deserialized allergens form the settings.
        /// </summary>
        /// <param name="allergenIds"></param>
        /// <param name="deserializedAllergens"></param>
        /// <returns></returns>
        private static ObservableCollection<AllergenViewModel> MatchAllergenIdsWithAllergensFromSettings(List<string> allergenIds, ObservableCollection<AllergenViewModel> deserializedAllergens)
        {
            ObservableCollection<AllergenViewModel> allergensResult = new ObservableCollection<AllergenViewModel>();
            foreach (string allergenId in allergenIds)
            {
                foreach (AllergenViewModel deserializedAllergen in deserializedAllergens)
                {
                    if (allergenId.Equals(deserializedAllergen.Id))
                    {
                        allergensResult.Add(deserializedAllergen);
                    }
                }
            }
            return allergensResult;
        }

        /// <summary>
        /// Matchs the additive ids of the meal with the deserialized additives form the settings.
        /// </summary>
        /// <param name="additiveIds"></param>
        /// <param name="deserializedAdditives"></param>
        /// <returns></returns>
        private static ObservableCollection<AdditiveViewModel> MatchAdditiveIdsWithAdditivesFromSettings(List<string> additiveIds, ObservableCollection<AdditiveViewModel> deserializedAdditives)
        {
            ObservableCollection<AdditiveViewModel> additivesResult = new ObservableCollection<AdditiveViewModel>();

            foreach (string additiveId in additiveIds)
            {
                foreach (AdditiveViewModel deserializedAdditive in deserializedAdditives)
                {
                    if (additiveId.Equals(deserializedAdditive.Id))
                    {
                        additivesResult.Add(deserializedAdditive);
                        break;
                    }
                }
            }
            return additivesResult;
        }

        /// <summary>
        /// Matchs the InfoSymbol ids of the meal with the deserialized InfoSymbols form the settings.
        /// </summary>
        /// <param name="symbolIds"></param>
        /// <param name="deserializedInfoSymbols"></param>
        /// <returns></returns>
        private static ObservableCollection<InfoSymbolViewModel> MatchInfoSymbolIdsWithInfoSymbolsFromSettings(List<string> symbolIds, ObservableCollection<InfoSymbolViewModel> deserializedInfoSymbols)
        {
            ObservableCollection<InfoSymbolViewModel> resultInfoSymbols = new ObservableCollection<InfoSymbolViewModel>();

            foreach (string symbolId in symbolIds)
            {
                // TODO: Diese Zeile löschen, wenn symbole gespeichert wurden.
                resultInfoSymbols.Add(new InfoSymbolViewModel(symbolId, symbolId));

                foreach (InfoSymbolViewModel deserializedSymbolInfo in deserializedInfoSymbols)
                {
                    if (symbolId.Equals(deserializedSymbolInfo.Id))
                    {
                        resultInfoSymbols.Add(deserializedSymbolInfo);
                        break;
                    }
                }
            }
            return resultInfoSymbols;
        }

        /// <summary>
        /// Hole die JSON-Daten vom Server ab.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetServerData(string serviceURI, string serviceURL, string propertyName)
        {
            string data = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serviceURI);
                    string url = serviceURL;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        // Hole JSON-File
                        data = await response.Content.ReadAsStringAsync();

                        // Hole den Speicherort der JSON Datei
                        ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
                        String dateiName = MensaRestApiResource.GetString(propertyName);

                        // Write data to a file
                        StorageFile sampleFile = await _localFolder.CreateFileAsync(dateiName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteTextAsync(sampleFile, data);

                    }
                }
            }
            catch (Exception)
            {
                //TODO Holger Fehlerbehandlung einbauen
            }

            return data;

        }

        /// <summary>
        /// Lese das abgespeicherte JSON
        /// </summary>
        /// <returns>JSON als Zeichenkette</returns>
        private async Task<string> ReadSavedJSON()
        {
            // Helfer
            string data = "";

            try
            {
                // Hole den Standort der JSON Datei
                ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
                String dateiName = MensaRestApiResource.GetString("MealJSONFile");

                StorageFile sampleFile = await _localFolder.GetFileAsync(dateiName);

                // Abgleich mit dem heutigen Datum TODO Holger
                //DateTimeOffset fileCreationDateOff = sampleFile.DateCreated;
                //DateTime fileCreationDate = fileCreationDateOff.Date;

                data = await FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }

            return data;
        }

        /// <summary>
        /// Das aus der JSON-Datei eingelesene Datumsformat in DateTime umwandeln.
        /// </summary>
        /// <param name="dateString">Datum eines Tages durch Bindestriche "-" getrennt</param>
        /// <returns>DateTime des uebergebenen Strings</returns>
        private DateTime parseDateTimeFromJsonString(string dateString)
        {
            // Trenner uebergeben
            string[] separators = { "-" };

            // String dateParts aus JSON-Objekt anhand der Trenner aufteilen und die Leerzeichen entfernen, wenn vorhanden
            string[] dateParts = dateString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Neues DateTime-Objekt erzeugen und die einzelnen Teile in in Integer parsen
            DateTime result = new DateTime(Int32.Parse(dateParts[0]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[2]));

            return result;
        }

        internal ObservableCollection<DayViewModel> SearchMealsOfToday(DateTime currentDate, List<DayViewModel> AllDaysWithMeals)
        {
            ObservableCollection<DayViewModel> today = new ObservableCollection<DayViewModel>();
 
            foreach (DayViewModel day in AllDaysWithMeals)
            {
                // Vergleiche explizit das Datum ohne Uhrzeit.
                if (day.Date.Date.CompareTo(currentDate.Date) == 0)
                {
                    today.Add(day);
                    break; // there should be only one current day;
                }
            }
            return today;
        }

        internal ObservableCollection<DayViewModel> SearchMealOfForecast(DateTime currentDate, List<DayViewModel> AllDaysWithMeals)
        {
            ObservableCollection<DayViewModel> resultDays = new ObservableCollection<DayViewModel>(); 

            foreach (DayViewModel day in AllDaysWithMeals)
            {
                // Wähle nur Tage, die in der Zukunft liegen aus.
                // Vergleiche explizit das Datum ohne Uhrzeit.
                if (day.Date.Date.CompareTo(currentDate.Date) > 0)
                {
                    resultDays.Add(day);
                }
            }
            return resultDays;
        }
    }
}