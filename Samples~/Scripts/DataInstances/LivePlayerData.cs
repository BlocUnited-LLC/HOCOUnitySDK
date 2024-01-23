using Cysharp.Threading.Tasks;
using Hoco.Cloud;
using UnityEngine;
namespace Hoco.Samples.Runtime
{
    /// <summary>This is the data class that is used to store the PlayerData in the Cloud Database. Feel free to alter the included <see cref="PlayerData"/> class, but keep in mind it will revert to stock after you Update the SDK with a new version.</summary>
    [System.Serializable]
    public class LivePlayerData : LiveCloudData
    {
        private static readonly string k_storageKey = "TestPlayerData";
        private static string k_filterByAddress { get => $"{nameof(PlayerData)}.PlayerAddress"; }
        public PlayerData? PlayerData { get; set; } = new PlayerData();
        public LivePlayerData() { PlayerData = new PlayerData(); PlayerData.PlayerName = string.Empty; PlayerData.PlayerAddress = string.Empty; }
        public static async UniTask<LivePlayerData[]> GetAll()
        {
            return await CloudAPI<LivePlayerData>.GetAll(k_storageKey);
        }
        public static async UniTask<LivePlayerData> GetByAddress(string walletAddress)
        {
            return await CloudAPI<LivePlayerData>.Get(k_filterByAddress, walletAddress, k_storageKey);
        }
        public static async UniTask<LivePlayerData> GetOrCreateByAddress(string walletAddress)
        {
            var existingData = await CloudAPI<LivePlayerData>.Get(k_filterByAddress, walletAddress, k_storageKey);
            if (string.IsNullOrEmpty(existingData.Id))
            {
                existingData.PlayerData = new PlayerData();
                existingData.PlayerData.PlayerAddress = walletAddress;
                bool success = await Create(existingData.PlayerData);
                if (success)
                    return await GetByAddress(walletAddress);
                else
                    Debug.LogError("Failed to Create ItemBoxData");
            }
            else
            {
                Debug.Log(string.Format("Found PlayerData.PlayerAddress: {0} : {1}", walletAddress, existingData.Id));
            }
            return existingData;
        }
        public static async UniTask<LivePlayerData> Update(LivePlayerData modifiedData)
        {
            var success = await Cloud.CloudAPI<bool>.Update<PlayerData>(modifiedData.PlayerData, modifiedData.Id, k_storageKey);
            if (success)
                return modifiedData;
            else return new LivePlayerData();
        }
        public static async UniTask<bool> Delete(LivePlayerData dedData)
        {
            return await Cloud.CloudAPI<LivePlayerData>.Delete(dedData, k_storageKey);
        }
        /**
        * \brief Create new <see cref="PlayerData"/> which will wrap the <see cref="PlayerData"/> into a <see cref="LivePlayerData"/> and store it in the Cloud Database.
        * \details This is how you would <see cref="Create(PlayerData)"/> new <see cref="PlayerData"/> in your Mongo Database.
        * \code{.cs}
        * var success = await Cloud.CloudAPI<bool>.Create<PlayerData>(newPlayerData, k_storageKey);
        * if(success)                                                                    //Data was created and stored in Database, you can now invoke <see cref="GetByAddress(string)"/> 
        *   Debug.Log("PlayerData Created!");                           //using the same <see cref="PlayerData.PlayerAddress"/> you just pushed to the Database
        * else //Creating Failed, No WAN, or bad config
        *   Debug.Log("PlayerData Failed to Create :(");
        * YourClass.YourMethod();
        * \endcode
        */
        public static async UniTask<bool> Create(PlayerData newPlayerData)
        {
            return await Cloud.CloudAPI<PlayerData>.Create(newPlayerData, k_storageKey);
        }
    }
}