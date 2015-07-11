using MensaApp.ViewModel;
using MensaApp.DataModel;
using MensaApp.DataModel.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MensaApp.DataModel.Rest;

namespace MensaApp.Service
{
    public class SettingsMapping
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// Nutrition //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// creates a list of new nutrition view models
        /// </summary>
        /// <param name="descriptions"></param>
        /// <param name="setting"></param>
        /// <param name="infoSymbole"></param>
        /// <param name="additives"></param>
        /// <param name="allergens"></param>
        /// <returns></returns>
        public ObservableCollection<NutritionViewModel> mapToNutritionViewModels(List<NutritionDescription> descriptions, NutritionSetting setting,
            ObservableCollection<InfoSymbolViewModel> infoSymbole, ObservableCollection<AdditiveViewModel> additives, ObservableCollection<AllergenViewModel> allergens)
        {
            ObservableCollection<NutritionViewModel> resultNutritionViewModels = new ObservableCollection<NutritionViewModel>();

            foreach (NutritionDescription description in descriptions)
            {
                if (setting != null && description.id.Equals(setting.id))
                {
                    resultNutritionViewModels.Add(mapToNutritionViewModel(description, setting, infoSymbole, additives, allergens));
                }
                else
                {
                    // if there is no setting for a description, then create a default view model.
                    resultNutritionViewModels.Add(mapToNutritionViewModel(description, null, infoSymbole, additives, allergens));
                }
            }
            return resultNutritionViewModels;
        }

        /// <summary>
        /// creates a new nutriton view Model
        /// </summary>
        /// <param name="description"></param>
        /// <param name="setting">nullable</param>
        /// <param name="infoSymbol"></param>
        /// <param name="additives"></param>
        /// <param name="allergens"></param>
        /// <returns></returns>
        private NutritionViewModel mapToNutritionViewModel(NutritionDescription description, NutritionSetting setting,
            ObservableCollection<InfoSymbolViewModel> infoSymbol, ObservableCollection<AdditiveViewModel> additives, ObservableCollection<AllergenViewModel> allergens)
        {
            ObservableCollection<InfoSymbolViewModel> excludedSymbols = getInfoSymbolViewModelsByIds(description.excludedSymbols, infoSymbol);
            ObservableCollection<AdditiveViewModel> excludedAdditive = getAdditiveViewModelsByIds(description.excludedAdditives, additives);
            ObservableCollection<AllergenViewModel> excludedAllergens = getAllergenViewModelsByIds(description.excludedAllergens, allergens);

            // check whether settings are available for a certain description.
            if (setting != null)
            {
                return new NutritionViewModel(description.id, description.name, description.definition, excludedSymbols, excludedAdditive, excludedAllergens, setting.isSelected);
            }
            else
            {
                return new NutritionViewModel(description.id, description.name, description.definition, excludedSymbols, excludedAdditive, excludedAllergens);
            }
        }

        /// <summary>
        /// Maps a list of nutriton ViewModels into a nutritionSetting.
        /// If none of the nutrition ViewModels are selected, when retrun NULL;
        /// </summary>
        /// <param name="nutritionViewModels"></param>
        /// <returns></returns>
        public NutritionSetting mapToNutritionSetting(ObservableCollection<NutritionViewModel> nutritionViewModels)
        {
            NutritionSetting nutritionSetting = null;

            NutritionViewModel selectedNutritionViewModel = getSelectedNutritionViewModel(nutritionViewModels);
            if (selectedNutritionViewModel != null)
            {
                nutritionSetting = new NutritionSetting(selectedNutritionViewModel.Id, selectedNutritionViewModel.IsSelectedNutrition);
            }
            return nutritionSetting;
        }

