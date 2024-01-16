using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Hoco.Cloud;
using System.IO;
namespace Hoco.Editor
{
    public class HocoSDKWindow : EditorWindow
    {
        [MenuItem("Hoco/Settings")]
        public static void Initialize()
        {
            var window = GetWindow<HocoSDKWindow>("Hoco SDK");
            window.minSize = new Vector2(240, 420);
            window.Show();
        }

        private void OnEnable()
        {

            if (CloudAPIConfiguration.Selected == null)
            {
                HocoSDKEditorSettings.VerifyResoucesPath();
                var instance = CloudAPIConfiguration.Instance;
                var instancePath = AssetDatabase.GetAssetPath(instance);
                Debug.LogError(string.Format("Had to set Selected CloudAPIConfiguration to {0}", instancePath), instance);
                CloudAPIConfiguration.Selected = instance;
                HocoSDKEditorSettings.HocoSDKSettingsPath = instancePath;
                EditorUtility.SetDirty(CloudAPIConfiguration.Selected);

                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(CloudAPIConfiguration.Selected);
            }
        }
        private void OnGUI()
        {
            GUILayout.Label("CloudAPI Status", EditorStyles.boldLabel);
            GUILayout.Space(10);
            using (var statusBoxScope = new GUILayout.HorizontalScope())
            {

                using (var statusScope = new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(string.Format("Selected Cloud API Configuration: {0}", CloudAPIConfiguration.Selected == null ? "NULL" : Cloud.CloudAPIConfiguration.Selected.name.ToUpper()), EditorStyles.miniBoldLabel);
                    if (GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
                        OnEnable();
                    if (GUILayout.Button("Show", EditorStyles.miniButtonMid))
                    {
                        Selection.activeObject = CloudAPIConfiguration.Selected;
                        EditorGUIUtility.PingObject(CloudAPIConfiguration.Selected);
                    }
                    if (GUILayout.Button("X", EditorStyles.miniButtonRight))
                    {
                        CloudAPIConfiguration.Selected = null;
                        HocoSDKEditorSettings.HocoSDKSettingsPath = default;
                    }
                }
            }

            GUILayout.Label(string.Format("Set Config Path \n{0}", HocoSDKEditorSettings.HocoSDKSettingsPath));
            //var asset = AssetDatabase.LoadAssetAtPath<Cloud.CloudAPIConfiguration>(HocoSDKEditorSettings.HocoSDKSettingsPath);
            //GUILayout.Label(string.Format("Asset: {0}", asset == null ? "NULL" : asset.name.ToUpper()));
        }
    }
}