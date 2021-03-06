﻿using MensaApp.Common;
using MensaApp.Service;
using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace MensaApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private NavigationHelper navigationHelper;

        private DataAndUpdateService _dataAndUpdateService;
        private SettingsPageViewModel _settingViewModel = new SettingsPageViewModel();

        public SettingPage()
        {
            this.InitializeComponent();
            _dataAndUpdateService = new DataAndUpdateService();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public SettingsPageViewModel SettingsPageViewModel
        {
            get { return this._settingViewModel; }
        }

        /// <summary>
        /// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        
        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Laden der Settings lokal
            ListOfSettingViewModel listOfSettingViewModel = await _dataAndUpdateService.DeliverSettingsForSettingsPage();

            // Aktualisieren der Oberflaeche
            _settingViewModel.Nutritions = listOfSettingViewModel.NutritionViewModels;
            _settingViewModel.Additives = listOfSettingViewModel.AdditiveViewModels;
            _settingViewModel.Allergens = listOfSettingViewModel.AllergenViewModels;
        }

        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird.  Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/></param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper-Registrierung

        /// <summary>
        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// <para>
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="NavigationHelper.LoadState"/>
        /// und <see cref="NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.
        /// </para>
        /// </summary>
        /// <param name="e">Stellt Daten für Navigationsmethoden und -ereignisse bereit.
        /// Handler, bei denen die Navigationsanforderung nicht abgebrochen werden kann.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void SaveSettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // Fortschrittsbalken einblenden
            ProgressBar.Visibility = Visibility.Visible;

            await _dataAndUpdateService.SaveSettingsFromSettingsPage(_settingViewModel.Nutritions, _settingViewModel.Additives, _settingViewModel.Allergens);

            // Fortschrittsbalken ausblenden
            ProgressBar.Visibility = Visibility.Collapsed;

            // Zu dem heutigen Essensangebot navigieren
            Frame mensaFrame = MainPage.Current.FindName("MensaFrame") as Frame;
            if (mensaFrame != null && mensaFrame.CanGoBack)
                mensaFrame.GoBack();
        }

        /// <summary>
        /// updates the settingsViewModel, when the selection of the nutrition combobox has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NutritionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox nutritionCombobox = sender as ComboBox;
            NutritionViewModel selectedNutritionViewModel = nutritionCombobox.SelectedItem as NutritionViewModel;

            if (selectedNutritionViewModel != null)
            {
                _settingViewModel.SelectedNutrition = selectedNutritionViewModel;

                foreach (NutritionViewModel nutritionViewModel in _settingViewModel.Nutritions)
                {
                    nutritionViewModel.IsSelectedNutrition = nutritionViewModel.Id.Equals(selectedNutritionViewModel.Id) ? true : false;
                }
            }
            // Update disabled additives und allergens after nutrition selection has changed.
            _settingViewModel.Additives = _dataAndUpdateService.UpdateSettingsAdditivesBySelectedNutrition(_settingViewModel.SelectedNutrition, _settingViewModel.Additives);
            _settingViewModel.Allergens = _dataAndUpdateService.UpdateSettingsAllergensBySelectedNutrition(_settingViewModel.SelectedNutrition, _settingViewModel.Allergens);
            AdditivesList.UpdateLayout();
            AllergensList.UpdateLayout();
        }
    }
}
