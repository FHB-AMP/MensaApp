using MensaApp.DataModel;
using MensaApp.DataModel.Rest;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private ServingSettings _servingSettings;

        private string _symbolInfosFilename;
        private string _additivesFilename;
        private string _allergensFilename;

        public ServingMealOffer()
        {
            _servingSettings = new ServingSettings();

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
        public async Task<List<DayViewModel>> FindMealOffersForCertainAmountOfDaysAsync(int requiredAmountOfDays)
        {
            List<DayViewModel> resultDays = new List<DayViewModel>();
            ListOfDays rootObject = await _servingSettings.GetListOfDays();

            // TODO uses new
            ListOfSettingViewModel listOfSettingViewModels = await _servingSettings.GetListOfSettingViewModelsAsync();
            ObservableCollection<NutritionViewModel> deserializedNutritions = listOfSettingViewModels.NutritionViewModels;
            NutritionViewModel deserializedSelectedNutrition = FindSelectedNutritionOrReturnFirst(deserializedNutritions);
            ObservableCollection<InfoSymbolViewModel> deserializedInfoSymbolSettings = listOfSettingViewModels.InfoSymbolViewModels;
            ObservableCollection<AdditiveViewModel> deserializedAdditiveSettings = listOfSettingViewModels.AdditiveViewModels;
            ObservableCollection<AllergenViewModel> deserializedAllergenSettings = listOfSettingViewModels.AllergenViewModels;

            int dayIterator = 0;
            int foundDaysCounter = 0;
            // DayIterator darf nicht doppelt so groß werden wie die erforderliche Anzahl der gesuchten Tage. 
            // Soll Deadlock verhindern.
            while (foundDaysCounter < requiredAmountOfDays &&
                dayIterator < requiredAmountOfDays * 2)
            {
                // setze das Datum für den gesuchten Tag.
                // i Iterator startet mit 0, um den heutigen Tag in die Suche einzuschließen.
                DateTime requiredDate = DateTime.Today.AddDays(dayIterator++);

                DayViewModel resultDay = new DayViewModel();
                resultDay.Date = requiredDate;
                // Mahlzeit (Objekt) des gewuenschten Tages finden
                if (rootObject != null && rootObject.days != null)
                {
                    foreach (Day day in rootObject.days)
                    {
                        // Parse von String zu DateTime
                        DateTime DateOfMealDay = parseDateTimeFromJsonString(day.date);
                        // finde den gewuenschten Tag
                        if (DateOfMealDay.CompareTo(requiredDate) == 0)
                        {
                            if (day.meals != null)
                            {
                                foreach (Meal meal in day.meals)
                                {
                                    List<string> mealInfoSymbolIds = meal.symbols;
                                    List<string> mealAdditiveIds = meal.additives;
                                    List<string> mealAllergenIds = meal.allergens;

                                    ObservableCollection<InfoSymbolViewModel> resultInfoSymbols = MatchInfoSymbolIdsWithInfoSymbolsFromSettings(mealInfoSymbolIds, deserializedInfoSymbolSettings);
                                    ObservableCollection<AdditiveViewModel> resultAdditives = MatchAdditiveIdsWithAdditivesFromSettings(mealAdditiveIds, deserializedAdditiveSettings);
                                    ObservableCollection<AllergenViewModel> resultAllergens = MatchAllergenIdsWithAllergensFromSettings(mealAllergenIds, deserializedAllergenSettings);

                                    bool suitableNutrition = EvaluateIsSuitableNutrition(deserializedSelectedNutrition, mealInfoSymbolIds, mealAdditiveIds, mealAllergenIds);
                                    bool suitableAdditives = EvaluateIsSuitableAdditives(resultAdditives);
                                    bool suitableAllergens = EvaluateIsSuitableAllergens(resultAllergens);
                                    bool suitableMeal = EvaluateSuitableMeal(suitableNutrition, suitableAdditives, suitableAllergens);

                                    resultDay.Meals.Add(new MealViewModel(meal.mealNumber, meal.name, resultInfoSymbols, resultAdditives, resultAllergens, suitableMeal, suitableNutrition, suitableAdditives, suitableAllergens));
                                }
                            }
                            resultDays.Add(resultDay);
                            // für jeden gefundenen Tag wird i erhöht.
                            foundDaysCounter++;
                            // Der gewünschte Tag wurde gefunden. Darum muss an dieser Stelle nicht weiter danach gesucht werden. -> break; (Schmidt will hate me. xD)
                            break;
                        }
                    }
                }
            }
            return resultDays;
        }

        private NutritionViewModel FindSelectedNutritionOrReturnFirst(ObservableCollection<NutritionViewModel> deserializedNutritions)
        {
            NutritionViewModel resultNutritionViewModel = new NutritionViewModel();

            if (deserializedNutritions != null)
            {
                bool isSelectedNutritionFound = false;
                foreach (NutritionViewModel nutritionViewModel in deserializedNutritions)
                {
                    if (nutritionViewModel.IsSelectedNutrition)
                    {
                        resultNutritionViewModel = nutritionViewModel;
                        isSelectedNutritionFound = true;
                    }
                }
                if (!isSelectedNutritionFound && deserializedNutritions.Count > 0)
                {
                    // If no Nutrition is selected and deserializedNutritions has items get the first of them. (normally should by 'Normal')
                    var nutritionViewModelIterator = deserializedNutritions.GetEnumerator();
                    nutritionViewModelIterator.MoveNext();
                    resultNutritionViewModel = nutritionViewModelIterator.Current;
                    nutritionViewModelIterator.Dispose();
                }
            }
            return resultNutritionViewModel;
        }


        /// <summary>
        /// stub which should be deleted in the final version.
        /// </summary>
        /// <param name="nutritionId"></param>
        /// <returns></returns>
        private NutritionViewModel getTestNutritionFromStub(string nutritionId)
        {
            // TODO delete this method when its not necassery any more.
            NutritionViewModel resultNutrition = new NutritionViewModel();
            if (nutritionId != null)
            {
                switch (nutritionId)
                {
                    case "(NOR)":
                        // Add normal nutrition
                        string normalDefinition = "Keine Einschränkungen.";
                        resultNutrition = new NutritionViewModel("(NOR)", "Normal", normalDefinition, new ObservableCollection<InfoSymbolViewModel>(), new ObservableCollection<AdditiveViewModel>(), new ObservableCollection<AllergenViewModel>(), true);
                        break;
                    case "(OVO)" :
                        // Add Vegetarian nutrition
                        ObservableCollection<InfoSymbolViewModel> infoSymbolsVegi = new ObservableCollection<InfoSymbolViewModel>();
                        ObservableCollection<AllergenViewModel> excludedAllergensVegi = new ObservableCollection<AllergenViewModel>();
                        ObservableCollection<AdditiveViewModel> excludedAdditivesVegi = new ObservableCollection<AdditiveViewModel>();
                        infoSymbolsVegi.Add(new InfoSymbolViewModel("mit Schweinefleisch", "Schweinefleisch"));
                        infoSymbolsVegi.Add(new InfoSymbolViewModel("mit Rindfleisch", "Rindfleisch"));
                        infoSymbolsVegi.Add(new InfoSymbolViewModel("mit Lamm", "Lamm"));
                        infoSymbolsVegi.Add(new InfoSymbolViewModel("mit Fisch", "Fisch"));
                        infoSymbolsVegi.Add(new InfoSymbolViewModel("mit Geflügelfleisch", "Geflügelfleisch"));
                        excludedAdditivesVegi.Add(new AdditiveViewModel("(GE)", "mit Gelatine", "", false));
                        excludedAllergensVegi.Add(new AllergenViewModel("(N)", "Weichtiere sind Schnecken, Muscheln, Austern und Tintenfische", "Fisch- und Feinkostsalate, Paella und Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
                        excludedAllergensVegi.Add(new AllergenViewModel("(D)", "Fisch", "Paella, Bouillabaise, Worchester Sauce, asiatische Würzpasten", false));
                        excludedAllergensVegi.Add(new AllergenViewModel("(B)", "Krebstiere sind Garnelen, Hummer, Fluss-und Taschenkrebse, Krabben", "Feinkostsalate, Paella, Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
                        string veggieDefinition = "Ovo-Lacto-Vegetarier essen nichts vom toten Tier.";
                        resultNutrition = new NutritionViewModel("(OVO)", "Ovo-Lacto-Vegetarisch", veggieDefinition, infoSymbolsVegi, excludedAdditivesVegi, excludedAllergensVegi);
                        break;
                    case "(VEG)" :
                        // Add Vegan nutrion
                        ObservableCollection<InfoSymbolViewModel> infoSymbolsVega = new ObservableCollection<InfoSymbolViewModel>();
                        ObservableCollection<AllergenViewModel> excludedAllergensVega = new ObservableCollection<AllergenViewModel>();
                        ObservableCollection<AdditiveViewModel> excludedAdditivesVega = new ObservableCollection<AdditiveViewModel>();
                        infoSymbolsVega.Add(new InfoSymbolViewModel("mit Schweinefleisch", "Schweinefleisch"));
                        infoSymbolsVega.Add(new InfoSymbolViewModel("mit Rindfleisch", "Rindfleisch"));
                        infoSymbolsVega.Add(new InfoSymbolViewModel("mit Lamm", "Lamm"));
                        infoSymbolsVega.Add(new InfoSymbolViewModel("mit Fisch", "Fisch"));
                        infoSymbolsVega.Add(new InfoSymbolViewModel("mit Geflügelfleisch", "Geflügelfleisch"));

                        excludedAdditivesVega.Add(new AdditiveViewModel("(GE)", "mit Gelatine", "", false));
                        excludedAdditivesVega.Add(new AdditiveViewModel("(13)", "mit Milcheiweiß", "", false));
                        excludedAdditivesVega.Add(new AdditiveViewModel("(14)", "mit Eiklar", "Einsatz von Fremdeiweiß, wird als Bindemittel verwendet.", false));
                        excludedAdditivesVega.Add(new AdditiveViewModel("(22)", "mit Milchpulver", "", false));
                        excludedAdditivesVega.Add(new AdditiveViewModel("(23)", "mit Molkenpulver", "", false));
                        excludedAdditivesVega.Add(new AdditiveViewModel("(TL)", "enthält tierisches Lab", "", false));
                        excludedAllergensVega.Add(new AllergenViewModel("(N)", "Weichtiere sind Schnecken, Muscheln, Austern und Tintenfische", "Fisch- und Feinkostsalate, Paella und Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
                        excludedAllergensVega.Add(new AllergenViewModel("(D)", "Fisch", "Paella, Bouillabaise, Worchester Sauce, asiatische Würzpasten", false));
                        excludedAllergensVega.Add(new AllergenViewModel("(B)", "Krebstiere sind Garnelen, Hummer, Fluss-und Taschenkrebse, Krabben", "Feinkostsalate, Paella, Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
                        excludedAllergensVega.Add(new AllergenViewModel("(C)", "Eier", "Mayonnaisen, Remouladen, Teigwaren (Tortellini, Spätzle, Schupfnudeln), Gnocchi, Backwaren, Panaden, geklärte und gebundene Suppen", false));
                        excludedAllergensVega.Add(new AllergenViewModel("(G)", "Milch", "Backwaren, vegetarische Bratlinge, Wurstwaren, Dressings und Würzsaucen", false));
                        string veganDefinition = "Veganer essen gar keine tierischen Produkte.";
                        resultNutrition = new NutritionViewModel("(VEG)", "Vegan", veganDefinition, infoSymbolsVega, excludedAdditivesVega, excludedAllergensVega);
                        break;
                    default:
                        goto case "(NOR)";
                }
            }
            return resultNutrition;
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
        /// <param name="selectedNutrition"></param>
        /// <param name="resultInfoSymbols"></param>
        /// <param name="resultAdditives"></param>
        /// <param name="resultAllergens"></param>
        /// <returns></returns>
        private bool EvaluateIsSuitableNutrition(NutritionViewModel selectedNutrition, List<string> mealInfoSymbolIds,
            List<string> mealAdditiveIds, List<string> mealAllergenIds)
        {
            if (selectedNutrition != null)
            {
                if (selectedNutrition.ExcludedSymbols != null)
                {
                    foreach (InfoSymbolViewModel infoSymbolViewModel in selectedNutrition.ExcludedSymbols)
                    {
                        if (mealInfoSymbolIds.Contains(infoSymbolViewModel.Id))
                            return false;
                    }
                }


                if (selectedNutrition.ExcludedAdditives != null)
                {
                    foreach (AdditiveViewModel additiveViewModel in selectedNutrition.ExcludedAdditives)
                    {
                        if (mealAdditiveIds.Contains(additiveViewModel.Id))
                            return false;
                    }
                }


                if (selectedNutrition.ExcludedAllergens != null)
                {
                    foreach (AllergenViewModel allergenViewModel in selectedNutrition.ExcludedAllergens)
                    {
                        if (mealAllergenIds.Contains(allergenViewModel.Id))
                            return false;
                    }
                }
            }
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
                if (allergen.IsExcluded || allergen.IsDisabled)
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
                if (additives.IsExcluded || additives.IsDisabled)
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

            if (allergenIds != null && deserializedAllergens != null)
            {
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

            if (additiveIds != null && deserializedAdditives != null)
            {
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

            if (symbolIds != null && deserializedInfoSymbols != null)
            {
                foreach (string symbolId in symbolIds)
                {
                    foreach (InfoSymbolViewModel deserializedSymbolInfo in deserializedInfoSymbols)
                    {
                        if (symbolId.Equals(deserializedSymbolInfo.Id))
                        {
                            resultInfoSymbols.Add(deserializedSymbolInfo);
                            break;
                        }
                    }
                }
            }
            return resultInfoSymbols;
        }

        /// <summary>
        /// Hole die JSON-Daten vom Server ab.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetServerData(string serviceURI, string serviceURL)
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
                    }
                }
            }
            catch (ArgumentNullException)
            {
                Debug.WriteLine("[MensaApp.ServingMealOffer] HTML Get Request Failure");
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