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
        internal ListOfSettingViewModel GetListOfSettingViewModels(ListsOfDescriptions listsOfDescriptions, ListsOfSettings listOfSettings)
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

        internal void UpdateExcludingOfAdditives(NutritionViewModel selectedNutritionViewModel, ObservableCollection<AdditiveViewModel> additiveViewModels)
        {
            _settingsMapping.excludeAdditiveViewModelsByNutrition(selectedNutritionViewModel, additiveViewModels);
        }

        internal void UpdateExcludingOfAllergens(NutritionViewModel selectedNutritionViewModel, ObservableCollection<AllergenViewModel> allergensViewModel)
        {
            _settingsMapping.excludeAllergenViewModelsByNutrition(selectedNutritionViewModel, allergensViewModel);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveSettings ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Saves Settings to file
        /// </summary>
        /// <param name="nutritionViewModels"></param>
        /// <param name="additiveViewModels"></param>
        /// <param name="allergenViewModels"></param>
        public async Task SaveSettings(ObservableCollection<NutritionViewModel> nutritionViewModels, 
            ObservableCollection<AdditiveViewModel> additiveViewModels, ObservableCollection<AllergenViewModel> allergenViewModels)
        {
            ListsOfSettings listsOfSettings = new ListsOfSettings();
            listsOfSettings.nutritionSetting = _settingsMapping.mapToNutritionSetting(nutritionViewModels);
            listsOfSettings.additivSettings = _settingsMapping.mapToAdditiveSettings(additiveViewModels);
            listsOfSettings.allergenSettings = _settingsMapping.mapToAllergenSettings(allergenViewModels);
            await SaveSettings(listsOfSettings);
            return;
        }

        /// <summary>
        /// Save Settings to file
        /// </summary>
        /// <param name="listsOfSettings"></param>
        private async Task SaveSettings(ListsOfSettings listsOfSettings)
        {
            await _fileService.SaveListOfSettings(listsOfSettings);
            return;
        }

        internal Task<ListsOfSettings> LoadListsOfSettingsFromFileAysnc()
        {
            return _fileService.LoadListOfSettingsAsync();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveAndLoadDescriptions ////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Save descriptions to file
        /// </summary>
        /// <param name="listsOfSettings"></param>
        public async Task SaveDescriptions(ListsOfDescriptions listsOfDescriptions)
        {
            await _fileService.SaveListOfDescriptions(listsOfDescriptions);
            return;
        }

        internal Task<ListsOfDescriptions> LoadListsOfDescriptionsFromFileAysnc()
        {
            return _fileService.LoadListOfDescriptionsAsync();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveAndLoadMeals ///////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public async Task SaveMeals(ListOfDays listsOfDays)
        {
            await _fileService.SaveListOfDays(listsOfDays);
            return;
        }

        internal async Task<ListOfDays> LoadListOfDaysFromFileAsync()
        {
            return await _fileService.LoadListOfDaysAsync();
        }
    }
}
