using MensaApp.ViewModel;
using MensaApp.DataModel;
using MensaApp.DataModel.Rest;
using MensaApp.DataModel.Setting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Diagnostics;

namespace MensaApp.Service
{
    public class FileService
    {
        private SettingsMapping _mapping;

        private StorageFolder _localFolder;
        private string _settingsFilename;
        private string _mealsFilename;
        private string _descriptionFilename;

        public FileService()
        {
            _mapping = new SettingsMapping();

            _localFolder = ApplicationData.Current.LocalFolder;

            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            _settingsFilename = MensaRestApiResource.GetString("SettingsFilename");
            _descriptionFilename = MensaRestApiResource.GetString("DescriptionFilename");
            _mealsFilename = MensaRestApiResource.GetString("MealsFilename");
        }

        /// <summary>
        /// Saves all descriptions to json file.
        /// </summary>
        /// <param name="listOfDescriptions"></param>
        internal async void SaveListOfDescriptionsAsync(ListsOfDescriptions listOfDescriptions)
        {
            if (listOfDescriptions != null)
            {
                string jsonString = JsonConvert.SerializeObject(listOfDescriptions);
                await SaveJsonStringToFile(_descriptionFilename, jsonString);
            }
        }

        /// <summary>
        /// Loads all descriptions form json file.
        /// </summary>
        /// <returns></returns>
        internal async Task<ListsOfDescriptions> LoadListOfDescriptionsAsync()
        {
            ListsOfDescriptions description = new ListsOfDescriptions();
            string jsonString = await LoadJsonStringFromFile(_descriptionFilename);

            if (jsonString != null)
            {
                description = JsonConvert.DeserializeObject<ListsOfDescriptions>(jsonString);
            }
            return description;
        }

        /// <summary>
        /// Saves all settings to json file.
        /// </summary>
        /// <param name="listOfSettings"></param>
        internal async void SaveListOfSettingsAsync(ListsOfSettings listOfSettings)
        {
            if (listOfSettings != null)
            {
                string jsonString = JsonConvert.SerializeObject(listOfSettings);
                await SaveJsonStringToFile(_settingsFilename, jsonString);
            }
        }

        /// <summary>
        /// Loads all settings form json file.
        /// </summary>
        /// <returns></returns>
        internal async Task<ListsOfSettings> LoadListOfSettingsAsync()
        {
            ListsOfSettings settings = new ListsOfSettings();
            string jsonString = await LoadJsonStringFromFile(_settingsFilename);

            if (jsonString != null)
            {
                settings = JsonConvert.DeserializeObject<ListsOfSettings>(jsonString);
            }
            return settings;
        }
        
        private async Task SaveJsonStringToFile(string filename, string jsonString)
        {
            try
            {
                StorageFile file = await _localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (DataWriter textWriter = new DataWriter(textStream))
                    {
                        textWriter.WriteString(jsonString);
                        await textWriter.StoreAsync();
                        textWriter.Dispose();
                    }
                    textStream.Dispose();
                }
            }
            catch(FileNotFoundException ex)
            {
                Debug.WriteLine("Konnte Datei " + filename + " nicht abspeichern!");
            }
        }


        private async Task<string> LoadJsonStringFromFile(string filename)
        {
            string jsonString = null;

            try
            {
                StorageFile file = await _localFolder.GetFileAsync(filename);
                if (file != null)
                {
                    jsonString = await FileIO.ReadTextAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService] Datei: {0} konnte nicht gefunden werden", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService] Datei: {0} konnte nicht zugegriffen werden", filename);
            }

            return jsonString;
        }


        internal async void SaveListOfDaysAsync(ListOfDays listsOfDays)
        {
            if (listsOfDays != null)
            {
                string jsonString = JsonConvert.SerializeObject(listsOfDays);
                await SaveJsonStringToFile(_mealsFilename, jsonString);
            }
        }

        internal async Task<ListOfDays> LoadListOfDaysAsync()
        {
            ListOfDays listOfDays = new ListOfDays();
            string jsonString = await LoadJsonStringFromFile(_mealsFilename);

            if (jsonString != null)
            {
                listOfDays = JsonConvert.DeserializeObject<ListOfDays>(jsonString);
            }
            return listOfDays;
        }
    }
}
