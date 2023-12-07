using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateBehaviourScript : EditorWindow
{
    [MenuItem("Assets/Create/C# Script with namespace", priority = 50)]
    public static void ShowWindow()
    {
        _ = EditorWindow.GetWindow(typeof(CreateBehaviourScript));
    }

    private bool autoFocus = true;
    private string scriptName = "NewScript";

    private void OnGUI()
    {
        GUILayout.Label("Custom Script Creator", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        GUI.SetNextControlName(scriptName);

        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        if (autoFocus)
        {
            EditorGUI.FocusTextInControl("NewScript");
            autoFocus = false;
        }

        if (GUILayout.Button("Create Script") || Event.current.keyCode == KeyCode.Return)
        {
            _ = EditorGUI.EndChangeCheck();
            CreateNewScript();
        }
    }

    private void CreateNewScript()
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

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(
            $"{filePath}/{scriptName}.cs"
        );

        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
            "Packages/com.kylturpro.item-templates-for-unity/Editor/NewBehaviourScript.txt"
        );

        string content = textAsset.text;
        string writeNameSpace = filePath
            .Replace("/", ".")
            .Replace("Assets.", "")
            .Replace("Scripts.", "");
        content = content.Replace("$rootnamespace$", writeNameSpace);
        content = content.Replace("$rootpath$", writeNameSpace.Replace(".", "/"));
        content = content.Replace("$safeitemname$", scriptName);

        File.WriteAllText(assetPathAndName, content);

        AssetDatabase.Refresh();

        Object createdScript = AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(Object));
        Selection.activeObject = createdScript;
        EditorGUIUtility.PingObject(createdScript);

        Close();
    }
}
