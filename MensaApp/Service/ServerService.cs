using MensaApp.DataModel.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace MensaApp.Service
{
    class ServerService
    {
        private string _mealBaseUrl;
        private string _mealPathUrl;
        private string _descriptionsBaseURL;
        private string _descriptionsPathURL;

        public ServerService()
        {
            ResourceLoader MensaRestApiResource = ResourceLoader.GetForCurrentView("MensaRestApi");
            _mealBaseUrl = MensaRestApiResource.GetString("MealBaseURL");
            _mealPathUrl = MensaRestApiResource.GetString("MealURL");
            _descriptionsBaseURL = MensaRestApiResource.GetString("DescriptionsBaseURL");
            _descriptionsPathURL = MensaRestApiResource.GetString("DescriptionsURL");
        }

        /// <summary>
        /// Get ListsOfDescriptions from server;
        /// </summary>
        /// <returns></returns>
        public async Task<ListsOfDescriptions> GetListsOfDescriptionsFromServerAsync()
        {
            ListsOfDescriptions listsOfDescriptions = null;

            // Get request to the server.
            string listsOfDescriptionsJsonString = await GetDataFromServerAsync(_descriptionsBaseURL, _descriptionsPathURL);

            if (listsOfDescriptionsJsonString != null && listsOfDescriptionsJsonString.Length > 0)
            {
                listsOfDescriptions = JsonConvert.DeserializeObject<ListsOfDescriptions>(listsOfDescriptionsJsonString);
            }
            return listsOfDescriptions;
        }

        /// <summary>
        /// Get ListOfDays from server;
        /// </summary>
        /// <returns></returns>
        public async Task<ListOfDays> GetDaysWithMealsFromServerAsync()
        {
            ListOfDays listOfDays = null;
            
            // Get request to the server.
            string listOfDaysJsonString = await GetDataFromServerAsync(_mealBaseUrl, _mealPathUrl);

            if (listOfDaysJsonString != null && listOfDaysJsonString.Length > 0)
            {
                listOfDays = JsonConvert.DeserializeObject<ListOfDays>(listOfDaysJsonString);
            }
            return listOfDays;
        }

        /// <summary>
        /// Hole die JSON-Daten vom Server ab.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDataFromServerAsync(string serviceURI, string serviceURL)
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
                    }
                }
            }
            catch (ArgumentNullException)
            {
                Debug.WriteLine("[ServerService.GetDataFromServer] HTML Get Request Failure");
            }
            return data;
        }
    }
}
