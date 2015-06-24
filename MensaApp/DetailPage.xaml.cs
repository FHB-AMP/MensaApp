using MensaApp.Common;
using MensaApp.DataModel;
using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class DetailPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MealDetailPageViewModel _mealDetailPageViewModel = new MealDetailPageViewModel();

        public DetailPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void populateTodayDetail() 
        {
            int mealNumber = 2;
            string name = "Seelachs in Sesampanade mit jungen Erbsen, dazu Grillkartoffeln oder Pommes frites";
            bool isSuitableMeal = false;
            bool isSuitableNutrition = true;
            bool isSuitableAdditives = false;
            bool isSuitableAllergens = true;

            // Schwein und Rind
            ObservableCollection<string> symbols = new ObservableCollection<string>();
            symbols.Add("mit Schweinefleisch");
            symbols.Add("mit Rindfleisch");

            // (2)(3)(8)
            ObservableCollection<AdditiveViewModel> additives = new ObservableCollection<AdditiveViewModel>();
            additives.Add(new AdditiveViewModel("(2)", "mit Konservierungsstoff", "Erhaltung bzw. Verlängerung der Genusstauglichkeit des Lebensmittels.", false));
            additives.Add(new AdditiveViewModel("(3)", "mit Antioxidationsmittel", "wie (1) und (2)", false));
            additives.Add(new AdditiveViewModel("(8)", "mit Phosphat", "Bestandteil des Erbgutes aller Lebewesen und ist in Lebensmitteln tierischen Ursprungs enthalten. Phosphatverbindungen werden u.a. als Säuerungsmittel in Cola, Wurstwaren eingesetzt", true));

            // (A)(G)(C)(F)(I)
            ObservableCollection<AllergenViewModel> allergens = new ObservableCollection<AllergenViewModel>();
            allergens.Add(new AllergenViewModel("(A)", "Gluten ist das Klebereiweiß in den Getreidesorten Weizen, Dinkel, Roggen, Gerste Hafer und Kamut", "Saucen, panierte Speisen, Puddings, Bulgur, Couscous, Grießspeisen, Backwaren, Saitan, verzehrfertige Joghurt-und Quarkspeisen, Feinkostsalate, Wurstwaren, Schimmel- und Schmelzkäse", false));
            allergens.Add(new AllergenViewModel("(C)", "Eier", "Mayonnaisen, Remouladen, Teigwaren (Tortellini, Spätzle, Schupfnudeln), Gnocchi, Backwaren, Panaden, geklärte und gebundene Suppen", false));
            allergens.Add(new AllergenViewModel("(F)", "Soja", "Milch- und Sahneersatz auf Sojabasis, Tofu, Sojasauce, Zusatzstoff in Süsswaren v.a. in Schokolade, Wurst- und Fleischwaren", false));
            allergens.Add(new AllergenViewModel("(G)", "Milch", "Backwaren, vegetarische Bratlinge, Wurstwaren, Dressings und Würzsaucen", false));
            allergens.Add(new AllergenViewModel("(I)", "Sellerie", "Gewürzmischungen, Salatsaucenbasis, Instant-Brühen, Fleischwaren, Ketchup, Bratlinge", false));
            
            MealViewModel mealVM = new MealViewModel(mealNumber, name, symbols, additives, allergens, isSuitableMeal, isSuitableNutrition, isSuitableAdditives, isSuitableAllergens);

            _mealDetailPageViewModel.Date = DateTime.Now;
            _mealDetailPageViewModel.Meal = mealVM;
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

        public MealDetailPageViewModel MealDetailPageViewModel
        {
            get { return this._mealDetailPageViewModel; }
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
            if (e.NavigationParameter != null)
            {
                DetailPageParamModel paramModel = e.NavigationParameter as DetailPageParamModel;
                _mealDetailPageViewModel.Date = paramModel.date;
                _mealDetailPageViewModel.Meal = paramModel.meal;
            }
            else
            {
                populateTodayDetail();
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
    }
}
