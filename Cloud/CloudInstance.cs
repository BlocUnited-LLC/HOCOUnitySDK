using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hoco.Runtime;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Hoco.Cloud
{
    /// <summary>
    /// The Core Logic for interacting with the MongoDb Cloud Database.
    /// </summary>
    /// <typeparam name="T">Any type of Serializable Data Class</typeparam>
    public class CloudInstance<T> where T : new()
    {

        /// <summary>
        /// A class that contains the data needed to interact with a collection via WebRequests
        /// </summary>
        [System.Serializable]
        private class LiveDataRequest
        {
            public string collectionName { get; set; }
            public string id { get; set; }
            public string json { get; set; }
        }

        public static async UniTask<T> Get(string filterProperty, string filterValue, string storageKey = "NULL")
        {
            string url = ConstructGetUrl(storageKey, filterProperty, filterValue);

            // Make the GET request. Assume GetRequest is an async method that fetches the data.
            string jsonResponse = await GetRequest(url);

            Debug.Log($"GetFilteredDataResponse: {jsonResponse}");

            try
            { // Deserialize the JSON response into type T. This assumes that T is a type that can be deserialized from the JSON response.
                var data = JsonConvert.DeserializeObject<T[]>(jsonResponse);
                if (data == null || data.Length == 0)
                {
                    Debug.Log("GetFilteredDataSuccess");
                    return new T();
                }
                else
                {
                    Debug.Log("GetFilteredDataSuccess");
                    return data.FirstOrDefault();
                }
                //Debug.Log("GetFilteredDataSuccess");
                //return data.FirstOrDefault();
            }
            catch (System.Exception e)
            { // In case of an exception, log the error and return a new existingInstance of T.
                Debug.LogError($"Error in GetFilteredData: {e.Message}");
                var data = JsonConvert.DeserializeObject<T>(jsonResponse);
                if (data == null)
                    return new T();
                else
                    return data;
            }
        }
        public static async UniTask<bool> Update<TInput>(TInput instanceData, string instanceId, string storageKey = "NULL") where TInput : class, new() //d Constraint to ensure TOutput is a class and has a parameterless constructor
        {
            if (string.IsNullOrEmpty(instanceId))
                return false;
            try
            {

                string url = ConstructUpdateUrl(storageKey);
                Dictionary<string, object> formattedJsonData = new Dictionary<string, object>() { { storageKey, instanceData } };//InstanceData should be a container class like PlayerData or CellData
                string formattedJsonDataString = JsonConvert.SerializeObject(formattedJsonData, Formatting.None);
                LiveDataRequest raw = new LiveDataRequest
                {
                    collectionName = storageKey,
                    id = instanceId,
                    //GetId(instanceData), 
                    json = formattedJsonDataString
                };
                string rawJson = JsonConvert.SerializeObject(raw, Formatting.None);
                Debug.Log(string.Format("\nSending Update Json:\n{0}\n\n{1}", url, rawJson));
                var response = await PutRequest(url, rawJson);

                try
                {
                    Debug.Log(string.Format("\nUpdateDataSuccess\n{0}\n", response));
                    //var updatedData = JsonConvert.DeserializeObject<TOutput>(response);
                    //return updatedData;
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in GetFilteredData: {e.Message}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            return false;
        }
        public static async UniTask<T> Update(LiveCloudData modifiedInstance, string storageKey = "NULL")
        {
            try
            {
                string url = ConstructUpdateUrl(storageKey);//k_FunctionUpdateCollectionURL;//FormatUrl(k_FunctionUpdateCollectionURL, k_PlayerDataCollectionName);//FormatUpdateUrl(k_PlayerDataCollectionName);
                Dictionary<string, object> formattedJsonData = new Dictionary<string, object>() { { storageKey, modifiedInstance } };
                string formattedJsonDataString = JsonConvert.SerializeObject(formattedJsonData, Formatting.None);
                LiveDataRequest raw = new LiveDataRequest { collectionName = storageKey, id = modifiedInstance.Id, json = formattedJsonDataString };
                string rawJson = JsonConvert.SerializeObject(raw, Formatting.None);
                Debug.Log(string.Format("Sending Json:\n{0}\n{1}", url, rawJson));
                var response = await PutRequest(url, rawJson);
                try
                { // Deserialize the JSON response into type T. This assumes that T is a type that can be deserialized from the JSON response.
                    var updatedData = JsonConvert.DeserializeObject<T>(response);
                    Debug.Log("GetFilteredDataSuccess");
                    return updatedData;
                }
                catch (System.Exception e)
                { // In case of an exception, log the error and return a new existingInstance of T.
                    Debug.LogError($"Error in GetFilteredData: {e.Message}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            return new T();
        }
        //private static string GetId<T>(T instance) where T : LiveCloudData
        //{
        //    // This method now only accepts T that is or derives from LiveCloudData.
        //    return instance?.Id ?? string.Empty;
        //}
   
        public static async UniTask<bool> Create(T newData, string storageKey = "NULL")
        {
            try
            {
                string url = ConstructCreateUrl(storageKey);
                Debug.Log(string.Format("{0}", url));
                //FormatUrl(k_FunctionCreateCollectionURL, k_PlayerDataCollectionName);//FormatCreateUrl(k_PlayerDataCollectionName);
                //var localPlayerData = new PlayerData { PlayerAddress = _playerAddress };
                Dictionary<string, object> formattedJsonData = new Dictionary<string, object>() { { storageKey, newData } };
                string formattedJsonDataString = JsonConvert.SerializeObject(formattedJsonData, Formatting.None);
                LiveDataRequest raw = new LiveDataRequest { collectionName = storageKey, json = formattedJsonDataString };
                string rawJson = JsonConvert.SerializeObject(raw, Formatting.None);
                Debug.Log(string.Format("{0}", JsonConvert.SerializeObject(raw, Formatting.Indented)));
                var response = await PostRequest(url, rawJson);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            return false;
        }
        public static async UniTask<bool> Delete(LiveCloudData existingInstance, string storageKey = "NULL")
        {
            try
            {
                string url = ConstructDeleteUrl(storageKey);//FormatUrl(k_FunctionDeleteCollectionURL, k_PlayerDataCollectionName);//FormatDeleteURL(k_PlayerDataCollectionName);
                //Dictionary<string, object> formattedJsonData = new Dictionary<string, object>() { { storageKey, playerData.PlayerData } };
                //string formattedJsonDataString = JsonConvert.SerializeObject(formattedJsonData, Formatting.None);
                LiveDataRequest raw = new LiveDataRequest { collectionName = storageKey, id = existingInstance.Id };
                string rawJson = JsonConvert.SerializeObject(raw, Formatting.None);
                var response = await DeleteRequest(url, rawJson);
                Debug.Log($"DELETION_SUCCESS {response}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("DELETION_FAIL");
                Debug.LogError(e.Message);
            }
            return false;
        }

        /// <summary>Used for React GetRequest requests.</summary>
        /// <param name="url">The Url for the Server Endpoint</param>
        /// <returns>The result or Code response.</returns>
        private static async UniTask<string> GetRequest(string url)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();
                return request.downloadHandler.text;
            }
        }

        /// <summary>Used for React Post requests</summary>
        /// <param name="url">The Url for the Server Endpoint.</param>
        /// <param name="data">The Json data to send to the server.</param>
        /// <returns>Code response.</returns>
        private static async UniTask<string> PostRequest(string url, string data)
        {
            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(data);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                await request.SendWebRequest();
                return request.downloadHandler.text;
            }
        }

        /// <summary>Used for React Put requests.</summary>
        /// <param name="url">The Url for the Server Endpoint.</param>
        /// <param name="data">The Json data to send to the server.</param>
        /// <returns>Code response.</returns>
        private static async UniTask<string> PutRequest(string url, string data)
        {
            using (var request = new UnityWebRequest(url, "PUT"))
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(data);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                await request.SendWebRequest();
                return request.downloadHandler.text;
            }
        }

        /// <summary>Used for React Delete requests.</summary>
        private static async UniTask<string> DeleteRequest(string url, string data)
        {
            using (var request = new UnityWebRequest(url, "DELETE"))
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(data);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                await request.SendWebRequest();
                return request.downloadHandler.text;
            }
        }

        /// <summary>Needed to Create a encoded URL that contained the params needed for filtering a collection. ex. https://mongorest.azurewebsites.net/api/MongoDB/GetFilteredData</summary>
        /// <returns>The URL to put in the <see cref="GetRequest(string)"/> task</returns>
        private static string ConstructGetUrl(string collectionName, string filterProperty, string filterValue)
        {
            string getUrl = CloudBaseSettings.GetURL;
            Dictionary<string, object> filterJson = new Dictionary<string, object>() { { filterProperty, filterValue } };
            string jsonFilterString = JsonConvert.SerializeObject(filterJson);// Serialize to JSON string
            Debug.Log($"GetFilter: {jsonFilterString}");
            string encodedJsonFilter = System.Uri.EscapeDataString(jsonFilterString);// URL-encode the filter string
            string finalUrl = $"{getUrl}?collectionName={collectionName}&filterJson={encodedJsonFilter}";// Append the encoded filter to the URL
            Debug.Log(finalUrl);
            return finalUrl;
        }
        private static string ConstructUpdateUrl(string collectionName)
        {
            return string.Format("{0}?collectionName={1}", CloudBaseSettings.UpdateURL, collectionName);
        }
        private static string ConstructCreateUrl(string _collectionName)
        {
            return string.Format("{0}?collectionName={1}", CloudBaseSettings.CreateURL, _collectionName);
        }
        private static string ConstructDeleteUrl(string collectionName)
        {
            return string.Format("{0}?collectionName={1}", CloudBaseSettings.DeleteURL, collectionName);
        }

    }
}