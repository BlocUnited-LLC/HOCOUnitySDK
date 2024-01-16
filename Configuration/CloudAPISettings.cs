using UnityEngine;
namespace Hoco.Cloud
{
    [System.Serializable]
    public class CloudAPISettings
    {
        public string GetURL { get => $"{ApiServerUrl}{GetEndPoint}"; }
        public string GetAllURL { get => $"{ApiServerUrl}{GetAllEndPoint}"; }
        public string GetManyURL { get => $"{ApiServerUrl}{GetManyEndPoint}"; }
        public string CreateURL { get => $"{ApiServerUrl}{CreateEndPoint}"; }
        public string DeleteURL { get => $"{ApiServerUrl}{DeleteEndPoint}"; }
        public string UpdateURL { get => $"{ApiServerUrl}{UpdateEndPoint}"; }
        [field: SerializeField] public string ApiServerUrl { get; private set; } = "https://mongorest.azurewebsites.net/api/MongoDB/";
        [field: SerializeField] public string GetEndPoint { get; private set; } = "GetFilteredData";
        [field: SerializeField] public string GetAllEndPoint { get; private set; } = "GetAllDataByCollectionName";
        [field: SerializeField] public string GetManyEndPoint { get; private set; } = "GetMultipleCollectionsByName";
        [field: SerializeField] public string CreateEndPoint { get; private set; } = "CreateCollection";
        [field: SerializeField] public string DeleteEndPoint { get; private set; } = "DeleteCollection";
        [field: SerializeField] public string UpdateEndPoint { get; private set; } = "UpdateCollection";
    }
}