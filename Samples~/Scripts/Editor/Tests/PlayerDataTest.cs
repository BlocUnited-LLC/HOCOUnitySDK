using System.Collections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine.TestTools;
using Hoco.Samples.Runtime;
using UnityEngine;
namespace Hoco.Samples.Editor.Tests
{
    public class PlayerDataTest
    {
        private static string k_TestWalletAddress = "0xWASD69694201337TEST";

        [UnityTest]
        public IEnumerator RunGetAll() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Getting all Players from cloud");
            var data = await GetAllTestPlayers();
            LogStatus(string.Format("Got {0} Players", data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                LogStatus(string.Format("Player: {0}", JsonConvert.SerializeObject(data[i], Formatting.Indented)));
            }
            Assert.IsTrue(data != null);
        });

        [UnityTest]
        public IEnumerator RunDeleteAll() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Getting all Players from cloud");
            var data = await GetAllTestPlayers();
            LogStatus(string.Format("Found {0} Players... Deleting", data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                await DeleteTestPlayer(data[i]);
            }
            LogProccess("All Players Deleted");
            Assert.IsTrue(true);
        });

        [UnityTest]
        public IEnumerator ModifyCycle_SingleSequential() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Sequential Modify Cycle Test for PlayerData");
            bool pass = true; 
            float time = Time.realtimeSinceStartup;
            var testData = await GetOrCreateTestPlayer();
            if (string.IsNullOrEmpty(testData.Id))
            {
                LogStatus("Failed to Create/Get LivePlayerData");
                pass = false;
            }
            else
            {
                LogStatus(string.Format("Created/Pulled Player: '{0}' - ({1}) Successfully ({2}s)", testData.PlayerData.PlayerAddress, testData.Id, Time.realtimeSinceStartup - time));
            }

            for (int i = 0; i < 10; i++)
            {
                ModifyPlayerData(testData);
            }
            time = Time.realtimeSinceStartup;
            LogProccess("Updating Player with Random Attributes");
            testData = await UpdateTestPlayer(testData);
            LogStatus(string.Format("Player Update Completed ({0}s)", Time.realtimeSinceStartup - time));
            //LogProccess("Cleaning up ItemBox");
            //await DeleteTestPlayer(testData);
            Assert.IsTrue(pass);
        });

        [UnityTest]
        public IEnumerator FullCycle_SingleSequential() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Testing PlayerData...");
            LogStatus(string.Format("{0}", k_TestWalletAddress));
            string walletAddress = string.Format("{0}", k_TestWalletAddress);
            bool pass = true;
            LogStatus(string.Format("Getting PlayerData: {0}", k_TestWalletAddress));
            var livePlayerData = await LivePlayerData.GetByAddress(walletAddress);
            
            if (!string.IsNullOrEmpty(livePlayerData.Id))//PlayerData exists in DB, This should never be the case as this test will cleanup any created data
            {
                LogStatus(string.Format("Deleting stale PlayerData: {0}", k_TestWalletAddress));
                pass = await LivePlayerData.Delete(livePlayerData);
                Assert.IsTrue(pass);
            }
            //Begin the Creation Testings
            LogStatus(string.Format("Creating PlayerData( {0} )", k_TestWalletAddress));
            var newPlayer = new PlayerData() { PlayerAddress = k_TestWalletAddress };
            bool success = await LivePlayerData.Create(newPlayer);
            Assert.IsTrue(success);
            LogStatus(string.Format("PlayerData( {0} )", (success ? "CREATED" :"FAILED")));

            livePlayerData = await LivePlayerData.GetByAddress(walletAddress);
            var result = JsonConvert.SerializeObject(livePlayerData, Formatting.Indented);
            LogStatus(string.Format("Getting Created PlayerData: ( {0} )", result));

            if (!string.IsNullOrEmpty(livePlayerData.Id))//PlayerData was created successfully
            {
                LogStatus(string.Format("Modding PlayerData: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(livePlayerData, Newtonsoft.Json.Formatting.Indented)));
                for (int i = 0; i < 10; i++)
                {
                    ModifyPlayerData(livePlayerData);
                    await UniTask.Delay(200);
                }
                livePlayerData = await LivePlayerData.Update(livePlayerData);
                await UniTask.Delay(200);
                LogStatus(string.Format("Deleting PlayerData: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(livePlayerData, Newtonsoft.Json.Formatting.Indented)));
                pass = await LivePlayerData.Delete(livePlayerData);
            }
            else//Something went wrong
            {
                LogStatus("Failed to create PlayerData :(");
                pass = false;
                Assert.IsTrue(false);
            }
            Assert.IsTrue(pass);
        });

        private async UniTask<LivePlayerData[]> GetAllTestPlayers()
        {
            return await LivePlayerData.GetAll();
        }
        private async UniTask<LivePlayerData> GetOrCreateTestPlayer(int index = 0)
        {  
            return await LivePlayerData.GetOrCreateByAddress(TestAddress(index));
        }
        private async UniTask<LivePlayerData> UpdateTestPlayer(LivePlayerData testData)
        {
            float time = Time.realtimeSinceStartup;
            LogProccess(string.Format("Live Updating Player: '{0}' [{1}]", testData.PlayerData.PlayerAddress, testData.PlayerData.PlayerName));

            testData = await LivePlayerData.Update(testData);

            LogStatus(string.Format("Update for ItemBox: '{0}' Completed ({1}s) <3", testData.PlayerData.PlayerAddress, Time.realtimeSinceStartup - time));
            return testData;
        }
        private async UniTask DeleteTestPlayer(LivePlayerData testPlayer)

        {
            float time = Time.realtimeSinceStartup;
            bool pass = await LivePlayerData.Delete(testPlayer);
            LogStatus(string.Format("Deletion of Player: '{0}' ({1}s)", testPlayer.PlayerData.PlayerAddress, pass ? "SUCCESS <3" : "FAILURE :(", Time.realtimeSinceStartup - time));

            testPlayer = await LivePlayerData.GetByAddress(testPlayer.PlayerData.PlayerAddress);

            if (string.IsNullOrEmpty(testPlayer.Id))
            {
                LogStatus("Deletion Check Confirmed <3");
            }
        }

        private string TestAddress(int index = 0)
        {
            return string.Format("{0}_{1}", k_TestWalletAddress, index);
        }
        private int GetRandomInt()
        {
            return UnityEngine.Random.Range(0, 10000);
        }
        private int GetRandomSmallInt()
        {
            return UnityEngine.Random.Range(0, 7);
        }
        public void ModifyPlayerData(LivePlayerData _data)
        {
            var p = _data.PlayerData;
            if (string.IsNullOrEmpty(p.PlayerName) || p.PlayerName == "NULL")
                p.PlayerName = string.Format("Minion of Dencho #{0}", GetRandomInt());
            if (p.Experience >= 1000 * p.Level * (1 + (p.Level / 2f)))
            {
                LogStatus(string.Format("Level Up! [ {0} >> {1} ]", p.Level, p.Level + 1));
                p.Level++;
                //p.PrimaryStats.Agility += GetRandomSmallInt();
                //p.PrimaryStats.Charisma += GetRandomSmallInt();
                //p.PrimaryStats.Endurance += GetRandomSmallInt();
                //p.PrimaryStats.Intelligence += GetRandomSmallInt();
                //p.PrimaryStats.Perception += GetRandomSmallInt();
                //p.PrimaryStats.Strength += GetRandomSmallInt();
                //p.PrimaryStats.WillPower += GetRandomSmallInt();
                //p.PrimaryStats.Wisdom += GetRandomSmallInt();
                //LogStatus(string.Format("{0}", Newtonsoft.Json.JsonConvert.SerializeObject(p.PrimaryStats)));
            }
            int expGained = GetRandomInt();
            LogStatus(string.Format("Gained {0} EXP", expGained));
            p.Experience += expGained;
        }

        private void LogProccess(string msg)
        {
            TestContext.WriteLine(string.Format("\n\n>>> {0}\n", msg));
        }
        private void LogStatus(string msg)
        {
            TestContext.WriteLine(string.Format("=== {0}\n", msg));
        }
    }

}