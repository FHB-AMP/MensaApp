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
        
        private string _settingsFilename;
        private string _mealsFilename;
        private string _descriptionFilename;
        private readonly int _totalNumberOfFiles = 3;

        public FileService()
        {
            _mapping = new SettingsMapping();

            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            _settingsFilename = MensaRestApiResource.GetString("SettingsFilename");
            _descriptionFilename = MensaRestApiResource.GetString("DescriptionFilename");
            _mealsFilename = MensaRestApiResource.GetString("MealsFilename");
        }

        /// <summary>
        /// Saves all descriptions to json file.
        /// </summary>
        /// <param name="listOfDescriptions"></param>
        internal void SaveListOfDescriptions(ListsOfDescriptions listOfDescriptions)
        {
            if (listOfDescriptions != null)
            {
                string jsonString = JsonConvert.SerializeObject(listOfDescriptions);
                SaveJsonStringToFile(_descriptionFilename, jsonString);
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

            if (jsonString != null && jsonString.Length < 0)
            {
                description = JsonConvert.DeserializeObject<ListsOfDescriptions>(jsonString);
            }
            return description;
        }

        /// <summary>
        /// Saves all settings to json file.
        /// </summary>
        /// <param name="listOfSettings"></param>
        internal void SaveListOfSettings(ListsOfSettings listOfSettings)
        {
            if (listOfSettings != null)
            {
                string jsonString = JsonConvert.SerializeObject(listOfSettings);
                SaveJsonStringToFile(_settingsFilename, jsonString);
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

            if (jsonString != null && jsonString.Length > 0)
            {
                settings = JsonConvert.DeserializeObject<ListsOfSettings>(jsonString);
            }
            return settings;
        }

        /// <summary>
        /// Saves list of days from file.
        /// </summary>
        /// <param name="listsOfDays"></param>
        internal void SaveListOfDays(ListOfDays listsOfDays)
        {
            if (listsOfDays != null)
            {
                string jsonString = JsonConvert.SerializeObject(listsOfDays);
                SaveJsonStringToFile(_mealsFilename, jsonString);
            }
        }

        /// <summary>
        /// loads list of days from file.
        /// </summary>
        /// <returns></returns>
        internal async Task<ListOfDays> LoadListOfDaysAsync()
        {
            ListOfDays listOfDays = new ListOfDays();
            string jsonString = await LoadJsonStringFromFile(_mealsFilename);

            if (jsonString != null && jsonString.Length < 0)
            {
                listOfDays = JsonConvert.DeserializeObject<ListOfDays>(jsonString);
            }
            return listOfDays;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveToFile /////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async void SaveJsonStringToFile(string filename, string jsonString)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    StorageFile file = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(jsonString);
                            await textWriter.StoreAsync();
                        }
                    }
                }
            }
            catch(FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// LoadFromFile ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async Task<string> LoadJsonStringFromFile(string filename)
        {
            string jsonString = null;

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;                
                if (localFolder != null)
                {
                    IReadOnlyList<StorageFile> files = await localFolder.GetFilesAsync();
                    if (files != null)
                    {
                        IEnumerator<StorageFile> filesIterator = files.GetEnumerator();
                        while (filesIterator.MoveNext())
                        {
                            StorageFile file = filesIterator.Current;
                            if (file.Name.Equals(filename))
                            {
                                jsonString = await FileIO.ReadTextAsync(file);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }

            return jsonString;
        }

        /// <summary>
        /// Search all exsisting files and create missing files without content
        /// </summary>
        public async Task<int> CreateMissingFilesWithoutContentAsync()
        {
            int amountOfMissingFiles = 0;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            if (localFolder != null)
            {
                IReadOnlyList<StorageFile> files = await localFolder.GetFilesAsync();
                if (files != null && files.Count < _totalNumberOfFiles)
                {
                    List<string> filenames = new List<string>();
                    filenames.Add(_mealsFilename);
                    filenames.Add(_descriptionFilename);
                    filenames.Add(_settingsFilename);

                    foreach (string filename in filenames)
                    {
                        bool isExistingFilename = false;
                        IEnumerator<StorageFile> filesIterator = files.GetEnumerator();
                        while (filesIterator.MoveNext())
                        {
                            StorageFile file = filesIterator.Current;
                            if (file.Name.Equals(filename))
                            {
                                isExistingFilename = true;
                            }
                        }
                        if (!isExistingFilename)
                        {
                            amountOfMissingFiles++;
                            // if file not existing, then create a new empty file.
                            SaveJsonStringToFile(filename, "");
                        }
                    }
                }
            }
            return amountOfMissingFiles;
        }


    }
}
