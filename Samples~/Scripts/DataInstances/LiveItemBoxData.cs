using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Hoco.Cloud;
namespace Hoco.Samples.Runtime
{
    public class LiveItemBoxData : LiveCloudData
    {
        private static readonly string k_storageKey = "TestItemBoxData";
        //private static readonly string k_filterByUId { get => string.Format("{0}.{1}", nameof(ItemBoxData), nameof(ItemBoxData.UId)); }//Cant do this yet
        private static readonly string k_filterByUId = $"{nameof(ItemBoxData)}.UId";
 
        //"TestItemBoxData.UId";
        public ItemBoxData? ItemBoxData { get; set; } = new ItemBoxData();
        public static async UniTask<LiveItemBoxData[]> GetAll()
        {
            return await CloudAPI<LiveItemBoxData>.GetAll(k_storageKey);
        }
        public static async UniTask<LiveItemBoxData> GetByUId(string UId)
        {
            return await Cloud.CloudAPI<LiveItemBoxData>.Get(k_filterByUId, UId, k_storageKey);
        }

        /// <summary>
        /// Does a GetOrCreateByAddress>Create>GetOrCreateByAddress call stack to ensure the LiveItemBoxData exists in the Cloud Database, otherwise it will create a new one.
        /// </summary>
        /// <param name="UId">Uniqe ID to get or create in the Cloud</param>
        /// <returns>The live data in the cloud</returns>
        public static async UniTask<LiveItemBoxData> GetOrCreateByUId(string uId)
        {
            var existingData = await Cloud.CloudAPI<LiveItemBoxData>.Get(k_filterByUId, uId, k_storageKey);
            if (string.IsNullOrEmpty(existingData.Id))
            {
                existingData.ItemBoxData = new ItemBoxData();
                existingData.ItemBoxData.UId = uId;
                bool success = await Create(existingData.ItemBoxData);
                if (success)
                    return await GetByUId(uId);
                else
                    Debug.LogError("Failed to Create ItemBoxData");
            }
            else
            {
                Debug.Log(string.Format("Found ItemBoxData.UId: {0} : {1}",uId, existingData.Id));
            }
            return existingData;
        }
        public static async UniTask<LiveItemBoxData> Update(LiveItemBoxData modifiedData)
        {
            var success = await Cloud.CloudAPI<bool>.Update<LiveItemBoxData>(modifiedData, modifiedData.Id, k_storageKey);
            if (success)
                return modifiedData;
            else return new LiveItemBoxData();
        }
        public static async UniTask<bool> Delete(LiveItemBoxData liveData)
        {
            return await Cloud.CloudAPI<LiveItemBoxData>.Delete(liveData, k_storageKey);
        }
        public static async UniTask<bool> Create(ItemBoxData localData)
        {
            return await Cloud.CloudAPI<ItemBoxData>.Create(localData, k_storageKey);
        }
    }
}