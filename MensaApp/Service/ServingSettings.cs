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
        /// Combines all discriptions and all settings to a list of settingViewModels
        /// </summary>
        public async Task<ListOfSettingViewModel> GetListOfSettingViewModelsAsync()
        {
            ListOfSettingViewModel resultListOfSettingViewModel = new ListOfSettingViewModel();

            // Phase 1
            // Load descriptions and settings from file
            // Starts both async tasks in the first step
            Task<ListsOfDescriptions> listsOfDescriptionsTask = _fileService.LoadListOfDescriptionsAsync();
            Task<ListsOfSettings> listOfSettingsTask = _fileService.LoadListOfSettingsAsync();
            // Await both async tasks to finish in the second step
            ListsOfDescriptions listsOfDescriptions = await listsOfDescriptionsTask;
            ListsOfSettings listOfSettings = await listOfSettingsTask;

            // Phase 2
            // Combines descriptions and settings to viewModels
            resultListOfSettingViewModel.AdditiveViewModels = _settingsMapping.mapToAdditiveViewModels(listsOfDescriptions.additives, listOfSettings.additivSettings);
            resultListOfSettingViewModel.AllergenViewModels = _settingsMapping.mapToAllergenViewModels(listsOfDescriptions.allergens, listOfSettings.allergenSettings);
            resultListOfSettingViewModel.InfoSymbolViewModels = _settingsMapping.mapToInfoSymbolViewModels(listsOfDescriptions.infoSymbols);

            // Phase 3
            // Combine complex nutritionViewModels
            resultListOfSettingViewModel.NutritionViewModels = _settingsMapping.mapToNutritionViewModels(listsOfDescriptions.nutritions,
                listOfSettings.nutritionSetting, resultListOfSettingViewModel.InfoSymbolViewModels, resultListOfSettingViewModel.AdditiveViewModels, resultListOfSettingViewModel.AllergenViewModels);

            // Phase 4
            // mark additives, allergens and infoSymbols as excluded by selected nutrition.
            NutritionViewModel selectedNutritionViewModel = _settingsMapping.getSelectedNutritionViewModel(resultListOfSettingViewModel.NutritionViewModels);
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
            _fileService.SaveListOfSettingsAsync(listsOfSettings);
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
        private void SaveDescriptions(ListsOfDescriptions listsOfDescriptions)
        {
            _fileService.SaveListOfDescriptionsAsync(listsOfDescriptions);
        }

        internal void SaveDescriptions(string descriptionJSONStringFromServer)
        {
            ListsOfDescriptions listsOfDescriptions = new ListsOfDescriptions();
            listsOfDescriptions = JsonConvert.DeserializeObject<ListsOfDescriptions>(descriptionJSONStringFromServer);
            SaveDescriptions(listsOfDescriptions);
        }

        internal void SaveMeals(string mealsJSONStringFromServer)
        {
            ListOfDays listsOfDays = new ListOfDays();
            listsOfDays = JsonConvert.DeserializeObject<ListOfDays>(mealsJSONStringFromServer);
            SaveMeals(listsOfDays);
        }

        private void SaveMeals(ListOfDays listsOfDays)
        {
            _fileService.SaveListOfDaysAsync(listsOfDays);
        }

        internal async Task<ListOfDays> GetListOfDays()
        {
            return await _fileService.LoadListOfDaysAsync();
        }
    }
}
