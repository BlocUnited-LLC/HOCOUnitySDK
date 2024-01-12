using System.Collections;
using UnityEngine;

namespace Hoco.Data
{
    /// <summary>
    /// A Cell is a point in worldspace that uses properties to define its Positio, Bounds and other features. 
    /// The Cell is meant to be a persistant object that lives on the block chain as a NFT, but the data in this class can be used as metadata to allow the players to nteract with this persistant instance of a Cell.
    /// </summary>
    [System.Serializable]
    public class CellData : NFTData
    {
        /// <summary>The radius of a cel determines how big the cell is in worldspace.</summary>
        public int Raduis = 100;
        /// <summary>
        /// This is the UniqueId among the whole project
        /// It is made out of the FloorId_BlocId combo and formated to fit
        /// A cell with a BlocId of 300 on floor 20 will have the TowerId of 020_0300
        /// </summary>
        public string TowerId = string.Empty;
        /// <summary>
        /// This is the Floor Index that the Cell lives on
        /// </summary>
        public int FloorId = 0;
        /// <summary>
        /// The id that is to be minted on the block
        /// It is also the Index of the CellData in the <see cref="CellManager.Cells"/> cache
        /// </summary>
        public int BlocId = 0;

        /// <summary>
        /// The id of the grid that this cell lives in
        /// </summary>
        public int GridId = 0;
        /// <summary>
        /// The Column Index on its current grid and ranges from 0 - ColumnCount
        /// </summary>
        public int GridColumn = 0;
        /// <summary>
        /// The Row Index on its current grid and ranges from 0 - RowCount
        /// </summary>
        public int GridRow = 0;
        /// <summary>
        /// The id of this cell relative to the grid it lives in
        /// </summary>
        public int CellId = 0;
        /// <summary>
        /// The resource type that can be gathered from this cell
        /// </summary>
        public string CellResource = "Dirt";
        public float CellResourceRate = 1;
        public CellPosition CellPosition = new CellPosition();
        /// <summary>
        /// 0 = Default mintable cell, 1 = Town/Entrance cell, 2 = Elevator/Exit Cell, 3 = Boss Cell
        /// </summary>
        public int CellTerritory = 0;
    }
    [System.Serializable]
    public class CellPosition
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
    }

}