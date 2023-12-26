using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTemplateWithNamespace
{
    public static class CreateBehaviourScript
    {
        public const string DEFAULT_SCRIPT_NAME = "NewBehaviourScript";
        public const string EDITOR_FOLDER_PATH =
            "Packages/com.kylturpro.item-templates-for-unity/Editor";
        public const string TEMP_GENERATING_FOLDER = "Assets";
        private const string TO_REMOVE = "Assets";
        private static string BASE_APPLICATION_PATH =>
            Application.dataPath[..^TO_REMOVE.Length].Replace('\\', '/');

        [MenuItem("Assets/Create/C# Script With Namespace", priority = 50)]
        public static void ShowWindow()
        {
            // Get unique script name with path in context
            string newScriptName = GetNewScriptPathName(DEFAULT_SCRIPT_NAME);

            // Get base template content
            string content = GetTextFromAssetText($"{EDITOR_FOLDER_PATH}/BaseTemplate.txt");

            // Writing pretty template
            string assetRelativePath = Path.GetRelativePath(BASE_APPLICATION_PATH, newScriptName)
                .Replace(Path.GetFileName(newScriptName), "");

            // Split
            List<string> separated = assetRelativePath.Trim('\\').Split("\\").ToList();

            // Replace spaces with _
            separated = separated.Select(x => x.Replace(" ", "_")).ToList();

            // Filter
            separated = separated
                .Where(x => !TemplateSettings.Instance.RemovingNamespaceWords.Contains(x))
                .ToList();

            if (separated.Count() == 0)
            {
                separated.Add(TemplateSettings.Instance.DefaultNamespace);
            }

            content = content.Replace(
                "$rootnamespace$",
                separated.Aggregate((x, y) => x + "." + y)
            );
            content = content.Replace("$rootpath$", separated.Aggregate((x, y) => x + "/" + y));

            // Generating unique template
            string templateNamePath = AssetDatabase.GenerateUniqueAssetPath(
                $"{TEMP_GENERATING_FOLDER}/________NewTemplate.cs.txt"
            );

            // Creating template
            File.Create(templateNamePath).Close();
            File.WriteAllText(templateNamePath, content);

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateNamePath, newScriptName);
        }

        private static string GetTextFromAssetText(string path)
        {
            return AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
        }

        private static string GetNewScriptPathName(string scriptName)
        {
            string filePath;

            if (Selection.assetGUIDs.Length == 0)
            {
                filePath = "Assets";
            }
            else
            {
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                string fileExtension = Path.GetExtension(filePath);
                if (fileExtension != "")
                {
                    filePath = Path.GetDirectoryName(filePath);
                }
            }

            return AssetDatabase.GenerateUniqueAssetPath($"{filePath}/{scriptName}.cs");
        }
    }
}
