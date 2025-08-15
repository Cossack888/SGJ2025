using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBindingTool : EditorWindow
{
    private const string enumPath = "Assets/Input/InputActionType.cs";
    private const string responseFolder = "Assets/Input/GeneratedResponses";
    private const string referenceFolder = "Assets/Input/GeneratedReferences";
    private const string bindingListPath = "Assets/Input/InputBindingList.asset";

    [MenuItem("Tools/Input/Generate All Input Bindings from InputActionAssets")]
    public static void Generate()
    {
        var assetGuids = AssetDatabase.FindAssets("t:InputActionAsset");
        HashSet<string> sanitizedNames = new HashSet<string>();

        foreach (var guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);

            foreach (var map in inputAsset.actionMaps)
            {
                foreach (var action in map.actions)
                {
                    string name = SanitizeEnumName(map.name + "_" + action.name);
                    sanitizedNames.Add(name);
                }
            }
        }

        GenerateEnum(sanitizedNames);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorApplication.delayCall += () =>
        {
            ContinueGeneratingAssets();
        };
    }

    private static void GenerateEnum(IEnumerable<string> names)
    {
        if (!Directory.Exists("Assets/Input"))
            Directory.CreateDirectory("Assets/Input");

        using (StreamWriter writer = new StreamWriter(enumPath, false))
        {
            writer.WriteLine("public enum InputActionType");
            writer.WriteLine("{");
            foreach (string name in names.Distinct())
                writer.WriteLine($"    {name},");
            writer.WriteLine("}");
        }

        Debug.Log(" InputActionType enum generated.");
    }

    private static void ContinueGeneratingAssets()
    {
        if (!Directory.Exists(referenceFolder)) Directory.CreateDirectory(referenceFolder);
        if (!Directory.Exists(responseFolder)) Directory.CreateDirectory(responseFolder);

        Dictionary<string, InputActionReference> references = new Dictionary<string, InputActionReference>();

        var inputAssets = AssetDatabase.FindAssets("t:InputActionAsset", new[] { "Assets/Input" })
            .Select(guid => AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetDatabase.GUIDToAssetPath(guid)));

        foreach (var inputAsset in inputAssets)
        {
            foreach (var map in inputAsset.actionMaps)
            {
                foreach (var action in map.actions)
                {
                    string name = SanitizeEnumName(map.name + "_" + action.name);
                    string inputRefPath = $"{referenceFolder}/{name}_Reference.asset";
                    InputActionReference reference = AssetDatabase.LoadAssetAtPath<InputActionReference>(inputRefPath);

                    // Create and save InputActionReference asset if it doesn't exist
                    if (reference == null)
                    {
                        reference = InputActionReference.Create(action);
                        AssetDatabase.CreateAsset(reference, inputRefPath);
                        AssetDatabase.SaveAssets();
                    }

                    references[name] = reference;

                    CreateInputResponseSO(reference, name);
                }
            }
        }

        CreateBindingList(references);
        Debug.Log(" InputResponseSOs and Binding List generated.");
    }

    private static void CreateInputResponseSO(InputActionReference reference, string name)
    {
        string path = $"{responseFolder}/{name}_InputResponse.asset";
        InputResponseSO response = AssetDatabase.LoadAssetAtPath<InputResponseSO>(path);

        if (response == null)
        {
            response = ScriptableObject.CreateInstance<InputResponseSO>();
            response.name = name;
            response.inputAction = reference;
            AssetDatabase.CreateAsset(response, path);
        }
        else
        {
            response.inputAction = reference;
            EditorUtility.SetDirty(response);
        }
    }

    private static void CreateBindingList(Dictionary<string, InputActionReference> references)
    {
        InputBindingListSO bindingList = AssetDatabase.LoadAssetAtPath<InputBindingListSO>(bindingListPath);
        if (bindingList == null)
        {
            bindingList = ScriptableObject.CreateInstance<InputBindingListSO>();
            AssetDatabase.CreateAsset(bindingList, bindingListPath);
        }

        var entries = new List<InputBindingListSO.BindingEntry>();

        foreach (var pair in references)
        {
            string name = pair.Key;
            var responsePath = $"{responseFolder}/{name}_InputResponse.asset";
            var response = AssetDatabase.LoadAssetAtPath<InputResponseSO>(responsePath);

            if (Enum.TryParse(name, out InputActionType parsed))
            {
                entries.Add(new InputBindingListSO.BindingEntry
                {
                    actionType = parsed,
                    response = response
                });
            }
            else
            {
                Debug.LogWarning(" Could not parse InputActionType: " + name);
            }
        }

        bindingList.bindings = entries.ToArray();
        EditorUtility.SetDirty(bindingList);
    }

    private static string SanitizeEnumName(string raw)
    {
        string sanitized = raw.Replace("/", "_").Replace(" ", "_");
        sanitized = new string(sanitized.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
        if (char.IsDigit(sanitized[0])) sanitized = "_" + sanitized;
        return sanitized;
    }
}
