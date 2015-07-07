﻿using MensaApp.Common;
using MensaApp.Service;
using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
            //_settingViewModel.Nutritions.Add(new NutritionViewModel("(NORMAL)", "Normal", normalDefinition, new ObservableCollection<string>(), new ObservableCollection<AdditiveViewModel>(), new ObservableCollection<AllergenViewModel>(), true));

            // Add Vegetarian nutrition
            ObservableCollection<string> symbolsVegi = new ObservableCollection<string>();
            ObservableCollection<AllergenViewModel> excludedAllergensVegi = new ObservableCollection<AllergenViewModel>();
            ObservableCollection<AdditiveViewModel> excludedAdditivesVegi = new ObservableCollection<AdditiveViewModel>();
            symbolsVegi.Add("mit Schweinefleisch");
            symbolsVegi.Add("mit Rindfleisch");
            symbolsVegi.Add("mit Lamm");
            symbolsVegi.Add("mit Fisch");
            symbolsVegi.Add("mit Geflügelfleisch");
            excludedAdditivesVegi.Add(new AdditiveViewModel("(GE)", "mit Gelatine", "", false));
            excludedAllergensVegi.Add(new AllergenViewModel("(N)", "Weichtiere sind Schnecken, Muscheln, Austern und Tintenfische", "Fisch- und Feinkostsalate, Paella und Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
            excludedAllergensVegi.Add(new AllergenViewModel("(D)", "Fisch", "Paella, Bouillabaise, Worchester Sauce, asiatische Würzpasten", false));
            excludedAllergensVegi.Add(new AllergenViewModel("(B)", "Krebstiere sind Garnelen, Hummer, Fluss-und Taschenkrebse, Krabben", "Feinkostsalate, Paella, Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
            string veggieDefinition = "Ovo-Lacto-Vegetarier essen nichts vom toten Tier.";
            //_settingViewModel.Nutritions.Add(new NutritionViewModel("(VEGGIE)", "Ovo-Lacto-Vegetarisch", veggieDefinition, symbolsVegi, excludedAdditivesVegi, excludedAllergensVegi));

            // Add Vegan nutrion
            ObservableCollection<string> symbolsVega = new ObservableCollection<string>();
            ObservableCollection<AllergenViewModel> excludedAllergensVega = new ObservableCollection<AllergenViewModel>();
            ObservableCollection<AdditiveViewModel> excludedAdditivesVega = new ObservableCollection<AdditiveViewModel>();
            symbolsVega.Add("mit Schweinefleisch");
            symbolsVega.Add("mit Rindfleisch");
            symbolsVega.Add("mit Lamm");
            symbolsVega.Add("mit Fisch");
            symbolsVega.Add("mit Geflügelfleisch");

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
            //_settingViewModel.Nutritions.Add(new NutritionViewModel("(VEGAN)", "Vegan", veganDefinition, symbolsVega, excludedAdditivesVega, excludedAllergensVega));

            //_settingViewModel.SelectedNutrition = _settingViewModel.Nutritions.First();
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // DC MockUp
            //populateNutritions();

            // HK local-saved Settings, wenn nichts vorhanden vom Server
            holeSettings();
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
            ProgressBar.Visibility = Visibility.Visible;
            // Neuen Serializer erzeugen
            SerializeSettings ss = new SerializeSettings();

            // Hole den Standort der JSON Datei
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            String dateiName = MensaRestApiResource.GetString("SettingsAdditivesJSONFile");

            // Uebergebe die aktuellen vorgenommenen Einstellungen zum Serialisieren (Zusatzstoffe)
            ss.serializeAdditives(_settingViewModel.Additives, dateiName);

            dateiName = MensaRestApiResource.GetString("SettingsAllergenesJSONFile");

            // Uebergebe die aktuellen vorgenommenen Einstellungen zum Serialisieren (Allergene)
            ss.serializeAllergenes(_settingViewModel.Allergens, dateiName);
            ProgressBar.Visibility = Visibility.Collapsed;
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

        private async void holeSettings()
        {
            SerializeSettings ss = new SerializeSettings();
            ObservableCollection<ViewModel.AdditiveViewModel> oo = new ObservableCollection<ViewModel.AdditiveViewModel>();
            ObservableCollection<ViewModel.AllergenViewModel> oo2 = new ObservableCollection<ViewModel.AllergenViewModel>();

            // Hole den Standort der JSON Datei
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            String dateiName = MensaRestApiResource.GetString("SettingsAdditivesJSONFile");

            oo = await ss.deserializeAdditives(dateiName);

            if (oo.Count == 0)
            {
                synchronizeWithServer();
            } else
            {
                _settingViewModel.Additives.Clear();
                foreach (AdditiveViewModel addiVM in oo)
                {
                    _settingViewModel.Additives.Add(new AdditiveViewModel(addiVM.Id, addiVM.Definition, addiVM.Meaning, addiVM.IsExcluded));
                }
                
            }

            dateiName = MensaRestApiResource.GetString("SettingsAllergenesJSONFile");
            oo2 = await ss.deserializeAllergenes(dateiName);

            if (oo2.Count == 0)
            {
                synchronizeWithServer();
            }
            else
            {
                _settingViewModel.Allergens.Clear();
                foreach (AllergenViewModel allergVM in oo2)
                {
                    _settingViewModel.Allergens.Add(new AllergenViewModel(allergVM.Id, allergVM.Definition, allergVM.ContainedIn, allergVM.IsExcluded));
                }

            }
        }

        /// <summary>
        /// Neu-Laden aller Zusatzstoffe und Allergene
        /// </summary>
        private async void synchronizeWithServer()
        {
            // Hole die MensaRestSchnittstellen Parameter
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            String AdditivesBaseURL = MensaRestApiResource.GetString("AdditivesBaseURL");
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
