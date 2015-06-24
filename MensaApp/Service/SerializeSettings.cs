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
        internal async void serializeAdditives(ObservableCollection<ViewModel.AdditiveViewModel> observableCollection, string dateiname)
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

        internal async Task<ObservableCollection<ViewModel.AdditiveViewModel>> deserializeAdditives(string dateiname)
        {
            // Helfer
            ObservableCollection<ViewModel.AdditiveViewModel> observableCollection = new ObservableCollection<ViewModel.AdditiveViewModel>();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFile sampleFile = await localFolder.GetFileAsync(dateiname);

                var data = await FileIO.ReadTextAsync(sampleFile);

                // JSON-File in Objekte verwandeln
                observableCollection = JsonConvert.DeserializeObject<ObservableCollection<ViewModel.AdditiveViewModel>>(data);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }
            
            return observableCollection;
        }

        internal async void serializeAllergenes(ObservableCollection<ViewModel.AllergenViewModel> observableCollection, string dateiname)
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

        internal async Task<ObservableCollection<ViewModel.AllergenViewModel>> deserializeAllergenes(string dateiname)
        {
            // Helfer
            ObservableCollection<ViewModel.AllergenViewModel> observableCollection = new ObservableCollection<ViewModel.AllergenViewModel>();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFile sampleFile = await localFolder.GetFileAsync(dateiname);

                var data = await FileIO.ReadTextAsync(sampleFile);

                // JSON-File in Objekte verwandeln
                observableCollection = JsonConvert.DeserializeObject<ObservableCollection<ViewModel.AllergenViewModel>>(data);
            }
            catch (Exception)
            {
                // TODO Holger Fang etwas
            }

            return observableCollection;
        }
    }
}
