using UnityEngine;

namespace Hoco.Samples
{
    [System.Serializable]
    public class ItemData
    {
        /// <summary>
        /// The ItemId is specialy crafted via InvItem Types, and is followed by the actual id which is 4 digits long.
        /// For example, If a Wall InvItem type = 10 then a WallItem might have an index of 100005, 101210, or 109999. 
        /// If a Deco InvItem type = 420 then a DecoItem might have an index of 4200005, 4201210, or 4209999.
        /// See <see cref="ItemType"/> for more details
        /// </summary>
        public int ItemId
        {
            get { return m_ItemId >= 0 ? m_ItemId : int.Parse(string.Format("{0}{1}", (int)ItemType, ItemIndex.ToString("0000"))); }
        }
        private int m_ItemId = -1;
        /// <summary>
        /// This is the Unique Index of the Type, like the 5 in 100005,or the 1210 in  101210, or 9999 in 109999.
        /// </summary>
        public int ItemIndex
        {
            get => m_ItemIndex; 
            set
            {
                m_ItemId = -1;
                m_ItemIndex = value;
            }
        }
        private int m_ItemIndex = -1;
        /// <summary>
        /// The Name of the InvItem
        /// </summary>
        public string ItemName = string.Empty;
        /// <summary>
        /// Flavor text for the Player
        /// </summary>
        public string ItemDescription = string.Empty;
        /// <summary>
        /// The Weight of the InvItem when its a Stack of 1
        /// </summary>
        public float ItemWeight = 0f;
        public long ItemValue = 1;
        /// <summary>
        /// Should be the value of one of the <see cref="ItemType"/>s
        /// </summary>
        public ItemType ItemType = ItemType.General;
        public ItemRarity ItemRarity = ItemRarity.Trash;
        public EquipmentType ItemEquipType = EquipmentType.None;
        public DecorationType ItemDecoType = DecorationType.GroundAndWalls;
        [field: SerializeField]
        public ItemStats ItemStats { get; set; } = new ItemStats();

        /// <summary>
        /// Use to see if <see cref="NFTData"/> is null before you use it, When true it mean its an NFT Item.
        /// </summary>
        public bool HasNFTData { get; private set; }

        public NFTMetadata NFTData
        {
            get => nftData;
            set
            {
                nftData = value;
                HasNFTData = nftData != null;
            }
        }
        private NFTMetadata nftData = new NFTMetadata();

    }

}