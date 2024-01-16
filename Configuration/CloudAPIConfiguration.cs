using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hoco.Cloud
{
    /// <summary>
    /// A type of ScriptableSingleton that holds the <see cref="CloudAPISettings"/> for the Cloud API. A <see cref="ScriptableObject"/> where you configure what server to use.
    /// </summary>
    [AssetPath("CloudAPIConfiguration")]
    [CreateAssetMenu(fileName = "CloudAPIConfiguration_Default", menuName = "Hoco/CloudAPIConfiguration")]
    public class CloudAPIConfiguration : ScriptableSingleton<CloudAPIConfiguration>
    {
        public static bool IsInitialized { get; private set; }
        public static CloudAPIConfiguration Selected 
        {
            get => m_Selected;
            set
            {
                IsInitialized = (m_Selected = value) != null;
            }
        }
        [SerializeField]
        private static CloudAPIConfiguration m_Selected = null;
        public CloudAPISettings Settings = new CloudAPISettings();
    }

}