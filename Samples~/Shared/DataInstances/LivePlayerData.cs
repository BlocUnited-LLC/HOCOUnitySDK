using System.Collections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
namespace Hoco.Runtime
{
    /// <summary>This is the data class that is used to store the PlayerData in the Cloud Database. Feel free to alter the included <see cref="PlayerData"/> class, but keep in mind it will revert to stock after you Update the SDK with a new version.</summary>
    [System.Serializable]
    public class LivePlayerData : LiveCloudData
    {
        public PlayerData? PlayerData { get; set; } = new PlayerData();
        public LivePlayerData() { PlayerData.PlayerName = string.Empty; PlayerData.PlayerAddress = string.Empty; }
        private static readonly string k_storageKey = "PlayerData";
        private static readonly string k_filterByAddress = "PlayerData.PlayerAddress";
        public static async UniTask<LivePlayerData> GetFromPlayerAddress(string walletAddress)
        {
            return await Cloud.CloudBase<LivePlayerData>.Get(k_filterByAddress, walletAddress, k_storageKey);
        }
        public static async UniTask<LivePlayerData> Update(LivePlayerData modifiedData)
        {
            var success = await Cloud.CloudBase<bool>.Update<PlayerData>(modifiedData.PlayerData, modifiedData.Id, k_storageKey);
            if (success)
                return modifiedData;
            else return new LivePlayerData();
        }
        public static async UniTask<bool> Delete(LivePlayerData dedData)
        {
            return await Cloud.CloudBase<LivePlayerData>.Delete(dedData, k_storageKey);
        }
        /**
        * \brief Create new <see cref="PlayerData"/> which will wrap the <see cref="PlayerData"/> into a <see cref="LivePlayerData"/> and store it in the Cloud Database.
        * \details This is how you would <see cref="Create(PlayerData)"/> new <see cref="PlayerData"/> in your Mongo Database.
        * \code{.cs}
        * var success = await Cloud.CloudBase<bool>.Create<PlayerData>(newPlayerData, k_storageKey);
        * if(success)                                                                    //Data was created and stored in Database, you can now invoke <see cref="GetFromPlayerAddress(string)"/> 
        *   Debug.Log("PlayerData Created!");                           //using the same <see cref="PlayerData.PlayerAddress"/> you just pushed to the Database
        * else //Creating Failed, No WAN, or bad config
        *   Debug.Log("PlayerData Failed to Create :(");
        * YourClass.YourMethod();
        * \endcode
        */
        public static async UniTask<bool> Create(PlayerData newPlayerData)
        {
            
            return await Cloud.CloudBase<PlayerData>.Create(newPlayerData, k_storageKey);
        }
    }
}