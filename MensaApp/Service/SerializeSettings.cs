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
using Windows.Storage.Streams;

namespace MensaApp.Service
{
    class SerializeSettings
    {
        internal async void serializeAdditives(ObservableCollection<AdditiveViewModel> observableCollection, string dateiname)
        {
            // Serialize our Product class into a string             
            string jsonContents = JsonConvert.SerializeObject(observableCollection);

            // Get the app data folder and create or replace the file we are storing the JSON in.            
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile textFile = await localFolder.CreateFileAsync(dateiname, CreationCollisionOption.ReplaceExisting);

            // Open the file...      
            using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // write the JSON string!
                using (DataWriter textWriter = new DataWriter(textStream))
                {
                    textWriter.WriteString(jsonContents);
                    await textWriter.StoreAsync();
                }
            }
        }

        internal async Task<ObservableCollection<AdditiveViewModel>> deserializeAdditives(string dateiname)
        {
            // Helfer
            ObservableCollection<AdditiveViewModel> observableCollection = new ObservableCollection<AdditiveViewModel>();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFile sampleFile = await localFolder.GetFileAsync(dateiname);

                var data = await FileIO.ReadTextAsync(sampleFile);

                // JSON-File in Objekte verwandeln
                observableCollection = JsonConvert.DeserializeObject<ObservableCollection<AdditiveViewModel>>(data);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }
            
            return observableCollection;
        }

        internal async void serializeAllergenes(ObservableCollection<AllergenViewModel> observableCollection, string dateiname)
        {
            // Serialize our Product class into a string             
            string jsonContents = JsonConvert.SerializeObject(observableCollection);

            // Get the app data folder and create or replace the file we are storing the JSON in.            
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile textFile = await localFolder.CreateFileAsync(dateiname, CreationCollisionOption.ReplaceExisting);

            // Open the file...      
            using (IRandomAccessStream textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // write the JSON string!
                using (DataWriter textWriter = new DataWriter(textStream))
                {
                    textWriter.WriteString(jsonContents);
                    await textWriter.StoreAsync();
                }
            }
        }

        internal async Task<ObservableCollection<AllergenViewModel>> deserializeAllergenes(string dateiname)
        {
            // Helfer
            ObservableCollection<AllergenViewModel> observableCollection = new ObservableCollection<AllergenViewModel>();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFile sampleFile = await localFolder.GetFileAsync(dateiname);

                var data = await FileIO.ReadTextAsync(sampleFile);

                // JSON-File in Objekte verwandeln
                observableCollection = JsonConvert.DeserializeObject<ObservableCollection<AllergenViewModel>>(data);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }

            return observableCollection;
        }
    }
}
