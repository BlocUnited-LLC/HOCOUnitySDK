using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hoco.Data
{
    /// <summary>The base class for all entities that represent NFTs.</summary>
    [System.Serializable]
    public abstract class NFTData
    {
        /// <summary>The name of this NFT</summary>
        public string name = string.Empty;
        /// <summary>The description of this NFT</summary>
        public string description = string.Empty;
        /// <summary>The URL path on IPFS from the metadata</summary>
        public string image = string.Empty;
        public string external_url = string.Empty;
    }

}