using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateWithNamespace
{
    public class TemplateSettings : ScriptableObject
    {
        private const string LAST_KNOWN_LOCATION = "LAST_KNOWN_LOCATION";

        private static TemplateSettings instance;
        public static TemplateSettings Instance
        {
            get
            {
                if (instance)
                {
                    return instance;
                }

                string lastKnownLocation = EditorPrefs.GetString(LAST_KNOWN_LOCATION, null);
                if (lastKnownLocation != null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<TemplateSettings>(lastKnownLocation);
                    if (instance)
                    {
                        return instance;
                    }
                }

                string[] guids = AssetDatabase.FindAssets($"t:{nameof(TemplateSettings)}");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = AssetDatabase.LoadAssetAtPath<TemplateSettings>(path);

                    lastKnownLocation = path;
                    EditorPrefs.SetString(LAST_KNOWN_LOCATION, lastKnownLocation);
                }
                return instance;
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeTemplateSettings()
        {
            if (!Instance)
            {
                _ = EditorUtility.DisplayDialog(
                    "Template settings installer",
                    "Template settings asset does not exist, pick a location in project to create it.",
                    "Ok"
                );

                string path = EditorUtility.SaveFilePanelInProject(
                    "Template installer",
                    "TemplateSettings",
                    "asset",
                    ""
                );

                if (string.IsNullOrEmpty(path) == false)
                {
                    instance = ScriptableObject.CreateInstance<TemplateSettings>();
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        [Header("Namespace if root is empty")]
        [SerializeField]
        private string defaultNamespace = "DefaultNamespace";
        public string DefaultNamespace => defaultNamespace;

        [Header("Namespace words to delete")]
        [SerializeField]
        private List<string> removingNamespaceWords = new() { "Assets", "Scripts" };
        public IEnumerable<string> RemovingNamespaceWords => removingNamespaceWords;
    }
}
