using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Hoco.Runtime
{
    /// <summary>This is the Base class for all LiveData objects that are used in Cloud Databases, you can inherit from this class to make your own Datatype. :O</summary>
    [System.Serializable]
    public abstract class LiveCloudData 
    {
        /// <summary>This unique identifier is used to identify the object in the Cloud MongoDB Database. It is automatically generated when the object is created, and is used for Updating, Deleting, and Querying the object.</summary>
        [JsonProperty("_id")]
        public string Id { get; set; } = string.Empty;
    }

}