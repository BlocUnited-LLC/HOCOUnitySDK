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
        private void OnEnable()
        {
            CheckConfiguration();
        }
        private void OnGUI()
        {
            DrawStatus();
        }

        private void DrawStatus()
        {
            using (var statusBoxScope = new GUILayout.VerticalScope(GUI.skin.box))
            {
                Color gCol = GUI.color;
                GUI.color = CloudAPIConfiguration.IsInitialized ? Color.green : Color.yellow;
                GUILayout.Label(string.Format("CloudAPI Status: {0}", CloudAPIConfiguration.IsInitialized ? "OK!" : "ERROR :("), EditorStyles.miniBoldLabel);
                GUI.color = gCol;
                using (var selectConfigScope = new GUILayout.VerticalScope())
                {
                    GUILayout.Label(string.Format("Selected Cloud API Configuration: {0}\nPath: {1}",
                        CloudAPIConfiguration.Selected == null ? "NULL" : Cloud.CloudAPIConfiguration.Selected.name.ToUpper(),
                        HocoSDKEditorSettings.HocoSDKSettingsPath),
                        EditorStyles.miniLabel);
                    using (var statusScope = new GUILayout.HorizontalScope())
                    {
                        if (CloudAPIConfiguration.IsInitialized)
                        {
                            if (GUILayout.Button("Show", EditorStyles.miniButtonLeft))
                            {
                                Selection.activeObject = CloudAPIConfiguration.Selected;
                                EditorGUIUtility.PingObject(CloudAPIConfiguration.Selected);
                            }
                            if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
                            {
                                CloudAPIConfiguration.Selected = null;
                                HocoSDKEditorSettings.HocoSDKSettingsPath = default;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Refresh", EditorStyles.miniButton))
                                OnEnable();
                        }
                    }
                }
            }
        }
        private void CheckConfiguration()
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

        [MenuItem("Hoco/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<HocoSDKWindow>("Hoco SDK");
            window.minSize = new Vector2(240, 420);
            window.Show();
        }
    }
}