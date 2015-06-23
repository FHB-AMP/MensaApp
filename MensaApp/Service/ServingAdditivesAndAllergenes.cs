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
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public async Task<List<ViewModel.AdditiveViewModel>> GetAdditives()
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer 1 und 2
            List<AdditiveViewModel> listeZusatzstoffe = new List<AdditiveViewModel>();
            AdditiveViewModel additiveVM = new AdditiveViewModel();

            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<RootObjectAdditives>(data);

            // aus Rootobject die Zusatzstoffe ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (Additive addi in rootObject.additives)
            {
                additiveVM.Id = rootObject.additives[0].id;
                additiveVM.Meaning = rootObject.additives[0].meaning;
                additiveVM.Definition = rootObject.additives[0].definition;
                listeZusatzstoffe.Add(additiveVM);
            }

            return listeZusatzstoffe;
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
