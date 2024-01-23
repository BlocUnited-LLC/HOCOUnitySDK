using System.Collections.Generic;
using UnityEngine;

namespace Hoco.Samples.Runtime
{
    [System.Serializable]
    public class ItemBoxData
    {
        public string UId { get; set; } = string.Empty;
        public List<ItemData> Items { get; set; } = new List<ItemData>();
    }
}