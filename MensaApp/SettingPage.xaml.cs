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

        public SettingPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public SettingsPageViewModel SettingsPageViewModel
        {
            get { return this._settingViewModel; }
        }
        
        private void populateNutritions()
        {
            // Add normal nutrition
            string normalDefinition = "Keine Einschränkungen.";
            _settingViewModel.Nutritions.Add(new NutritionViewModel("(NORMAL)", "Normal", normalDefinition, new ObservableCollection<InfoSymbolViewModel>(), new ObservableCollection<AdditiveViewModel>(), new ObservableCollection<AllergenViewModel>(), true));

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
            _settingViewModel.Nutritions.Add(new NutritionViewModel("(VEGGIE)", "Ovo-Lacto-Vegetarisch", veggieDefinition, infoSymbolsVegi, excludedAdditivesVegi, excludedAllergensVegi));

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
            _settingViewModel.Nutritions.Add(new NutritionViewModel("(VEGAN)", "Vegan", veganDefinition, infoSymbolsVega, excludedAdditivesVega, excludedAllergensVega));

            _settingViewModel.SelectedNutrition = _settingViewModel.Nutritions.First();
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
            //// DC MockUp
            //populateNutritions();

            // neues Objekt zum Laden der Settings
            ServingSettings servingSettings = new ServingSettings();

            // Laden der Settings lokal
            Task<ListOfSettingViewModel> listOfTaskWithSettingViewModels = servingSettings.getListOfSettingViewModelsAsync();

            // Task abwarten
            ListOfSettingViewModel listOfSettingViewModel = await listOfTaskWithSettingViewModels;

            // Aktualisieren der Oberflaeche
            _settingViewModel.Nutritions = listOfSettingViewModel.NutritionViewModels;
            _settingViewModel.Additives = listOfSettingViewModel.AdditiveViewModels;
            _settingViewModel.Allergens = listOfSettingViewModel.AllergenViewModels;

            if (listOfSettingViewModel.AdditiveViewModels.Count == 0)
            {
                // ????? Kann das wieder verwendet werden?
                synchronizeWithServer();
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

            ServingSettings servingSettings = new ServingSettings();

            servingSettings.SaveSettings(_settingViewModel.Nutritions, _settingViewModel.Additives, _settingViewModel.Allergens);

            // Fortschrittsbalken ausblenden
            ProgressBar.Visibility = Visibility.Collapsed;

            // Zu dem heutigen Essensangebot navigieren
            Frame.Navigate(typeof(MealsPage));
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
        /// Neu-Laden aller Zusatzstoffe und Allergene
        /// </summary>
        private async void synchronizeWithServer()
        {
            // Ressourcen ladden
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");

            // Holen des Grundaufrufs
            String AdditivesBaseURL = MensaRestApiResource.GetString("AdditivesBaseURL");

            // Holen der vertiefenden Struktur
            String AdditivesPathURL = MensaRestApiResource.GetString("AdditivesURL");

            // erzeuge neues Objekt
            ServingMealOffer servingMO = new ServingMealOffer();

            // Hole das JSON und speichere in Datei
            await servingMO.GetServerData(AdditivesBaseURL, AdditivesPathURL, "AdditivesJSONFile");

            // erzeuge neues Objekt
            ServingAdditivesAndAllergenes servingAAA1 = new ServingAdditivesAndAllergenes();

            // Erstelle Zusatzstoffe
            List<AdditiveViewModel> listeZusatzstoffe = await servingAAA1.GetAdditives();

            // Hole das JSON und speichere in Datei
            await servingMO.GetServerData(AdditivesBaseURL, AdditivesPathURL, "AdditivesJSONFile");

            // erzeuge neues Objekt
            ServingAdditivesAndAllergenes servingAAA2 = new ServingAdditivesAndAllergenes();
            List<AllergenViewModel> listeAllergene = await servingAAA2.GetAllergenes();

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _settingViewModel.Additives.Clear();

            foreach (AdditiveViewModel additiveVM in listeZusatzstoffe)
            {
                // Alle SettingViewModel der GUI uebergeben
                _settingViewModel.Additives.Add(additiveVM);
            }

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _settingViewModel.Allergens.Clear();

            foreach (AllergenViewModel allergenVM in listeAllergene)
            {
                // Alle SettingViewModel der GUI uebergeben
                _settingViewModel.Allergens.Add(allergenVM);
            }
        }

    }
}
