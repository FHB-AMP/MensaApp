using MensaApp.DataModel;
using MensaApp.DataModel.Rest;
using MensaApp.DataModel.Setting;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.Service
{
    class ServingSettings
    {
        private SettingsMapping _settingsMapping;
        private FileService _fileService;

        public ServingSettings()
        {
            _settingsMapping = new SettingsMapping();
            _fileService = new FileService();
        }

        /// <summary>
        /// Combines all descriptions and all settings to a list of settingViewModels
        /// </summary>
        public ListOfSettingViewModel GetListOfSettingViewModels(ListsOfDescriptions listsOfDescriptions, ListsOfSettings listOfSettings)
        {
            ListOfSettingViewModel resultListOfSettingViewModel = new ListOfSettingViewModel();

            // Phase 1
            // Combines descriptions and settings to viewModels
            resultListOfSettingViewModel.AdditiveViewModels = _settingsMapping.MapToAdditiveViewModels(listsOfDescriptions.additives, listOfSettings.additivSettings);
            resultListOfSettingViewModel.AllergenViewModels = _settingsMapping.MapToAllergenViewModels(listsOfDescriptions.allergens, listOfSettings.allergenSettings);
            resultListOfSettingViewModel.InfoSymbolViewModels = _settingsMapping.MapToInfoSymbolViewModels(listsOfDescriptions.infoSymbols);

            // Phase 2
            // Combine complex nutritionViewModels
            resultListOfSettingViewModel.NutritionViewModels = _settingsMapping.MapToNutritionViewModels(listsOfDescriptions.nutritions,
                listOfSettings.nutritionSetting, resultListOfSettingViewModel.InfoSymbolViewModels, resultListOfSettingViewModel.AdditiveViewModels, resultListOfSettingViewModel.AllergenViewModels);

            // Phase 3
            // mark additives, allergens and infoSymbols as excluded by selected nutrition.
            NutritionViewModel selectedNutritionViewModel = _settingsMapping.GetSelectedNutritionViewModel(resultListOfSettingViewModel.NutritionViewModels);
            _settingsMapping.excludeAdditiveViewModelsByNutrition(selectedNutritionViewModel, resultListOfSettingViewModel.AdditiveViewModels);
            _settingsMapping.excludeAllergenViewModelsByNutrition(selectedNutritionViewModel, resultListOfSettingViewModel.AllergenViewModels);
            _settingsMapping.excludeInfoSymbolViewModelsByNutrition(selectedNutritionViewModel, resultListOfSettingViewModel.InfoSymbolViewModels);

            return resultListOfSettingViewModel;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveSettings ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Saves Settings to file
        /// </summary>
        /// <param name="listOfSettingsViewModel"></param>
        public void SaveSettings(ListOfSettingViewModel listOfSettingsViewModel)
        {
            if (listOfSettingsViewModel != null)
                SaveSettings(listOfSettingsViewModel.NutritionViewModels, listOfSettingsViewModel.AdditiveViewModels, listOfSettingsViewModel.AllergenViewModels);
        }

        /// <summary>
        /// Saves Settings to file
        /// </summary>
        /// <param name="nutritionViewModels"></param>
        /// <param name="additiveViewModels"></param>
        /// <param name="allergenViewModels"></param>
        public void SaveSettings(ObservableCollection<NutritionViewModel> nutritionViewModels, 
            ObservableCollection<AdditiveViewModel> additiveViewModels, ObservableCollection<AllergenViewModel> allergenViewModels)
        {
            ListsOfSettings listsOfSettings = new ListsOfSettings();
            listsOfSettings.nutritionSetting = _settingsMapping.mapToNutritionSetting(nutritionViewModels);
            listsOfSettings.additivSettings = _settingsMapping.mapToAdditiveSettings(additiveViewModels);
            listsOfSettings.allergenSettings = _settingsMapping.mapToAllergenSettings(allergenViewModels);
            SaveSettings(listsOfSettings);
        }

        /// <summary>
        /// Save Settings to file
        /// </summary>
        /// <param name="listsOfSettings"></param>
        private void SaveSettings(ListsOfSettings listsOfSettings)
        {
            _fileService.SaveListOfSettings(listsOfSettings);
        }

        internal Task<ListsOfSettings> LoadListsOfSettingsFromFileAysnc()
        {
            return _fileService.LoadListOfSettingsAsync();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveDecriptions ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Saves descriptions to file
        /// </summary>
        /// <param name="nutritionViewModels"></param>
        /// <param name="additiveViewModels"></param>
        /// <param name="allergenViewModels"></param>
        public void SaveDescriptions(List<NutritionDescription> nutritionDescriptions,
            List<AdditiveDescription> additiveDescriptions, List<AllergenDescription> allergenDescriptions, List<InfoSymbolDescription> infoSymbolDescriptions)
        {
            ListsOfDescriptions listsOfDescriptions = new ListsOfDescriptions();
            listsOfDescriptions.nutritions = nutritionDescriptions;
            listsOfDescriptions.additives = additiveDescriptions;
            listsOfDescriptions.allergens = allergenDescriptions;
            listsOfDescriptions.infoSymbols = infoSymbolDescriptions;
            SaveDescriptions(listsOfDescriptions);
        }

        /// <summary>
        /// Save descriptions to file
        /// </summary>
        /// <param name="listsOfSettings"></param>
        public void SaveDescriptions(ListsOfDescriptions listsOfDescriptions)
        {
            _fileService.SaveListOfDescriptions(listsOfDescriptions);
        }

        internal void SaveDescriptions(string descriptionJSONStringFromServer)
        {
            ListsOfDescriptions listsOfDescriptions = new ListsOfDescriptions();
            listsOfDescriptions = JsonConvert.DeserializeObject<ListsOfDescriptions>(descriptionJSONStringFromServer);
            SaveDescriptions(listsOfDescriptions);
        }

        internal Task<ListsOfDescriptions> LoadListsOfDescriptionsFromFileAysnc()
        {
            return _fileService.LoadListOfDescriptionsAsync();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveMeals //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal void SaveMeals(string mealsJSONStringFromServer)
        {
            ListOfDays listsOfDays = new ListOfDays();
            listsOfDays = JsonConvert.DeserializeObject<ListOfDays>(mealsJSONStringFromServer);
            SaveMeals(listsOfDays);
        }

        public void SaveMeals(ListOfDays listsOfDays)
        {
            _fileService.SaveListOfDays(listsOfDays);
        }

        internal async Task<ListOfDays> LoadListOfDaysFromFileAsync()
        {
            return await _fileService.LoadListOfDaysAsync();
        }
    }
}
