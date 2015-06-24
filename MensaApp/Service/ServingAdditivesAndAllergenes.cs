using MensaApp.DataModel;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace MensaApp.Service
{
    class ServingAdditivesAndAllergenes
    {
        // Abspeichern und Lesen des JSON-Files
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public async Task<List<ViewModel.AdditiveViewModel>> GetAdditives()
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer
            List<AdditiveViewModel> listeZusatzstoffe = new List<AdditiveViewModel>();
            
            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectAdditives>(data);

            // aus Rootobject die Zusatzstoffe ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (Additive addi in rootObject.additives)
            {
                AdditiveViewModel additiveVM = new AdditiveViewModel();
                additiveVM.Id = addi.id;
                additiveVM.Meaning = addi.meaning;
                additiveVM.Definition = addi.definition;
                listeZusatzstoffe.Add(additiveVM);
            }

            return listeZusatzstoffe;
        }

        public async Task<List<ViewModel.AllergenViewModel>> GetAllergenes()
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer
            List<AllergenViewModel> listeAllergene = new List<AllergenViewModel>();

            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectAdditives>(data);

            // aus Rootobject die Zusatzstoffe ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (Allergen allerg in rootObject.allergens)
            {
                AllergenViewModel allergenVM = new AllergenViewModel();
                allergenVM.Id = allerg.id;
                allergenVM.Definition = allerg.definition;
                allergenVM.ContainedIn = allerg.containedIn;

                listeAllergene.Add(allergenVM);
            }

            return listeAllergene;
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
                String dateiName = MensaRestApiResource.GetString("AdditivesJSONFile");

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
    }
}
