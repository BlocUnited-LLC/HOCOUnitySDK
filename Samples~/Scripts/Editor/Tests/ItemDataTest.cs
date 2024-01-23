using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Hoco.Samples.Runtime;
using System.Linq;
namespace Hoco.Samples.Editor.Tests
{
    public class ItemDataTest 
    {
        private static readonly string k_TestBoxUId = "Tests_TestBox";

        [UnityTest]
        public IEnumerator RunGetAll() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Getting all ItemBoxes from cloud");
            var data = await GetAllTestBoxes();
            LogStatus(string.Format("Got {0} ItemBoxes", data.Length));
            for(int i =0; i < data.Length;i++)
            {
                LogStatus(string.Format("ItemBox: {0}", JsonConvert.SerializeObject(data[i])));
            }
            Assert.IsTrue(data != null);
        });

        [UnityTest]
        public IEnumerator RunDeleteAll() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Getting allItemBoxes from cloud");
            var data = await GetAllTestBoxes();
            LogStatus(string.Format("Found {0} ItemBoxes... Deleting", data.Length));
            for (int i = 0; i < data.Length; i++)
            {
                await DeleteTestBox(data[i]);
            }
            LogProccess("All Boxes Deleted");
            Assert.IsTrue(data.Length < 1);
        });

        [UnityTest]
        public IEnumerator ModifyCycle_SingleSequential() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Sequential Modify Cycle Test for ItemBoxData");
            bool pass = true;
            float time = Time.realtimeSinceStartup;
            var testData = await GetOrCreateTestBox();
            if (string.IsNullOrEmpty(testData.Id))
            {
                LogStatus("Failed to Create LiveItemBoxData");
                pass = false;
            }
            else
            {
                LogStatus(string.Format("Created/Pulled ItemBox: '{0}' - ({1}) Successfully ({2}s)", testData.ItemBoxData.UId,testData.Id, Time.realtimeSinceStartup - time));
            }
            RandomlyAddItems(testData, 3);
            time = Time.realtimeSinceStartup;
            LogProccess("Updating ItemBox with Random Items");
            testData = await UpdateTestBox(testData);
            LogStatus(string.Format("Box Update Completed ({0}s)", Time.realtimeSinceStartup - time));
            LogProccess("Cleaning up ItemBox");
            await DeleteTestBox(testData);
            LogProccess(string.Format("Box Cleanup Completed ({0}s)", Time.realtimeSinceStartup - time));
            Assert.IsTrue(pass);
        });

        [UnityTest]
        public IEnumerator ModifyCycle_MultiSequential() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Sequential Modify Cycle Test for 15 ItemBoxes");
            List<LiveItemBoxData> boxes = new List<LiveItemBoxData>();
            float time = Time.realtimeSinceStartup;
            for (int i = 0; i < 15; i++)
            {
                var box = await GetOrCreateTestBox(i);
                boxes.Add(box);
            }
            LogStatus(string.Format("Box Creation Completed ({0}s)", Time.realtimeSinceStartup - time));
            LogProccess("Updating all ItemBoxes with Random Items");
            for (int i = 0; i < boxes.Count; i++)
            {
                RandomlyAddItems(boxes[i], 3);
            }
            time = Time.realtimeSinceStartup;
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i] = await UpdateTestBox(boxes[i]);
            }
            time = Time.realtimeSinceStartup;
            LogStatus(string.Format("Box Update Completed ({0}s)", Time.realtimeSinceStartup - time));
            LogProccess("Cleaning up all ItemBoxes");
            for (int i = 0; i < boxes.Count; i++)
            {
                await DeleteTestBox(boxes[i]);
            }
            LogProccess(string.Format("Box Cleanup Completed ({0}s)", Time.realtimeSinceStartup - time));
            Assert.IsTrue(true);
        });

        [UnityTest]
        public IEnumerator ModifyCycle_MultiParallel() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Parallel Modify Cycle Test for ItemBoxes");
            List<UniTask<LiveItemBoxData>> createTasks = new List<UniTask<LiveItemBoxData>>();
            // Start all create tasks
            for (int i = 0; i < 15; i++)
            {
                createTasks.Add(GetOrCreateTestBox(i));
            }
            float time = Time.realtimeSinceStartup;
            LiveItemBoxData[] boxes = await UniTask.WhenAll(createTasks);
            LogStatus(string.Format("Box Creation Completed ({0}s)", Time.realtimeSinceStartup - time));
            for(int i = 0; i < boxes.Length; i++)
            {
                RandomlyAddItems(boxes[i], 3);
            }
            List<UniTask<LiveItemBoxData>> updateTasks = new List<UniTask<LiveItemBoxData>>();
            // Start all update tasks
            LogProccess("Updating all ItemBoxes with Random Items");
            for (int i = 0; i < boxes.Length; i++)
            {
                updateTasks.Add(UpdateTestBox(boxes[i]));
            }
            time = Time.realtimeSinceStartup;
            boxes = await UniTask.WhenAll(updateTasks);
            LogStatus(string.Format("Box Update Completed ({0}s)", Time.realtimeSinceStartup - time));
            // Start all delete tasks
            LogProccess("Cleaning up all ItemBoxes");
            List<UniTask> deleteTasks = new List<UniTask>();
            for(int i = 0; i < boxes.Length; i++)
            {
                deleteTasks.Add(DeleteTestBox(boxes[i]));
            }
            await UniTask.WhenAll(deleteTasks);
            LogStatus(string.Format("Box Cleanup Completed ({0}s)", Time.realtimeSinceStartup - time));

            Assert.IsTrue(true);
        });

        [UnityTest]
        public IEnumerator BirthCycle_MultiParallel() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Parallel Birth Cycle Test for 15 ItemBoxes");

            List<UniTask<LiveItemBoxData>> createTasks = new List<UniTask<LiveItemBoxData>>();
            float time = Time.realtimeSinceStartup;
            // Start all create tasks
            for (int i = 0; i < 15; i++)
            {
                createTasks.Add(GetOrCreateTestBox(i));
            }
            // Await all create tasks to complete
            LiveItemBoxData[] boxes = await UniTask.WhenAll(createTasks);
            LogStatus(string.Format("Box Creation Completed ({0}s)", Time.realtimeSinceStartup - time));

            LogProccess("Cleaning up all ItemBoxes");
            // Start all delete tasks
            List<UniTask> deleteTasks = boxes.Select(box => DeleteTestBox(box)).ToList();
            // Await all delete tasks to complete
            await UniTask.WhenAll(deleteTasks);
            LogProccess(string.Format("Box Cleanup Completed ({0}s)", Time.realtimeSinceStartup - time));
            Assert.IsTrue(true);
        });

        [UnityTest]
        public IEnumerator BirthCycle_MultiSequential() => UniTask.ToCoroutine(async () =>
        {
            LogProccess("Begining Squential Birth Cycle Test for 15 ItemBoxes");
            List<LiveItemBoxData> boxes = new List<LiveItemBoxData>();
            float time = Time.realtimeSinceStartup;
            for(int i = 0; i < 15; i++)
            {
                var box = await GetOrCreateTestBox(i);
                boxes.Add(box);
            }
            LogStatus(string.Format("Box Creation Completed ({0}s)", Time.realtimeSinceStartup - time));
            LogProccess("Cleaning up all ItemBoxes");
            time = Time.realtimeSinceStartup;
            for(int i = 0; i < boxes.Count; i++)
            {
                await DeleteTestBox(boxes[i]);
            }
            LogProccess(string.Format("Box Cleanup Completed ({0}s)", Time.realtimeSinceStartup - time));
            Assert.IsTrue(true);
        });

        [UnityTest]
        public IEnumerator SerializeCycle_Single() => UniTask.ToCoroutine(async () =>
        {
            TestContext.WriteLine("Testing ItemData...");
            ItemData data = GenerateItemData();
            
            string json = JsonConvert.SerializeObject(data);
            TestContext.WriteLine(string.Format("ItemData: Serialized\n\n{0}\n\n{1}", JsonConvert.SerializeObject(data, Formatting.Indented), json));
            await UniTask.Delay(0);
            var itemData = JsonConvert.DeserializeObject<ItemData>(json);
            json = JsonConvert.SerializeObject(itemData);
            TestContext.WriteLine(string.Format("ItemData Deserialized: \n\n{0}\n\n{1}", JsonConvert.SerializeObject(itemData, Formatting.Indented), json));
            
            Assert.IsTrue(itemData.ItemName == data.ItemName);
        });

        private async UniTask<LiveItemBoxData[]> GetAllTestBoxes()
        {
            return await LiveItemBoxData.GetAll();
        }
        private async UniTask<LiveItemBoxData> GetOrCreateTestBox(int index = 0)
        {
            return await LiveItemBoxData.GetOrCreateByUId(TestIdentifier(index));
        }
        private async UniTask<LiveItemBoxData> UpdateTestBox(LiveItemBoxData testBox)
        {
            float time = Time.realtimeSinceStartup;
            LogProccess(string.Format("Live Updating ItemBox: '{0}' [{1} Items]", testBox.ItemBoxData.UId, testBox.ItemBoxData.Items.Count));

            testBox = await LiveItemBoxData.Update(testBox);

            LogStatus(string.Format("Update for ItemBox: '{0}' Completed ({1}s) <3",testBox.ItemBoxData.UId,Time.realtimeSinceStartup - time));
            return testBox;
        }
        private async UniTask DeleteTestBox(LiveItemBoxData testBox)
        {
            float time = Time.realtimeSinceStartup;
            bool pass =  await LiveItemBoxData.Delete(testBox);
            
            LogStatus(string.Format("Deletion of ItemBox: '{0}' ({1}s)", testBox.ItemBoxData.UId,pass?"SUCCESS <3":"FAILURE :(", Time.realtimeSinceStartup - time));
            //else
                //LogStatus(string.Format("No ItemBox: '{0}' found in Cloud)", testBox.ItemBoxData.UId));

            testBox = await LiveItemBoxData.GetByUId(testBox.ItemBoxData.UId);
            //LogStatus(string.Format("{0}",JsonConvert.SerializeObject(testBox)));

            if (string.IsNullOrEmpty(testBox.Id))
            {
                LogStatus("Deletion Check Confirmed <3");
            }
        }

        private void RandomlyAddItems(LiveItemBoxData liveData, int count)
        {
            LogStatus(string.Format("Adding Random Items to ItemBox: {0}", liveData.ItemBoxData.UId));
            for (int i =0; i < count; i++)
            {
                var rngItem = GenerateItemData(Random.Range(1000, 10000));
                LogStatus(string.Format("AddItem: {0}", JsonConvert.SerializeObject(rngItem)));
                liveData.ItemBoxData.Items.Add(rngItem);
            }
        }
        private string TestIdentifier(int index = 0)
        {
            return string.Format("{0}_{1}", k_TestBoxUId, index);
        }

        private void LogProccess(string msg)
        {
            TestContext.WriteLine(string.Format("\n\n>>> {0}\n", msg));
        }
        private void LogStatus(string msg)
        {
            TestContext.WriteLine(string.Format("=== {0}\n", msg));
        }

        private ItemBoxData GenerateItemBoxData()
        {
            ItemBoxData data = new ItemBoxData();
            //data.UId = k_TestBoxUId;
            for (int i = 0; i < 100; i++)
            {
                ItemData itemData = GenerateItemData(i);
                data.Items.Add(itemData);
            }
            return data;
        }
        private ItemData GenerateItemData(int index = 0)
        {
            ItemData data = new ItemData();
            data.ItemName = string.Format("Test Item {0}", index);
            data.ItemDescription = "This is a test item";
            data.ItemValue = 1;
            data.ItemWeight = 1f;
            data.ItemType = ItemType.General;
            data.ItemRarity = ItemRarity.Trash;
            data.ItemEquipType = EquipmentType.None;
            data.ItemIndex = 1;
            PrimaryStats priStats = new PrimaryStats(new Dictionary<string, float>() { { "Test", 3 } });
            priStats.Strength = 4;
            priStats.Wisdom = 69;
            priStats.Perception = 420;
            SecondaryStats secStats = new SecondaryStats(new Dictionary<string, float>() { { "Test", 69 } });
            data.ItemStats = new ItemStats(priStats, secStats);
            data.ItemStats.PrimaryStats.SetStat("TestPower", 6969);
            data.ItemStats.PrimaryStats.SetStat("JumpPower", 69);
            data.ItemStats.PrimaryStats.SetStat("RunPower", 69);
            return data;
        }
    }
}