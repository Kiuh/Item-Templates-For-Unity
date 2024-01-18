using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateWithNamespace
{
    public class ClassEndingsRemoverPostprocessor : AssetPostprocessor
    {
        private const string MARKER_TEXT = "$customscript$";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            foreach (string asset in importedAssets.Where(x => x.EndsWith(".cs")))
            {
                TextAsset script = (TextAsset)
                    AssetDatabase.LoadAssetAtPath(asset, typeof(TextAsset));

                if (script.text.Contains(MARKER_TEXT))
                {
                    string content = script.text;
                    content = content.Replace(MARKER_TEXT, "");

                    foreach (string ending in TemplateSettings.Instance.RemovingClassEndings)
                    {
                        content = content.Replace(ending + "\")]", "\")]");
                    }

                    if (TemplateSettings.Instance.DeleteDuplication)
                    {
                        int firstIndex = content.IndexOf(content.Split(".").Last());
                        int lastIndex = content.IndexOf("\")]");
                        int length = lastIndex - firstIndex;
                        if (length <= 0)
                        {
                            return;
                        }
                        string scriptName = content.Substring(firstIndex, length);
                        content = content.Replace(scriptName + "." + scriptName, scriptName);
                    }

                    File.WriteAllText(AssetDatabase.GetAssetPath(script), content);
                    EditorUtility.SetDirty(script);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
