using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneDiagnostics
{
    [MenuItem("Tools/Diagnostics/Report Missing Components In Main Scene")]
    public static void ReportMissingComponentsInMainScene()
    {
        const string scenePath = "Assets/Scenes/Main.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var roots = scene.GetRootGameObjects();

        int missingCount = 0;
        var reports = new List<string>();

        foreach (var root in roots)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var components = transform.gameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        missingCount++;
                        reports.Add($"Missing component on '{GetPath(transform)}' at index {i}");
                    }
                }
            }
        }

        Debug.Log($"Scene diagnostics complete. Missing components: {missingCount}");
        foreach (var report in reports)
        {
            Debug.LogWarning(report);
        }
    }

    [MenuItem("Tools/Diagnostics/Repair Main Scene And Save")]
    public static void RepairMainSceneAndSave()
    {
        const string scenePath = "Assets/Scenes/Main.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // Opening the scene triggers Unity's internal removal of invalid components.
        // Mark dirty and save so the removal is persisted to disk.
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);

        if (saved)
        {
            AssetDatabase.SaveAssets();
            Debug.Log("Repair complete: Main scene was opened and saved.");
        }
        else
        {
            Debug.LogError("Repair failed: Unity could not save Main scene.");
        }
    }

    private static string GetPath(Transform current)
    {
        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = $"{current.name}/{path}";
        }

        return path;
    }
}
