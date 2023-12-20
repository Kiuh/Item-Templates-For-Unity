using System.IO;
using UnityEditor;
using UnityEngine;

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
        string neededNamespace = assetRelativePath
            .TrimEnd('\\')
            .Replace("\\", ".")
            .Replace(" ", "_")
            .Replace("Assets.Scripts.", "");
        content = content.Replace("$rootnamespace$", neededNamespace);
        content = content.Replace("$rootpath$", neededNamespace.Replace(".", "/"));

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
