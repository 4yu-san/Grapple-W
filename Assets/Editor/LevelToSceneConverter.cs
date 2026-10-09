using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelToSceneConverter : EditorWindow
{
    [MenuItem("Tools/Convert Neon Levels to Scenes")]
    public static void ConvertLevels()
    {
        string levelsDir = "Assets/Resources/Levels";
        string scenesDir = "Assets/Scenes";

        // Ensure scenes directory exists
        if (!Directory.Exists(scenesDir))
        {
            Directory.CreateDirectory(scenesDir);
        }

        // Find the LevelPrefabRegistry
        string[] registryGuids = AssetDatabase.FindAssets("t:LevelPrefabRegistry");
        if (registryGuids.Length == 0)
        {
            Debug.LogError("LevelToSceneConverter: Could not find any LevelPrefabRegistry in the project. Cannot build levels.");
            return;
        }
        LevelPrefabRegistry registry = AssetDatabase.LoadAssetAtPath<LevelPrefabRegistry>(AssetDatabase.GUIDToAssetPath(registryGuids[0]));

        // Get all JSON files starting with 'neon'
        string[] files = Directory.GetFiles(levelsDir, "neon*.json");
        if (files.Length == 0)
        {
            Debug.LogWarning("No files starting with 'neon' found in " + levelsDir);
            return;
        }

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string scenePath = $"{scenesDir}/{fileName}.unity";

            // Read the JSON data
            string json = File.ReadAllText(file);
            LevelData data = JsonUtility.FromJson<LevelData>(json);

            if (data == null || data.pieces == null || data.pieces.Length == 0)
            {
                Debug.LogError($"LevelToSceneConverter: {fileName} is empty or malformed.");
                continue;
            }

            // Create a new scene (with default light/camera)
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Create a temporary LevelBuilder
            GameObject builderGO = new GameObject("TempBuilder");
            LevelBuilder builder = builderGO.AddComponent<LevelBuilder>();
            
            // Set the registry using SerializedObject since registry is private
            SerializedObject so = new SerializedObject(builder);
            so.FindProperty("registry").objectReferenceValue = registry;
            so.ApplyModifiedProperties();

            // Build the level
            builder.Build(data);

            // Destroy the builder so it's not saved in the final scene
            DestroyImmediate(builderGO);

            // Save the scene
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"Successfully converted {fileName} to {scenePath}");
        }

        Debug.Log("Conversion complete!");
    }
}
