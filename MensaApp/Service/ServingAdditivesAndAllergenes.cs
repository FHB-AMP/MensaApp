using MensaApp.DataModel;
using MensaApp.DataModel.Rest;
using MensaApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<List<ViewModel.NutritionViewModel>> GetNutritions(ObservableCollection<AdditiveViewModel> additiveVMCollection, ObservableCollection<AllergenViewModel> allergenVMCollection)
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer
            List<NutritionViewModel> listeNutritionVM = new List<NutritionViewModel>();

            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<ListsOfDescriptions>(data);

            // aus Rootobject die Ernaehrungsweisen ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (NutritionDescription nutritionDescription in rootObject.nutritions)
            {
                NutritionViewModel nutritionVM = new NutritionViewModel();

                nutritionVM.Id = nutritionDescription.id;
                nutritionVM.Name = nutritionDescription.name;
                nutritionVM.Definition = nutritionDescription.definition;

                // ########### Zusatzstoffe-ViewModels ###########

                // Durchlaufe alle exkludierten Zusatzstoffe der Ernaehrungsweise vom Rest-Service
                foreach (String restAdditive in nutritionDescription.excludedAdditives)
                {
                    // Durchlaufe alle uebergebenen Zusatzstoffe-ViewModels
                    foreach (AdditiveViewModel additivVM in additiveVMCollection)
                    {
                        // Finde das passende ViewModel zur Id aus dem Rest-Service
                        if (restAdditive.Equals(additivVM.Id))
                        {
                            // Fuege das gefundene Zusatzstoff-ViewModel hinzu
                            nutritionVM.ExcludedAdditives.Add(additivVM);
                            // Verlasse die innere Schleife
                            break;
                        }
                    }
                }

                // ########### Allergene-ViewModels ###########

                // Durchlaufe alle exkludierten Allergene der Ernaehrungsweise vom Rest-Service
                foreach (String restAllergen in nutritionDescription.excludedAllergens)
                {
                    // Durchlaufe alle uebergebenen Zusatzstoffe-ViewModels
                    foreach (AllergenViewModel allergenVM in allergenVMCollection)
                    {
                        // Finde das passende ViewModel zur Id aus dem Rest-Service
                        if (restAllergen.Equals(allergenVM.Id))
                        {
                            // Fuege das gefundene Zusatzstoff-ViewModel hinzu
                            nutritionVM.ExcludedAllergens.Add(allergenVM);
                            // Verlasse die innere Schleife
                            break;
                        }
                    }
                }

                listeNutritionVM.Add(nutritionVM);
            }

            return listeNutritionVM;
        }

        public async Task<List<ViewModel.AdditiveViewModel>> GetAdditives()
        {
            // Lese das gespeicherte JSON
            string data = await ReadSavedJSON();

            // Helfer
            List<AdditiveViewModel> listeZusatzstoffe = new List<AdditiveViewModel>();
            
            // JSON-File in Objekte verwandeln
            var rootObject = JsonConvert.DeserializeObject<ListsOfDescriptions>(data);

            // aus Rootobject die Zusatzstoffe ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (AdditiveDescription addi in rootObject.additives)
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
            var rootObject = JsonConvert.DeserializeObject<ListsOfDescriptions>(data);

            // aus Rootobject die Zusatzstoffe ins Entity packen und im Anschluss der Liste hinzufügen
            foreach (AllergenDescription allerg in rootObject.allergens)
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
                String dateiName = MensaRestApiResource.GetString("DescriptionsServerJSONFile");

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
