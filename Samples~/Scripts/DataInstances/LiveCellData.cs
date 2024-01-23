using Hoco.Cloud;
namespace Hoco.Samples.Runtime
{
    /// <summary>The container for the live data of a cell that exists on the block chain and the database.</summary>
    [System.Serializable]
    public class LiveCellData : LiveCloudData
    {
        /// <summary>The CelData that is used to create the cell in the world..</summary>
        public CellData? CellData { get; set; } = new CellData();
    }
}