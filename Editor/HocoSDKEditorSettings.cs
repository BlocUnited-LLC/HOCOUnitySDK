using UnityEngine;
using UnityEditor;
using Hoco.Cloud;
namespace Hoco.Editor
{
    [InitializeOnLoad]
    public class HocoSDKEditorSettings
    {
        static HocoSDKEditorSettings()
        {
            CloudAPIConfiguration.Selected = AssetDatabase.LoadAssetAtPath<Cloud.CloudAPIConfiguration>(HocoSDKSettingsPath);
            if (CloudAPIConfiguration.Selected == null)
            {
                VerifyResoucesPath();
                CloudAPIConfiguration.Selected = CloudAPIConfiguration.Instance;
                HocoSDKSettingsPath = AssetDatabase.GetAssetPath(CloudAPIConfiguration.Selected);
                Debug.Log(string.Format("HocoSDKSettingsPath: {0}", HocoSDKSettingsPath));
            }
            Debug.Log(string.Format("HocoSDKSettings Initialized: {0}", HocoSDKSettingsPath));
        }
        public static string HocoSDKSettingsPath
        {
            get => EditorPrefs.GetString(string.Format("{0}.{1}.{2}",
                nameof(Cloud.CloudAPIConfiguration),
                "PATH",
                nameof(HocoSDKSettingsPath)), string.Format("Assets/Resouces/{0}.asset", nameof(Cloud.CloudAPIConfiguration)));
            set => EditorPrefs.SetString(string.Format("{0}.{1}.{2}",
                nameof(Cloud.CloudAPIConfiguration),
                "PATH",
                nameof(HocoSDKSettingsPath)), value);
        }
        public static void VerifyResoucesPath()
        {
            string resourcesFolderPath = "Assets/Resources";

            if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
            {
                // Create the Resources folder if it doesn't exist
                AssetDatabase.CreateFolder("Assets", "Resources");
                Debug.Log("Resources folder created.");
            }
            else
            {
                Debug.Log("Resources folder Validated.");
            }
        }
    }
}