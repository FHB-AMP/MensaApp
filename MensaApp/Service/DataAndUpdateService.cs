using MensaApp.DataModel.Rest;
using MensaApp.DataModel.Setting;
using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MensaApp.Service
{
    class DataAndUpdateService
    {
        private ServingSettings _servingSettings;
        private ServingMealOffer _servingMealOffer;
        private ServerService _serverService;

        private readonly string _updateDateKey = "updateDate";
        private readonly int updateHourOfDay = 8;

        public DataAndUpdateService()
        {
            _servingMealOffer = new ServingMealOffer();
            _servingSettings = new ServingSettings();
            _serverService = new ServerService();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// StartUp ////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delivers true when the mensa data has been updated.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckAtStartUp()
        {
            bool isSynchronized = false;
            if (!isUpToDate())
            {
                // Start Tasks
                Task<ListOfDays> mealsTask = UpdateMeals();
                Task<ListsOfDescriptions> descriptionsTask = UpdateDescriptions();
                // Await Tasks
                await mealsTask;
                await descriptionsTask;

                isSynchronized = true;
            }
            return isSynchronized;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// MealsPage //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delivers all day with Meals for the MealsPage
        /// </summary>
        /// <param name="amountOfDays"></param>
        /// <param name="forceUpdateFromServer"></param>
        /// <returns></returns>
        public async Task<List<DayViewModel>> DeliverAllDaysWithMealsForMealsPage(int amountOfDays, bool forceUpdateFromServer)
        {
            //Start async Tasks
            Task<ListOfDays> listOfDaysTask = FindMealsFromFileOrServer(forceUpdateFromServer);
            Task<ListsOfDescriptions> listsOfDescriptionsTask = FindDescriptionsFromFileOrServer(forceUpdateFromServer);
            Task<ListsOfSettings> listsOfSettingsTask = _servingSettings.LoadListsOfSettingsFromFileAysnc();

            //Await async Tasks
            ListOfDays listOfDays = await listOfDaysTask;
            ListsOfDescriptions listsOfDescriptions = await listsOfDescriptionsTask;
            ListsOfSettings listsOfSettings = await listsOfSettingsTask;

            //Process data to view models
            ListOfSettingViewModel listOfSettingViewModel = _servingSettings.GetListOfSettingViewModels(listsOfDescriptions, listsOfSettings);
            List<DayViewModel> dayViewModels = _servingMealOffer.SearchMealOffersForCertainAmountOfDays(amountOfDays, listOfDays, listOfSettingViewModel);

            return dayViewModels;
        }

        internal ObservableCollection<DayViewModel> SearchMealsOfToday(List<DayViewModel> AllDaysWithMeals)
        {
            ObservableCollection<DayViewModel> today = new ObservableCollection<DayViewModel>();

            foreach (DayViewModel day in AllDaysWithMeals)
            {
                // Vergleiche explizit das Datum ohne Uhrzeit.
                if (day.Date.Date.CompareTo(DateTime.Today.Date) == 0)
                {
                    today.Add(day);
                    break; // there should be only one current day;
                }
            }
            return today;
        }

        internal ObservableCollection<DayViewModel> SearchMealOfForecast(List<DayViewModel> AllDaysWithMeals)
        {
            ObservableCollection<DayViewModel> resultDays = new ObservableCollection<DayViewModel>();

            foreach (DayViewModel day in AllDaysWithMeals)
            {
                // Wähle nur Tage, die in der Zukunft liegen aus.
                // Vergleiche explizit das Datum ohne Uhrzeit.
                if (day.Date.Date.CompareTo(DateTime.Today.Date) > 0)
                {
                    resultDays.Add(day);
                }
            }
            return resultDays;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SettingsPage ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal async Task<ListOfSettingViewModel> DeliverSettingsForSettingsPage()
        {
            //Start async Tasks
            Task<ListsOfDescriptions> listsOfDescriptionsTask = FindDescriptionsFromFileOrServer(false);
            Task<ListsOfSettings> listsOfSettingsTask = _servingSettings.LoadListsOfSettingsFromFileAysnc();

            //Await async Tasks
            ListsOfDescriptions listsOfDescriptions = await listsOfDescriptionsTask;
            ListsOfSettings listsOfSettings = await listsOfSettingsTask;

            ListOfSettingViewModel listOfSettingViewModel = _servingSettings.GetListOfSettingViewModels(listsOfDescriptions, listsOfSettings);
            return listOfSettingViewModel;
        }

        internal ObservableCollection<AdditiveViewModel> UpdateSettingsAdditivesBySelectedNutrition(NutritionViewModel nutritionViewModel, ObservableCollection<AdditiveViewModel> additiveViewModels)
        {
            ObservableCollection<AdditiveViewModel> updateAdditiveViewModels = new ObservableCollection<AdditiveViewModel>();
            foreach (AdditiveViewModel additiveViewModel in additiveViewModels)
            {
                updateAdditiveViewModels.Add(additiveViewModel);
            }
            additiveViewModels.Clear();

            _servingSettings.UpdateExcludingOfAdditives(nutritionViewModel, updateAdditiveViewModels);
            return updateAdditiveViewModels;
        }

        internal ObservableCollection<AllergenViewModel> UpdateSettingsAllergensBySelectedNutrition(NutritionViewModel nutritionViewModel, ObservableCollection<AllergenViewModel> allergensViewModels)
        {
            ObservableCollection<AllergenViewModel> updateAllergenViewModels = new ObservableCollection<AllergenViewModel>();
            foreach (AllergenViewModel allergensViewModel in allergensViewModels)
            {
                updateAllergenViewModels.Add(allergensViewModel);
            }
            allergensViewModels.Clear();
            _servingSettings.UpdateExcludingOfAllergens(nutritionViewModel, updateAllergenViewModels);
            return updateAllergenViewModels;
        }

        internal async Task SaveSettingsFromSettingsPage(ObservableCollection<NutritionViewModel> nutritionViewModels,
            ObservableCollection<AdditiveViewModel> additiveViewModels, ObservableCollection<AllergenViewModel> allergenViewModels)
        {
            await _servingSettings.SaveSettings(nutritionViewModels, additiveViewModels, allergenViewModels);
            return;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// LocalMethods ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async Task<ListsOfDescriptions> FindDescriptionsFromFileOrServer(bool forceUpdateFromServer)
        {
            ListsOfDescriptions resultListsOfDescriptions = new ListsOfDescriptions();
            
            ListsOfDescriptions ListsOfDescriptionsFromFile = await _servingSettings.LoadListsOfDescriptionsFromFileAysnc();
            if (ListsOfDescriptionsFromFile == null ||
                ListsOfDescriptionsFromFile.infoSymbols == null || ListsOfDescriptionsFromFile.infoSymbols.Count == 0 ||
                ListsOfDescriptionsFromFile.nutritions == null || ListsOfDescriptionsFromFile.nutritions.Count == 0 ||
                ListsOfDescriptionsFromFile.additives == null || ListsOfDescriptionsFromFile.additives.Count == 0 ||
                ListsOfDescriptionsFromFile.allergens == null || ListsOfDescriptionsFromFile.allergens.Count == 0)
            {
                // update from server if not all descriptions are available from file.
                forceUpdateFromServer = true;
            }
            else
            {
                resultListsOfDescriptions = ListsOfDescriptionsFromFile;
            }
            
            if (forceUpdateFromServer)
            {
                ListsOfDescriptions ListsOfDescriptionsFromServer = await UpdateDescriptions();
                if (ListsOfDescriptionsFromServer != null &&
                    ListsOfDescriptionsFromServer.infoSymbols != null && ListsOfDescriptionsFromServer.infoSymbols.Count > 0 &&
                    ListsOfDescriptionsFromServer.nutritions != null && ListsOfDescriptionsFromServer.nutritions.Count > 0 &&
                    ListsOfDescriptionsFromServer.additives != null && ListsOfDescriptionsFromServer.additives.Count > 0 &&
                    ListsOfDescriptionsFromServer.allergens != null && ListsOfDescriptionsFromServer.allergens.Count > 0)
                {
                    resultListsOfDescriptions = ListsOfDescriptionsFromServer;
                }
            }
            return resultListsOfDescriptions;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceUpdateFromServer"></param>
        /// <returns></returns>
        private async Task<ListOfDays> FindMealsFromFileOrServer(bool forceUpdateFromServer)
        {
            ListOfDays resultListOfDays = new ListOfDays();
            
            ListOfDays listOfDaysFromFile = await _servingSettings.LoadListOfDaysFromFileAsync();
            if (listOfDaysFromFile == null || 
                listOfDaysFromFile.days == null || 
                listOfDaysFromFile.days.Count == 0)
            {
                // update from server if no meals with days could be found from file.
                forceUpdateFromServer = true;
            } 
            else 
            {
                resultListOfDays = listOfDaysFromFile;
            }
            
            if (forceUpdateFromServer)
            {
                ListOfDays listOfDaysFromServer= await UpdateMeals();
                if (listOfDaysFromServer != null && 
                    listOfDaysFromServer.days != null && 
                    listOfDaysFromServer.days.Count > 0)
                {
                    resultListOfDays = listOfDaysFromServer;
                }
            }
            return resultListOfDays;
        }

        /// <summary>
        /// Update Meals from server and saves them to file
        /// </summary>
        /// <returns></returns>
        private async Task<ListOfDays> UpdateMeals() 
        {
            ListOfDays listOfDays = await _serverService.GetDaysWithMealsFromServerAsync();
            
            if (listOfDays != null) 
            {
                // save listOfDays into file.
                await _servingSettings.SaveMeals(listOfDays);
            }
            return listOfDays;
        }

        /// <summary>
        /// Update Descriptions from server and saves them to file.
        /// </summary>
        /// <returns></returns>
        private async Task<ListsOfDescriptions> UpdateDescriptions()
        {
            ListsOfDescriptions listsOfDescriptions = await _serverService.GetListsOfDescriptionsFromServerAsync();

            if (listsOfDescriptions != null)
            {
                // save listsOfDescriptions into file.
                await _servingSettings.SaveDescriptions(listsOfDescriptions);
            }
            return listsOfDescriptions;
        }

        /// <summary>
        /// Checks wheather the data of meals and description is up to date.
        /// </summary>
        /// <returns></returns>
        private bool isUpToDate()
        {
            bool isUpToDate = false;
            DateTime lastUpdateDate;

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(_updateDateKey))
            {
                string dateString = (string) localSettings.Values[_updateDateKey];
                if (DateTime.TryParse(dateString, out lastUpdateDate) && 
                    lastUpdateDate.Date.Equals(DateTime.Now.Date) && lastUpdateDate.Hour >= updateHourOfDay)
                {
                    isUpToDate = true;
                }
            }
            else
            {
                string dateString = DateTime.Now.ToString();
                localSettings.Values[_updateDateKey] = dateString;
            }
            return isUpToDate;
        }
    }
}
