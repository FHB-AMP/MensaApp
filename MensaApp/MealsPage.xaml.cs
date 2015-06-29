using MensaApp.Common;
using MensaApp.DataModel;
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
using Windows.Storage;
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
    public sealed partial class MealsPage : Page
    {
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MealsPageViewModel _mealsPageViewModel = new MealsPageViewModel();
        
        public MealsPage()
        {
            this.InitializeComponent();
            
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public MealsPageViewModel MealsPageViewModel
        {
            get { return _mealsPageViewModel; }
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
            synchronizeWithServer();
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

        private void SettingAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        /// <summary>
        /// Neu-Laden der aktuellen Gerichte und anschließender Aktualisierung der Oberflaeche.
        /// </summary>
        private async void synchronizeWithServer()
        {
            // Zeige den Progressbar fuer den Zeitraum der asynchronen Datenverarbeitung
            ProgressBar.Visibility = Visibility.Visible;

            // Hole die MensaRestSchnittstellen Parameter
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            String MealURI = MensaRestApiResource.GetString("MealBaseURL");
            String MealURL = MensaRestApiResource.GetString("MealURL");

            // erzeuge neues Objekt
            ServingMealOffer servingMealOffer = new ServingMealOffer();

            // Start taeglich einmalige Synchro
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Hole den Speicherort der JSON Datei
            MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            String dateiName = MensaRestApiResource.GetString("MealJSONFile");

            // Helfer
            DateTime myDate = DateTime.MinValue;

            try
            {
                // Wenn nicht vorhanden FileNotFoundException
                StorageFile sampleFile = await localFolder.GetFileAsync(dateiName);

                // modified Zeitpunkt aus Properties holen
                var check = new List<string>();
                check.Add("System.DateModified");
                var props = await sampleFile.Properties.RetrievePropertiesAsync(check);
                var dateModified = props.SingleOrDefault().Value;

                // Property als DateTime parsen
                myDate = DateTime.ParseExact(dateModified.ToString(), "dd.MM.yyyy HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);

            }
            catch (FileNotFoundException) 
            { 
                // Tue nichts :)
                // debug message könnte man hier vll ablegen.
            }

            if (DateTime.Today != myDate)
            {
                // Hole das JSON und speichere in Datei
                await servingMealOffer.GetServerData(MealURI, MealURL, "MealJSONFile");
            }

            // Der Rest muss immer passieren
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            int amountOfDays = 6; // Current day + 5 days of forcast
            List<DayViewModel> AllDaysWithMeals = await servingMealOffer.FindMealOffersForCertainAmountOfDays(amountOfDays);
            
            ObservableCollection<DayViewModel> today = servingMealOffer.SearchMealsOfToday(DateTime.Today, AllDaysWithMeals);
            ObservableCollection<DayViewModel> forecast = servingMealOffer.SearchMealOfForecast(DateTime.Today, AllDaysWithMeals);

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _mealsPageViewModel.Today.Clear();
            // DayViewModel der GUI uebergeben
            _mealsPageViewModel.Today = today;

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _mealsPageViewModel.ForecastDays.Clear();
            // DayViewModels der GUI uebergeben
            _mealsPageViewModel.ForecastDays = forecast;

            ProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Manuelles Ausloesen des Neu-Ladens der aktuellen Gerichte und anschließender Aktualisierung der Oberflaeche anstoßen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            synchronizeWithServer();
        }

        /// <summary>
        /// Handles the tapped event from a list view item of meals lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MealsListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView listView = sender as ListView;
            MealViewModel selectedMeal = listView.SelectedItem as MealViewModel;

            if (selectedMeal != null)
            {
                // TODO insert correct datetime.
                DetailPageParamModel paramModel = new DetailPageParamModel(DateTime.Now, selectedMeal);
                Frame.Navigate(typeof(DetailPage), paramModel);
            }

        }

        private void FummelAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MealDetailPage));
        }

        private void ImpressumAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImpressumPage));
        }
    }
}
