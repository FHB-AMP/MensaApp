using MensaApp.Common;
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
    public sealed partial class SettingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private SettingViewModel _settingViewModel = new SettingViewModel();

        public SettingPage()
        {
            this.InitializeComponent();

            additives.Source = _settingViewModel.Additives;
            allergens.Source = _settingViewModel.Allergens;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public void populateAdditives()
        {
            _settingViewModel.Additives.Add(new AdditiveViewModel("(1)", "mit Farbstoff", "Optische Aufwertung der wertbestimmenden Zutaten (z.B. höherer Fruchtanteil in der Kaltschale).", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(2)", "mit Konservierungsstoff", "Erhaltung bzw. Verlängerung der Genusstauglichkeit des Lebensmittels.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(3)", "mit Antioxidationsmittel", "wie (1) und (2)", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(4)", "mit Geschmacksverstärker", "zur Verstärkung des Geschmacks der wertbestimmenden Zutaten", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(5)", "geschwefelt", "Schwefel dient der Abtötung von unerwünschten Mikroorganismen", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(6)", "geschwärzt", "Schwärzung erfolgt durch Eisenoxide. Zur Färbung grüner Oliven.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(7)", "gewachst", "Überzugsmittel der Fruchtschale von Zitrusfrüchten zur Beeinflussung der Haltbarkeit.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(8)", "mit Phosphat", "Bestandteil des Erbgutes aller Lebewesen und ist in Lebensmitteln tierischen Ursprungs enthalten. Phosphatverbindungen werden u.a. als Säuerungsmittel in Cola, Wurstwaren eingesetzt", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(9)", "mit Süßungsmittel", "Süßstoffe, liefern kaum Nahrungsenergie und werden deshalb u.a. in energiereduzierten Lebensmitteln eingesetzt", true));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(11)", "mit Aspartam-Acesulfamsalz (eingesetzt enthält eine Phenylalaninquelle)", "Wird als Süßungsmittel oder Geschmacksverstärker eingesetzt. Es geht im Stoffwechsel des Körpers ein. Der Eiweißbaustein Phenylalanin führt bei Personen, die an Phenylketourie leiden zu schweren Gesundheitsschäden.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(13)", "mit Milcheiweiß", "", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(14)", "mit Eiklar", "Einsatz von Fremdeiweiß, wird als Bindemittel verwendet.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(20)", "chininhaltig", "Bitteraroma in Erfrischungsgetränken wie Tonic-Wasser.", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(21)", "mit Koffein", "Aroma-gebende Komponente", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(22)", "mit Milchpulver", "", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(23)", "mit Molkenpulver", "", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(KF)", "mit kakaohaltiger Fettglasur", "", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(TL)", "enthält tierisches Lab", "", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(AL)", "mit Alkohol", "Aroma-gebende Komponente", false));
            _settingViewModel.Additives.Add(new AdditiveViewModel("(GE)", "mit Gelatine", "", false));
        }

        public void populateAllergens()
        {
            _settingViewModel.Allergens.Add(new AllergenViewModel("(A)", "Gluten ist das Klebereiweiß in den Getreidesorten Weizen, Dinkel, Roggen, Gerste Hafer und Kamut", "Saucen, panierte Speisen, Puddings, Bulgur, Couscous, Grießspeisen, Backwaren, Saitan, verzehrfertige Joghurt-und Quarkspeisen, Feinkostsalate, Wurstwaren, Schimmel- und Schmelzkäse", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(B)", "Krebstiere sind Garnelen, Hummer, Fluss-und Taschenkrebse, Krabben", "Feinkostsalate, Paella, Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(C)", "Eier", "Mayonnaisen, Remouladen, Teigwaren (Tortellini, Spätzle, Schupfnudeln), Gnocchi, Backwaren, Panaden, geklärte und gebundene Suppen", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(D)", "Fisch", "Paella, Bouillabaise, Worchester Sauce, asiatische Würzpasten", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(E)", "Erdnüsse", "Frühstücksflocken, Backwaren, Süßspeisen- und Aufstriche, Würzsaucen, Gemüsebratlinge", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(F)", "Soja", "Milch- und Sahneersatz auf Sojabasis, Tofu, Sojasauce, Zusatzstoff in Süsswaren v.a. in Schokolade, Wurst- und Fleischwaren", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(G)", "Milch", "Backwaren, vegetarische Bratlinge, Wurstwaren, Dressings und Würzsaucen", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(H)", "Schalenfrüchte sind Mandeln, Hasel-, Wal-, Cashew-, Pecan-, Para- und Macadamianüsse, Pistazien", "Marzipan, Nougat, Aufstriche, Back-, Wurstwaren, Pesto, Feinkostsalate, vegetarische Bratlinge", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(I)", "Sellerie", "Gewürzmischungen, Salatsaucenbasis, Instant-Brühen, Fleischwaren, Ketchup, Bratlinge", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(J)", "Senf", "Gesäuerte Gemüse, Chutneys, Dressings, Wurstwaren, Bratlinge", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(K)", "Sesam", "Backwaren, Frühstückscerealien, Brotaufstriche", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(L)", "Schwefeldioxid, Sulfite", "Wein, weinhaltige Getränke, getrocknete Früchte Convenience-Produkte (z.B. Bratkartoffel, Instant-Kartoffelpüree), Konserven", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(M)", "Lupine", "Vegetarische Convenience-Produkte, regenerierfertige Backwaren", false));
            _settingViewModel.Allergens.Add(new AllergenViewModel("(N)", "Weichtiere sind Schnecken, Muscheln, Austern und Tintenfische", "Fisch- und Feinkostsalate, Paella und Bouillabaise, asiatische Suppen, Saucen und Würzmischungen", false));
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
            populateAdditives(); 
            populateAllergens();
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

        }

    }
}
