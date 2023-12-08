using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
                .GetFiles(CreateBehaviourScript.EDITOR_FOLDER_PATH)
                .Where(x => Path.GetFileName(x).StartsWith("___"))
                .Select(x => x.Replace(@"\\", "/"))
                .ToArray();

            foreach (string path in toRemove)
            {
                Cache cache = Caching.GetCacheByPath(path);
                _ = Caching.RemoveCache(cache);
            }

            //List<string> outFailedFiles = new();
            //_ = AssetDatabase.DeleteAssets(toRemove, outFailedFiles);
            //if (outFailedFiles.Count > 0)
            //{
            //    Debug.LogError(
            //        $"Error deleting garbage files: {outFailedFiles.Aggregate("", (x, y) => x + "\n" + y)}"
            //    );
            //}
        }
    }
}
