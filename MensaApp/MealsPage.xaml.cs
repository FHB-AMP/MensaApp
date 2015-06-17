﻿using MensaApp.Common;
using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class MealsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ForecastViewModel _mealsTodayViewModel = new ForecastViewModel();
        private ForecastViewModel _mealsForecastViewModel = new ForecastViewModel();

        public MealsPage()
        {
            this.InitializeComponent();

            mealsToday.Source = _mealsTodayViewModel.Days;
            mealsForecast.Source = _mealsForecastViewModel.Days;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

        }

        private void populateTodayWithMeals()
        {
            DayViewModel dayVM = new DayViewModel();
            dayVM.Date = DateTime.Now;
            dayVM.Meals.Add(new MealViewModel(1, "Italienisches Nudelgericht mit Salami- oder Tofustreifen, dazu Reibekäse"));
            dayVM.Meals.Add(new MealViewModel(2, "Rostbratwurst mit Pommes frites und buntem Salatmix"));
            dayVM.Meals.Add(new MealViewModel(3, "Seefischfilet, gebraten mit Dillsauce Salzkartoffeln oder Reis dazu frischer Gurkensalat"));
            dayVM.Meals.Add(new MealViewModel(4, "Kartoffel-Spargel-Auflauf,dazu Saisonsalat mit Sonnenblumenkernen"));
            dayVM.Meals.Add(new MealViewModel(5, "Wiener Schnitzel mit lauwarmen Kartoffel - Gurkensalat und kleiner Salatgarnitur"));
            _mealsTodayViewModel.Days.Add(dayVM);
        }

        private void populateForecastDaysWithMeals()
        {
            DayViewModel dayVM1 = new DayViewModel();
            dayVM1.Date = DateTime.Now.AddDays(1);
            dayVM1.Meals.Add(new MealViewModel(1, "Märkische Kartoffelsuppe mit Wiener Würstchen oder Sojawürfeln (vegan), dazu Roggenbrot"));
            dayVM1.Meals.Add(new MealViewModel(2, "Pfannengyros mit Tzatziki und Langkornreis, dazu bunter Weißkraut-Möhrensalat"));
            dayVM1.Meals.Add(new MealViewModel(3, "Gegrillte Hähnchenkeule mit Letscho und gebackenen Kartoffelecken"));
            dayVM1.Meals.Add(new MealViewModel(4, "Brokkoli, Blumenkohl und Kartoffeln mit Gorgonzola gratiniert, dazu roter Linsensalat"));
            _mealsForecastViewModel.Days.Add(dayVM1);

            DayViewModel dayVM2 = new DayViewModel();
            dayVM2.Date = DateTime.Now.AddDays(2);
            dayVM2.Meals.Add(new MealViewModel(1, "Panierte Jagdwurst oder Tofusteak mit Nudeln und veganer Tomatensauce"));
            dayVM2.Meals.Add(new MealViewModel(2, "Rügener Rauch-Matjestopf mit Preiselbeerrahm, dazu Stangenbohnen und Bratkartoffeln"));
            dayVM2.Meals.Add(new MealViewModel(3, "Kalbsschnitzel mit lauwarmen Kartoffelsalat und Blattsalat"));
            dayVM2.Meals.Add(new MealViewModel(4, "Lasagne mit Sojabolognaise und Salatmix"));
            _mealsForecastViewModel.Days.Add(dayVM2);

            DayViewModel dayVM3 = new DayViewModel();
            dayVM3.Date = DateTime.Now.AddDays(5);
            dayVM3.Meals.Add(new MealViewModel(1, "Kräuterquark mit Leinöl und Salzkartoffeln"));
            dayVM3.Meals.Add(new MealViewModel(2, "Leberkäse mit süßem Senf und Bratkartoffeln, Gewürzgurke"));
            dayVM3.Meals.Add(new MealViewModel(3, "Hähnchen CordonBleu mit Sauce Bernaise und Brokkoli, dazu Kroketten oder Salzkartoffeln"));
            dayVM3.Meals.Add(new MealViewModel(4, "Sojagyros mit Tzatziki oder Ayvar und Couscous, dazu Weißkrautsalat"));
            _mealsForecastViewModel.Days.Add(dayVM3);
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
            populateTodayWithMeals();
            populateForecastDaysWithMeals();
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
    }
}