        /// <summary>
        /// Delivers the nutritionViewModel which is marked as selected. 
        /// If there is no NutritionViewModel selected, then return NULL.
        /// </summary>
        /// <param name="nutritionViewModels"></param>
        /// <returns></returns>
        public NutritionViewModel getSelectedNutritionViewModel(ObservableCollection<NutritionViewModel> nutritionViewModels) 
        {
            NutritionViewModel selectedNutritionViewModel = null;

            foreach (NutritionViewModel nutritionViewModel in nutritionViewModels)
            {
                if (nutritionViewModel.IsSelectedNutrition)
                {
                    selectedNutritionViewModel = nutritionViewModel;
                    break;
                }
            }
            return selectedNutritionViewModel;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// InfoSymbol /////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Maps into a list of info symbol view model from a list of symbolIds.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        public ObservableCollection<InfoSymbolViewModel> mapToInfoSymbolViewModels(List<InfoSymbolDescription> descriptions)
        {
            ObservableCollection<InfoSymbolViewModel> resultInfoSymbolViewModels = new ObservableCollection<InfoSymbolViewModel>();
            if (descriptions != null)
            {
                foreach (InfoSymbolDescription description in descriptions)
                {
                    resultInfoSymbolViewModels.Add(new InfoSymbolViewModel(description.id, description.definition));
                }
            }
            return resultInfoSymbolViewModels;
        }

        /// <summary>
        /// Delivers info symbol view models which ids are contained in the symbol id list.
        /// </summary>
        /// <param name="additiveIds"></param>
        /// <param name="descriptions"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ObservableCollection<InfoSymbolViewModel> getInfoSymbolViewModelsByIds(List<string> symbolIds, ObservableCollection<InfoSymbolViewModel> infoSymbols)
        {
            ObservableCollection<InfoSymbolViewModel> selectedInfoSymbols = new ObservableCollection<InfoSymbolViewModel>();

            if (symbolIds != null && infoSymbols != null)
            {
                foreach (InfoSymbolViewModel infoSymbol in infoSymbols)
                {
                    if (symbolIds.Contains(infoSymbol.Id))
                    {
                        selectedInfoSymbols.Add(infoSymbol);
                    }
                }
            }
            return selectedInfoSymbols;
        }

        /// <summary>
        /// Set isExclued attributes to true for all additives, which are exluded by the given nutrition.
        /// </summary>
        /// <param name="nutrition"></param>
        /// <param name="infoSymbols"></param>
        public void excludeInfoSymbolViewModelsByNutrition(NutritionViewModel nutrition, ObservableCollection<InfoSymbolViewModel> infoSymbols)
        {
            if (nutrition != null && infoSymbols != null)
            {
                foreach (InfoSymbolViewModel infoSymbol in infoSymbols)
                {
                    if (nutrition.ExcludedSymbols.Contains(infoSymbol))
                    {
                        infoSymbol.IsExcluded = true;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// Additives //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Maps into a list of additive view model by list of additive Ids from a list of additive settings and additive descripitons.
        /// Additive view models will selected by additive Ids and mapped by description
        /// </summary>
        /// <param name="additiveIds"></param>
        /// <param name="descriptions"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ObservableCollection<AdditiveViewModel> mapToAdditiveViewModelsByIds(List<string> additiveIds, List<AdditiveDescription> descriptions, List<AdditiveSetting> settings)
        {
            List<AdditiveDescription> selectedDescription = new List<AdditiveDescription>();

            foreach (AdditiveDescription description in descriptions)
            {
                if (additiveIds.Contains(description.id))
                {
                    selectedDescription.Add(description);
                }
            }
            return mapToAdditiveViewModels(selectedDescription, settings);
        }

        /// <summary>
        /// Delivers additive view models which ids are contained in the additives id list.
        /// </summary>
        /// <param name="additiveIds"></param>
        /// <param name="descriptions"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ObservableCollection<AdditiveViewModel> getAdditiveViewModelsByIds(List<string> additiveIds, ObservableCollection<AdditiveViewModel> additives)
        {
            ObservableCollection<AdditiveViewModel> selectedAdditives = new ObservableCollection<AdditiveViewModel>();

            if (additiveIds != null && additives != null)
            {
                foreach (AdditiveViewModel additive in additives)
                {
                    if (additiveIds.Contains(additive.Id))
                    {
                        selectedAdditives.Add(additive);
                    }
                }
            }
            return selectedAdditives;
        }

        /// <summary>
        /// Maps into a list of additive view model from a list of additive settings and additive descripitons.
        /// Additive view models will mapped by description 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        public ObservableCollection<AdditiveViewModel> mapToAdditiveViewModels(List<AdditiveDescription> descriptions, List<AdditiveSetting> settings)
        {
            ObservableCollection<AdditiveViewModel> resultAdditiveViewModels = new ObservableCollection<AdditiveViewModel>();

            foreach (AdditiveDescription description in descriptions)
            {
                bool isSettingAvailable = false;
                foreach (AdditiveSetting setting in settings)
                {
                    if (description.id.Equals(setting.id))
                    {
                        isSettingAvailable = true;
                        resultAdditiveViewModels.Add(mapToAdditiveViewModel(description, setting));
                        break;
                    }
                }
                if (!isSettingAvailable)
                {
                    // if there is no setting for a description, then create a default view model.
                    resultAdditiveViewModels.Add(mapToAdditiveViewModel(description, null));
                }
            }
            return resultAdditiveViewModels;
        }

        /// <summary>
        /// Maps into additive view model from additive setting and additive descripiton.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private AdditiveViewModel mapToAdditiveViewModel(AdditiveDescription description, AdditiveSetting setting)
        {
            // check whether settings are available for a certain description.
            if (setting != null)
            {
                return new AdditiveViewModel(description.id, description.definition, description.meaning, setting.isExcluded);
            }
            else
            {
                return new AdditiveViewModel(description.id, description.definition, description.meaning);
            }
        }

        /// <summary>
        /// Maps a list of additiveViewModels into a list of additive settings
        /// </summary>
        /// <param name="allergenViewModels"></param>
        /// <returns></returns>
        public List<AdditiveSetting> mapToAdditiveSettings(ObservableCollection<AdditiveViewModel> additiveViewModels)
        {
            List<AdditiveSetting> additiveSettings = new List<AdditiveSetting>();
            if (additiveViewModels != null)
            {
                foreach (AdditiveViewModel allergenViewModel in additiveViewModels)
                {
                    additiveSettings.Add(mapToAdditiveSetting(allergenViewModel));
                }
            }
            return additiveSettings;
        }

        /// <summary>
        /// maps a single additiveViewModel into a additive setting
        /// </summary>
        /// <param name="additiveViewModel"></param>
        /// <returns></returns>
        private AdditiveSetting mapToAdditiveSetting(AdditiveViewModel additiveViewModel)
        {
            return new AdditiveSetting(additiveViewModel.Id, additiveViewModel.IsExcluded);
        }

        /// <summary>
        /// Set isDisabled and isExclued attributes to true for all additives, which are exluded by the given nutrition.
        /// </summary>
        /// <param name="selectedNutrition"></param>
        /// <param name="additives"></param>
        public void excludeAdditiveViewModelsByNutrition(NutritionViewModel selectedNutrition, ObservableCollection<AdditiveViewModel> additives)
        {
            if (selectedNutrition != null && additives != null)
            {
                foreach (AdditiveViewModel additive in additives)
                {
                    if (selectedNutrition.ExcludedAdditives.Contains(additive))
                    {
                        additive.IsDisabled = true;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// Allergens //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Maps into a list of allergen view model by list of allergen Ids from a list of allergen settings and allergen descripitons.
        /// Allergen view models will selected by allergen Ids and mapped by description
        /// </summary>
        /// <param name="allergenIds"></param>
        /// <param name="descriptions"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ObservableCollection<AllergenViewModel> mapToAllergensViewModelsByAdditiveIds(List<string> allergenIds, List<AllergenDescription> descriptions, List<AllergenSetting> settings)
        {
            List<AllergenDescription> selectedDescription = new List<AllergenDescription>();

            foreach (AllergenDescription description in descriptions)
            {
                if (allergenIds.Contains(description.id))
                {
                    selectedDescription.Add(description);
                }
            }
            return mapToAllergenViewModels(selectedDescription, settings);
        }

        /// <summary>
        /// Delivers allergen view models which ids are contained in the allergen id list.
        /// </summary>
        /// <param name="allergenIds"></param>
        /// <param name="descriptions"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ObservableCollection<AllergenViewModel> getAllergenViewModelsByIds(List<string> allergenIds, ObservableCollection<AllergenViewModel> allergens)
        {
            ObservableCollection<AllergenViewModel> selectedAllergens = new ObservableCollection<AllergenViewModel>();

            if (allergenIds != null && allergens != null)
            {
                foreach (AllergenViewModel allergen in allergens)
                {
                    if (allergenIds.Contains(allergen.Id))
                    {
                        selectedAllergens.Add(allergen);
                    }
                }
            }
            return selectedAllergens;
        }

        /// <summary>
        /// Maps into a list of allergen view model from a list of allergen settings and allergen descripitons.
        /// Allergen view models will mapped by description 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        public ObservableCollection<AllergenViewModel> mapToAllergenViewModels(List<AllergenDescription> descriptions, List<AllergenSetting> settings)
        {
            ObservableCollection<AllergenViewModel> resultAllergenViewModels = new ObservableCollection<AllergenViewModel>();

            foreach (AllergenDescription description in descriptions)
            {
                bool isSettingAvailable = false;
                foreach (AllergenSetting setting in settings)
                {
                    if (description.id.Equals(setting.id))
                    {
                        isSettingAvailable = true;
                        resultAllergenViewModels.Add(mapToAllergenViewModel(description, setting));
                        break;
                    }
                }
                if (!isSettingAvailable)
                {
                    // if there is no setting for a description, then create a default view model.
                    resultAllergenViewModels.Add(mapToAllergenViewModel(description, null));
                }
            }
            return resultAllergenViewModels;
        }

        /// <summary>
        /// Maps into allergen view model from allergen setting and allergen descripiton.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private AllergenViewModel mapToAllergenViewModel(AllergenDescription description, AllergenSetting setting)
        {
            // check whether settings are available for a certain description.
            if (setting != null)
            {
                return new AllergenViewModel(description.id, description.definition, description.containedIn, setting.isExcluded);
            }
            else
            {
                return new AllergenViewModel(description.id, description.definition, description.containedIn);
            }
        }

        /// <summary>
        /// Maps a list of allergenVieModels into a list of allergen settings
        /// </summary>
        /// <param name="allergenViewModels"></param>
        /// <returns></returns>
        public List<AllergenSetting> mapToAllergenSettings(ObservableCollection<AllergenViewModel> allergenViewModels)
        {
            List<AllergenSetting> allergenSettings = new List<AllergenSetting>();
            if (allergenViewModels != null) {
                foreach (AllergenViewModel allergenViewModel in allergenViewModels)
                {
                    allergenSettings.Add(mapToAllergenSetting(allergenViewModel));
                }
            }
            return allergenSettings;
        }

        /// <summary>
        /// maps a single allergenViewModel into a allergen setting
        /// </summary>
        /// <param name="allergenViewModel"></param>
        /// <returns></returns>
        private AllergenSetting mapToAllergenSetting(AllergenViewModel allergenViewModel) 
        {
            return new AllergenSetting(allergenViewModel.Id, allergenViewModel.IsExcluded);
        }

        /// <summary>
        /// Set isDisabled and isExclued attributes to true for all allergens, which are exluded by the given nutrition.
        /// </summary>
        /// <param name="selectedNutrition"></param>
        /// <param name="allergens"></param>
        public void excludeAllergenViewModelsByNutrition(NutritionViewModel selectedNutrition, ObservableCollection<AllergenViewModel> allergens)
        {
            if (selectedNutrition != null && allergens != null)
            {
                foreach (AllergenViewModel allergen in allergens)
                {
                    if (selectedNutrition.ExcludedAllergens.Contains(allergen))
                    {
                        allergen.IsDisabled = true;
                    }
                }
            }
        }
    }
}
