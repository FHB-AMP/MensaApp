using MensaApp.Common;
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
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private SettingsPageViewModel _settingViewModel = new SettingsPageViewModel();

        private ServingSettings _servingSettings;

        public SettingPage()
        {
            this.InitializeComponent();

            _servingSettings = new ServingSettings();

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
        /// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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

            // For-Schleife zur Beahndlung von Verbindungsproblemen
            for (int i = 0; i < 2; i++)
            {
                // Laden der Settings lokal
                Task<ListOfSettingViewModel> listOfTaskWithSettingViewModels = _servingSettings.GetListOfSettingViewModelsAsync();

                // Task abwarten
                ListOfSettingViewModel listOfSettingViewModel = await listOfTaskWithSettingViewModels;

                // Aktualisieren der Oberflaeche
                _settingViewModel.Nutritions = listOfSettingViewModel.NutritionViewModels;

                // Setzen der ausgewaehlten Ernaehrungsweise im Dropdown-Menue
                foreach (NutritionViewModel nutritionVM in _settingViewModel.Nutritions)
                {
                    if (nutritionVM.IsSelectedNutrition)
                    {
                        _settingViewModel.SelectedNutrition = nutritionVM;
                    }
                } 

                _settingViewModel.Additives = listOfSettingViewModel.AdditiveViewModels;
                _settingViewModel.Allergens = listOfSettingViewModel.AllergenViewModels;

                if (listOfSettingViewModel.AdditiveViewModels.Count == 0 || listOfSettingViewModel.AllergenViewModels.Count == 0 || listOfSettingViewModel.NutritionViewModels.Count == 0)
                {
                    getDescriptionsFromServer();
                }
                else
                {
                    // Wenn Daten gefunden, dann kein weiterer Schleifendurchlauf
                    i = 2;
                }
            }

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

        private void SaveSettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // Fortschrittsbalken einblenden
            ProgressBar.Visibility = Visibility.Visible;

            _servingSettings.SaveSettings(_settingViewModel.Nutritions, _settingViewModel.Additives, _settingViewModel.Allergens);

            // Fortschrittsbalken ausblenden
            ProgressBar.Visibility = Visibility.Collapsed;

            // Zu dem heutigen Essensangebot navigieren // TODO Holger Daniel
            //Frame.Navigate(typeof(MealsPage));
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
        }

        /// <summary>
        /// Laden aller Zusatzstoffe und Allergene und Ernaehrungsweisen vom REST-Service
        /// </summary>
        private async void getDescriptionsFromServer()
        {
            // Ressourcen laden
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");

            // Holen des Grundaufrufs
            String DescriptionsBaseURL = MensaRestApiResource.GetString("DescriptionsBaseURL");

            // Holen der vertiefenden Struktur
            String DescriptionsPathURL = MensaRestApiResource.GetString("DescriptionsURL");

            // erzeuge neues Objekt
            ServingMealOffer servingMO = new ServingMealOffer();

            // Hole das JSON und speichere in Datei
            string descriptionJSONStringFromServer = await servingMO.GetServerData(DescriptionsBaseURL, DescriptionsPathURL);

            // Beschriebungen lokal vom Server abspeichern
            _servingSettings.SaveDescriptions(descriptionJSONStringFromServer);
            
        }

    }
}
