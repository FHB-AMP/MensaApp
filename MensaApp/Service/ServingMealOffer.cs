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
        
        public ServingMealOffer()
        {
            _servingSettings = new ServingSettings();
        }

        /// <summary>
        /// Stelle die Mahlzeit-Daten für eine bestimmte Anzahl an Tagen bereit.
        /// </summary>
        /// <param name="forecast">Anzahl der Tage einschließlich des aktuellen</param>
        /// <returns>Liste aus DayViewModellen, fuer jeden Tag ein DayViewModel, es sei denn es ist Samstag oder Sonntag</returns>
        public List<DayViewModel> SearchMealOffersForCertainAmountOfDays(int requiredAmountOfDays, ListOfDays listOfDays, ListOfSettingViewModel listOfSettingViewModel)
        {
            List<DayViewModel> resultDays = new List<DayViewModel>();

            ObservableCollection<NutritionViewModel> deserializedNutritions = listOfSettingViewModel.NutritionViewModels;
            NutritionViewModel deserializedSelectedNutrition = FindSelectedNutritionOrReturnFirst(deserializedNutritions);
            ObservableCollection<InfoSymbolViewModel> deserializedInfoSymbolSettings = listOfSettingViewModel.InfoSymbolViewModels;
            ObservableCollection<AdditiveViewModel> deserializedAdditiveSettings = listOfSettingViewModel.AdditiveViewModels;
            ObservableCollection<AllergenViewModel> deserializedAllergenSettings = listOfSettingViewModel.AllergenViewModels;

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
                if (listOfDays != null && listOfDays.days != null)
                {
                    foreach (Day day in listOfDays.days)
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
    }
}