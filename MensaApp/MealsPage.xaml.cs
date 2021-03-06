﻿using MensaApp.Common;
using MensaApp.DataModel;
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
        private NavigationHelper navigationHelper;

        private MealsPageViewModel _mealsPageViewModel = new MealsPageViewModel();
        private DataAndUpdateService _dataAndUpdateService;
        private bool _isLastNavigationTargetSettingsPage = false;
        
        public MealsPage()
        {
            this.InitializeComponent();

            _dataAndUpdateService = new DataAndUpdateService();
            
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
            // Restore page state
            RestorePageState(e);

            if (_mealsPageViewModel.Today.Count == 0 && _mealsPageViewModel.ForecastDays.Count == 0)
            {
                prepareMealsPage(false);
            }
        }

        /// <summary>
        /// Calls relevant properties form the page state dictionary and restore the state of the page when possible.
        /// </summary>
        /// <param name="e"></param>
        private void RestorePageState(LoadStateEventArgs e)
        {
            // restore last visited pivot item.
            if (e.PageState != null && e.PageState.ContainsKey("PivotIndex"))
            {
                object pivotIndex = 0;
                e.PageState.TryGetValue("PivotIndex", out pivotIndex);
                if (pivotIndex != null)
                {
                    mealsPivot.SelectedIndex = Convert.ToInt32(pivotIndex);
                }
            }

            // restore the last navigation target page
            if (e.PageState != null && e.PageState.ContainsKey("IsLastNavigationTargetSettingsPage"))
            {
                object isLastNavigationTargetObject = 0;
                e.PageState.TryGetValue("IsLastNavigationTargetSettingsPage", out isLastNavigationTargetObject);
                if (isLastNavigationTargetObject != null)
                {
                    _isLastNavigationTargetSettingsPage = (bool)isLastNavigationTargetObject;
                }
            }

            if (_isLastNavigationTargetSettingsPage)
            {
                // reload meals page when settings page was the last target page.
                prepareMealsPage(false);
            }
            else
            {
                // restore MealsPageViewModel
                if (e.PageState != null && e.PageState.ContainsKey("MealsPageViewModel"))
                {
                    object mealsPageViewModel;
                    e.PageState.TryGetValue("MealsPageViewModel", out mealsPageViewModel);
                    if (mealsPageViewModel != null)
                    {
                        _mealsPageViewModel.Today = (mealsPageViewModel as MealsPageViewModel).Today;
                        _mealsPageViewModel.ForecastDays = (mealsPageViewModel as MealsPageViewModel).ForecastDays;
                    }
                }
                // restore selected item of today listview and set scroller position.
                if (e.PageState != null && e.PageState.ContainsKey("TodayListIndex"))
                {
                    object listIndex = 0;
                    e.PageState.TryGetValue("TodayListIndex", out listIndex);
                    if (listIndex != null)
                    {
                        int listIndexInt = Convert.ToInt32(listIndex);
                        // take care about to select only exsiting items
                        if (listIndexInt >= 0 && TodayList.Items.Count > listIndexInt)
                        {
                            TodayList.SelectedIndex = listIndexInt;
                            TodayList.ScrollIntoView(TodayList.SelectedItem, ScrollIntoViewAlignment.Leading);
                            TodayList.UpdateLayout();
                        }
                    }
                }
                // restore selected item of forcast listview and set scroller position.
                if (e.PageState != null && e.PageState.ContainsKey("ForecastListIndex"))
                {
                    object listIndex = 0;
                    e.PageState.TryGetValue("ForecastListIndex", out listIndex);
                    if (listIndex != null)
                    {
                        int listIndexInt = Convert.ToInt32(listIndex);
                        // take care about to select only exsiting items
                        if (listIndexInt >= 0 && ForecastList.Items.Count > listIndexInt)
                        {
                            ForecastList.SelectedIndex = listIndexInt;
                            ForecastList.ScrollIntoView(ForecastList.SelectedItem, ScrollIntoViewAlignment.Leading);
                            ForecastList.UpdateLayout();
                        }
                    }
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
            StorePageState(e);
        }

        /// <summary>
        /// Stores the relevant elements of the page.
        /// </summary>
        /// <param name="e"></param>
        private void StorePageState(SaveStateEventArgs e)
        {
            e.PageState.Add("PivotIndex", mealsPivot.SelectedIndex);
            e.PageState.Add("MealsPageViewModel", _mealsPageViewModel);
            e.PageState.Add("TodayListIndex", TodayList.SelectedIndex);
            e.PageState.Add("ForecastListIndex", ForecastList.SelectedIndex);
            e.PageState.Add("IsLastNavigationTargetSettingsPage", _isLastNavigationTargetSettingsPage);
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
            if (e.SourcePageType.Equals(typeof(SettingPage)))
            {
                Debug.WriteLine("from settings page");
            }
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SettingAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            _isLastNavigationTargetSettingsPage = true;
            Frame.Navigate(typeof(SettingPage));
        }

        /// <summary>
        /// Neu-Laden der aktuellen Gerichte und anschließender Aktualisierung der Oberflaeche.
        /// </summary>
        private async void prepareMealsPage(bool forceUpdateFromServer)
        {
            // Zeige den Progressbar fuer den Zeitraum der asynchronen Datenverarbeitung
            ProgressBar.Visibility = Visibility.Visible;
            RefreshAppBarButton.IsEnabled = false;

            int amountOfDays = 6; // Current day + 5 days of forcast
            // load all meals from file
            List<DayViewModel> AllDaysWithMeals = await _dataAndUpdateService.DeliverAllDaysWithMealsForMealsPage(amountOfDays, forceUpdateFromServer);
            // search for meals of the current day
            ObservableCollection<DayViewModel> today = _dataAndUpdateService.SearchMealsOfToday(AllDaysWithMeals);
            // search for forcast meals
            ObservableCollection<DayViewModel> forecast = _dataAndUpdateService.SearchMealOfForecast(AllDaysWithMeals);

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _mealsPageViewModel.Today.Clear();
            // DayViewModel der GUI uebergeben
            _mealsPageViewModel.Today = today;
            TodayList.ClearValue(ListView.ItemsPanelProperty);

            // fuer erneutes ausfuehren zuvor loeschen, ansonsten doppelt
            _mealsPageViewModel.ForecastDays.Clear();
            // DayViewModels der GUI uebergeben
            _mealsPageViewModel.ForecastDays = forecast;

            PrepareListTransitionAninmation();
            RefreshAppBarButton.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Scroll at first position of lists and clear both lists of meals to trigger transition animation.
        /// </summary>
        private void PrepareListTransitionAninmation()
        {
            // today list of meals
            if (_mealsPageViewModel != null &&
                _mealsPageViewModel.Today != null &&
                _mealsPageViewModel.Today.Count > 0)
            {
                TodayList.ClearValue(ListView.ItemsPanelProperty);
                DayViewModel firstTodayItem = _mealsPageViewModel.ForecastDays.ElementAt(0);
                TodayList.ScrollIntoView(firstTodayItem, ScrollIntoViewAlignment.Leading);
                TodayList.UpdateLayout();
            }

            // forecast list of meals
            if (_mealsPageViewModel != null &&
                _mealsPageViewModel.ForecastDays != null &&
                _mealsPageViewModel.ForecastDays.Count > 0)
            {
                ForecastList.ClearValue(ListView.ItemsPanelProperty);
                DayViewModel firstForecastItem = _mealsPageViewModel.ForecastDays.ElementAt(0);
                ForecastList.ScrollIntoView(firstForecastItem, ScrollIntoViewAlignment.Leading);
                ForecastList.UpdateLayout();
            }
        }

        /// <summary>
        /// Manuelles Ausloesen des Neu-Ladens der aktuellen Gerichte und anschließender Aktualisierung der Oberflaeche anstoßen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            prepareMealsPage(true);
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
                _isLastNavigationTargetSettingsPage = false;
                DateTime dateOfMeal = getDateTimeOfSelectedMeal(selectedMeal, _mealsPageViewModel);
                DetailPageParameter paramModel = new DetailPageParameter(dateOfMeal, selectedMeal);
                Frame mensaFrame = MainPage.Current.FindName("MensaFrame") as Frame;
                if (mensaFrame != null)
                    mensaFrame.Navigate(typeof(DetailPage), paramModel);

            }
        }

        /// <summary>
        /// Search the given mealsPageViewModel for the given mealViewModel and 
        /// delivers the datetime of dayViewModel the given mealViewModel belongs to.
        /// </summary>
        /// <param name="selectedMeal"></param>
        /// <param name="mealsPageViewModel"></param>
        /// <returns></returns>
        private DateTime getDateTimeOfSelectedMeal(MealViewModel selectedMeal, MealsPageViewModel mealsPageViewModel)
        {
            DateTime resultDateTime = DateTime.Now;
            ObservableCollection<DayViewModel> allDayViewModels = new ObservableCollection<DayViewModel>();

            // collect all dayViewModels
            foreach (DayViewModel todayDayViewModel in mealsPageViewModel.Today)
            {
                allDayViewModels.Add(todayDayViewModel);
            }
            foreach (DayViewModel forcastDayViewModel in mealsPageViewModel.ForecastDays)
            {
                allDayViewModels.Add(forcastDayViewModel);
            }

            // search all collected dayViewModels for the selected mealViewModel
            bool isMealDateFound = false;
            var allDaysIterator = allDayViewModels.GetEnumerator();
            while (allDaysIterator.MoveNext() && !isMealDateFound)
            {
                ObservableCollection<MealViewModel> allMealsOfDay = allDaysIterator.Current.Meals;
                var allMealsIterator = allMealsOfDay.GetEnumerator();
                while (allMealsIterator.MoveNext() && !isMealDateFound)
                {
                    if (selectedMeal.Equals(allMealsIterator.Current))
                    {
                        resultDateTime = allDaysIterator.Current.Date;
                        isMealDateFound = true;
                    }
                }
            }
            allDaysIterator.Dispose();
            return resultDateTime;
        }

        private void ImpressumAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            _isLastNavigationTargetSettingsPage = false;
            Frame mensaFrame = MainPage.Current.FindName("MensaFrame") as Frame;
            if (mensaFrame != null)
                mensaFrame.Navigate(typeof(ImpressumPage));
        }
    }
}
