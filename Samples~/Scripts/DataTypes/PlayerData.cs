namespace Hoco.Samples.Runtime
{
    /// <summary>This is a PlayerData class provided by the HocoSDK. You can use this to store data about your player or delete it if you make your own.</summary>
    [System.Serializable]
    public class PlayerData
    {
        /// <summary>The WalletAddress of the Player which is also used as a Unique Identifier for the Player.</summary>
        public string PlayerAddress { get; set; } = "";
        
        /// <summary>The Dislpay name for the Player, usually asigned during the signup process</summary>
        public string PlayerName { get; set; } = "Player";
        
        /// <summary>The Player's Level, this is just an example of how you might store data about your player as not all game genres will have RPG elements.</summary>
        public int Level { get; set; } = 1;
        
        /// <summary>How much Experience the Player has, this is just an example of how you might store data about your player as not all game genres will have RPG elements.</summary>
        public int Experience { get; set; } = 0;

        //Here is a custom data type that might hold the PrimaryStats for a Player
        //public PrimaryStats PrimaryStats { get; set; } = new PrimaryStats();
        //Here is a custom data type that might hold the InventoryData for a Player
        //public List<InventoryData> Inventory { get; set; } = new List<InventoryData>();

    }
}