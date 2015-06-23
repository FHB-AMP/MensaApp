using MensaApp.DataModel;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace MensaApp.Service
{

    class ServingMealOffer
    {
        // Abspeichern und Lesen des JSON-Files
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        /// <summary>
        /// Stelle die Mahlzeit-Daten fuer die Vorhersage bereit.
        /// </summary>
        /// <param name="forecast">wie viele Tage in der Zukunft moechte man haben</param>
        /// <returns>Liste aus DayViewModellen, fuer jeden Tag ein DayViewModel, es sei denn es ist Samstag oder Sonntag</returns>
        public async Task<List<DayViewModel>> GetServerDataForNextDays(int forecast)
        {
            // Helfer
            List<DayViewModel> liste = new List<DayViewModel>();

            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectDays>(data);

            for (int k = 1; k < forecast + 1; k++)
            {
                DayViewModel dayVM = new DayViewModel();
                // Fuege Datum hinzu
                dayVM.Date = DateTime.Today.AddDays(k);

                // heutige Tag ohne Zeit
                DateTime nextDay = DateTime.Today.AddDays(k);

                // Mahlzeit (Objekt) des gewuenschten Tages finden
                for (int i = 0; i < rootObject.days.Count; i++)
                {
                    // Cast von String zu DateTime
                    DateTime listenTag = zerlegeJSONDate(rootObject.days[i].date);

                    // finde den gewuenschten Tag
                    if (DateTime.Compare(listenTag, nextDay) == 0)
                    {
                        // so viele Meals wie es an dem Tag gibt hinzufuegen
                        for (int j = 0; j < rootObject.days[i].meals.Count; j++)
                        {
                            // anzuzeigende Eintraege hinzufuegen
                            dayVM.Meals.Add(new MealViewModel(rootObject.days[i].meals[j].mealNumber, rootObject.days[i].meals[j].name, true, true, true, true));
                        }
                    }
                }
                liste.Add(dayVM);
            }
            return liste;
        }

        /// <summary>
        /// Stellt die Mahlzeit-Daten fuer heute bereit. 
        /// </summary>
        /// <returns>DayViewModel fuer den heutigen Tag</returns>
        public async Task<DayViewModel> GetServerDataForToday()
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer
            DayViewModel dayVM = new DayViewModel();

            // Fuege Datum hinzu
            dayVM.Date = DateTime.Today;

            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectDays>(data);

            // heutige Tag ohne Zeit
            DateTime today = DateTime.Today;

            // es ist Samstag oder Sonntag
            Boolean saSo = true;

            // Mahlzeit (Objekt) des heutigen Tages finden
            for (int i = 0; i < rootObject.days.Count; i++)
            {
                // Cast von String zu DateTime
                DateTime listenTag = zerlegeJSONDate(rootObject.days[i].date);

                // finde heute
                if (DateTime.Compare(listenTag, today) == 0)
                {
                    // doch kein Samstag oder Sonntag
                    saSo = false;

                    // so viele Meals wie es an dem Tag gibt hinzufuegen
                    for (int j = 0; j < rootObject.days[i].meals.Count; j++)
                    {
                        // anzuzeigende Eintraege hinzufuegen
                        dayVM.Meals.Add(new MealViewModel(rootObject.days[i].meals[j].mealNumber, rootObject.days[i].meals[j].name, true, true, true, true));

                        // Abgleich mit Settings fehlt noch komplett
                        //dayVM.Meals.Add(new MealViewModel(rootObject.days[i].meals[j].mealNumber, rootObject.days[i].meals[j].name, new ObservableCollection<string>(rootObject.days[i].meals[j].symbols),
                        //    new ObservableCollection<string>(rootObject.days[i].meals[j].additives),  new ObservableCollection<string>(rootObject.days[i].meals[j].allergens)));
                    }
                }
            }

            // wenn das heutige Datum mit keinem Listendatum passte dann ist Samstag, oder Sonntag 
            if (saSo == true)
            {
                // TODO Holger Was ist besser alles auf "false" oder alles auf "true"
                dayVM.Meals.Add(new MealViewModel(1, "Heute gibt es leider kein Angebot. Ihr Versorger.", false, false, false, false));
            }

            return dayVM;
        }

        /// <summary>
        /// Hole die JSON-Daten vom Server ab.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetServerData(string serviceURI, string serviceURL, string propertyName)
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

                        // Hole den Speicherort der JSON Datei
                        ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
                        String dateiName = MensaRestApiResource.GetString(propertyName);

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

        /// <summary>
        /// Lese das abgespeicherte JSON
        /// </summary>
        /// <returns>JSON als Zeichenkette</returns>
        private async Task<string> ReadSavedJSON()
        {
            // Helfer
            string data = "";

            try
            {
                // Hole den Standort der JSON Datei
                ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
                String dateiName = MensaRestApiResource.GetString("MealJSONFile");

                StorageFile sampleFile = await localFolder.GetFileAsync(dateiName);

                // Abgleich mit dem heutigen Datum 
                //System.DateTimeOffset fileCreationDate = sampleFile.DateCreated;

                data = await FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }

            return data;
        }

        /// <summary>
        /// Das aus der JSON-Datei eingelesene Datumsformat in DateTime umwandeln.
        /// </summary>
        /// <param name="dateString">Datum eines Tages durch Bindestriche "-" getrennt</param>
        /// <returns>DateTime des uebergebenen Strings</returns>
        private DateTime zerlegeJSONDate(string dateString)
        {
            // Trenner uebergeben
            string[] separators = { "-" };

            // String dateParts aus JSON-Objekt anhand der Trenner aufteilen und die Leerzeichen entfernen, wenn vorhanden
            string[] dateParts = dateString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Neues DateTime-Objekt erzeugen und die einzelnen Teile in in Integer parsen
            DateTime result = new DateTime(Int32.Parse(dateParts[0]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[2]));

            return result;
        }
    }
}