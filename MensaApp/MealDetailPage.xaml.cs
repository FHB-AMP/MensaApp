using MensaApp.Common;
using MensaApp.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

    class listenEintrag
    {
        public string nameMeal { get; set; }
    }

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MealDetailPage : Page
    {

        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MealDetailPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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

        private async void GetJSON_Click(object sender, RoutedEventArgs e)
        {
            //GetServerData_HTTPClient(sender, e);
            await GetServerData("http://demo0299672.mockable.io/", "", "haha.txt");
            speichereWas();
        }

        private async void speichereWas()
        {
            // Helfer
            string data = "";

            try
            {
                // Hole den Standort der JSON Datei
                ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
                String dateiName = MensaRestApiResource.GetString("MealJSONFile");

                StorageFile sampleFile = await localFolder.GetFileAsync("haha.txt");

                // Abgleich mit dem heutigen Datum TODO Holger
                DateTimeOffset fileCreationDateOff = sampleFile.DateCreated;
                DateTime fileCreationDate = fileCreationDateOff.Date;

                // modified Zeitpunkt aus Properties holen
                var check = new List<string>();
                check.Add("System.DateModified");
                var props = await sampleFile.Properties.RetrievePropertiesAsync(check);
                var dateModified = props.SingleOrDefault().Value;

                // Property als DateTime parsen
                DateTime myDate = DateTime.ParseExact(dateModified.ToString(), "dd.MM.yyyy HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);

                data = await FileIO.ReadTextAsync(sampleFile);

                Result.Text = myDate.Date.ToString();
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }

           
        }

        public async Task<string> GetServerData(string serviceURI, string serviceURL, string dateiName)
        {
            string data = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serviceURI);
                    string url = serviceURL;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        // Hole JSON-File
                        data = await response.Content.ReadAsStringAsync();

                        // Write data to a file
                        StorageFile sampleFile = await localFolder.CreateFileAsync(dateiName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteTextAsync(sampleFile, data);

                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Holger Fehlerbehandlung einbauen
            }

            return data;

        }

        private async void GetServerData_HTTPClient(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    /// TODO Holger wenn Jano seine PHP-Skripte angepasst hat hier einfuegen und alten entfernen
                    //client.BaseAddress = new Uri("https://mobile-quality-research.org");
                    //var url = "/services/meals/";

                    client.BaseAddress = new Uri("http://demo2108727.mockable.io/");
                    var url = "";

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        // Hole JSON-File
                        var data = response.Content.ReadAsStringAsync();

                        // String Ueberpruefung  falls nix passiert TODO Holger vor Abgabe entfernen
                        //Result.Text = data.Result;

                        // JSON-File in Objekte verwandeln
                        var rootObject = JsonConvert.DeserializeObject<RootObjectDays>(data.Result);

                        // Neue Listen fuer den heutigen Tag erstellen
                        var liste = new List<listenEintrag>();

                        // heutige Tag ohne Zeit
                        DateTime today = DateTime.Today;

                        // Mahlzeit (Objekt) des heutigen Tages finden
                        for (int i = 0; i < rootObject.days.Count; i++)
                        {
                            // Cast von String zu DateTime
                            DateTime listenTag = zerlegeJSONDate(rootObject.days[i].date);

                            // finde heute
                            if (listenTag.Equals(today))
                            {
                                // so viele Meals wie es an dem Tag gibt hinzufuegen
                                for (int j = 0; j < rootObject.days[i].meals.Count; j++)
                                {
                                    // anzuzeigende Eintraege hinzufuegen
                                    liste.Add(new listenEintrag() { nameMeal = rootObject.days[i].meals[j].name });
                                }
                            }
                        }

                        // der Oberflaeche die Liste zur Verfuegung stellen
                        //LstServerData.ItemsSource = liste;

                    }
                }
            }
            catch (Exception ex)
            {
                  //TODO Holger Fehlerbehandlung einbauen
            }
        }


        private DateTime zerlegeJSONDate(string dateString)
        {
            // Trenner uebergeben
            string[] separators = {"-"};

            // String dateParts aus JSON-Objekt anhand der Trenner aufteilen und die Leerzeichen entfernen, wenn vorhanden
            string[] dateParts = dateString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Neues DateTime-Objekt erzeugen und die einzelnen Teile in in Integer parsen
            DateTime result = new DateTime(Int32.Parse(dateParts[0]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[2]));

            return result;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // heutige Tag ohne Zeit
            DateTime today = DateTime.Today;
            Result.Text = today.ToString();
        }

    }
}
