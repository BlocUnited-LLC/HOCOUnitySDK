using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hoco.Runtime;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hoco.Editor.TestCase
{
    public class PlayerDataTest
    {
        private static string k_TestWalletAddress = "0xWASD69694201337TEST";

        [UnityTest]
        public IEnumerator TestPlayerData() => UniTask.ToCoroutine(async () =>
        {
            TestContext.WriteLine("Testing PlayerData...");
            TestContext.WriteLine(string.Format("{0}", k_TestWalletAddress));
            string walletAddress = string.Format("{0}", k_TestWalletAddress);
            bool pass = true;
            TestContext.WriteLine(string.Format("Getting PlayerData: {0}", k_TestWalletAddress));
            var livePlayerData = await LivePlayerData.GetFromPlayerAddress(walletAddress);
            

            if (!string.IsNullOrEmpty(livePlayerData.Id))//PlayerData exists in DB, This should never be the case as this test will cleanup any created data
            {
                TestContext.WriteLine(string.Format("Deleting stale PlayerData: {0}", k_TestWalletAddress));
                pass = await LivePlayerData.Delete(livePlayerData);
                Assert.IsTrue(pass);
            }

            //Begin the Creation Testings
            TestContext.WriteLine(string.Format("Creating PlayerData( {0} )", k_TestWalletAddress));
            var newPlayer = new PlayerData() { PlayerAddress = k_TestWalletAddress };
            bool success = await LivePlayerData.Create(newPlayer);
            //new LivePlayerData() { PlayerData = new PlayerData() { PlayerAddress = k_TestWalletAddress }
            Assert.IsTrue(success);
            TestContext.WriteLine(string.Format("PlayerData( {0} )", (success ? "CREATED" :"FAILED")));

            livePlayerData = await LivePlayerData.GetFromPlayerAddress(walletAddress);
            var result = JsonConvert.SerializeObject(livePlayerData, Formatting.Indented);
            TestContext.WriteLine(string.Format("Getting Created PlayerData: ( {0} )", result));

            if (!string.IsNullOrEmpty(livePlayerData.Id))//PlayerData was created successfully
            {
                TestContext.WriteLine(string.Format("Modding PlayerData: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(livePlayerData, Newtonsoft.Json.Formatting.Indented)));
                for (int i = 0; i < 10; i++)
                {
                    ModifyPlayerData(livePlayerData);
                    await UniTask.Delay(200);
                }
                livePlayerData = await LivePlayerData.Update(livePlayerData);
                await UniTask.Delay(200);
                TestContext.WriteLine(string.Format("Deleting PlayerData: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(livePlayerData, Newtonsoft.Json.Formatting.Indented)));
                pass = await LivePlayerData.Delete(livePlayerData);
            }
            else//Something went wrong
            {
                TestContext.WriteLine("Failed to create PlayerData :(");
                pass = false;
                Assert.IsTrue(false);
            }

            Assert.IsTrue(pass);
        });
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
                TestContext.WriteLine(string.Format("Level Up! [ {0} >> {1} ]", p.Level, p.Level + 1));
                p.Level++;
                //p.PrimaryStats.Agility += GetRandomSmallInt();
                //p.PrimaryStats.Charisma += GetRandomSmallInt();
                //p.PrimaryStats.Endurance += GetRandomSmallInt();
                //p.PrimaryStats.Intelligence += GetRandomSmallInt();
                //p.PrimaryStats.Perception += GetRandomSmallInt();
                //p.PrimaryStats.Strength += GetRandomSmallInt();
                //p.PrimaryStats.WillPower += GetRandomSmallInt();
                //p.PrimaryStats.Wisdom += GetRandomSmallInt();
                //TestContext.WriteLine(string.Format("{0}", Newtonsoft.Json.JsonConvert.SerializeObject(p.PrimaryStats)));
            }
            int expGained = GetRandomInt();
            TestContext.WriteLine(string.Format("Gained {0} EXP", expGained));
            p.Experience += expGained;
        }
    }

}