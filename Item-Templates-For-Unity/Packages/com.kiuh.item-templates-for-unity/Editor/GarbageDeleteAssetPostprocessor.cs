using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateWithNamespace
{
    public class GarbageDeleteAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            if (importedAssets.Length > 0)
            {
                string[] toRemove = Directory
                    .GetFiles(CreateBehaviourScript.TEMP_GENERATING_FOLDER)
                    .Where(x => Path.GetFileName(x).StartsWith("________"))
                    .Select(x => x.Replace(@"\\", "/"))
                    .ToArray();

                List<string> outFailedFiles = new();
                _ = AssetDatabase.DeleteAssets(toRemove, outFailedFiles);
                if (outFailedFiles.Count > 0)
                {
                    Debug.LogError(
                        $"Error deleting garbage files: {outFailedFiles.Aggregate("", (x, y) => x + "\n" + y)}"
                    );
                }
            }
        }
    }
}
