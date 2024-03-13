#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Quantum;
using UnityEditor;
using UnityEngine;

public class S_MapDataHelper : MonoBehaviour
{
    [SerializeField] private MapData mapData;

    [InspectorButton("GenerateAllMapAssetsAndRefs")]
    [SerializeField] private bool _generateAllMapAssetsAndRefs;
    
    public void InitializeData()
    {
        mapData = gameObject.GetComponent<MapData>();
    }

    public void GenerateAllMapAssetsAndRefs()
    {
        InitializeData(); // gets references to MapData MonoBehaviour, which stores all the asset data

        // Get the current scene name
        string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);

        // Set the new folder path
        string baseFolderPath = "Assets/Resources/DB/LevelData";
        string folderPath = $"{baseFolderPath}/{sceneName}";

        // Create the necessary folders if they don't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources/DB"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "DB");
        }
        if (!AssetDatabase.IsValidFolder(baseFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources/DB", "LevelData");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(baseFolderPath, sceneName);
        }

        // File paths using the new folder path
        string mapAssetPath = folderPath + $"/{sceneName}_map.asset";
        string customDataAssetPath = folderPath + $"/{sceneName}_customData.asset";
        string graphAssetPath = folderPath + $"/{sceneName}_navigationGraph.asset";

        // Check if assets already exist and create them if not
        if (!File.Exists(mapAssetPath))
        {
            MapAsset mapAsset = ScriptableObject.CreateInstance<MapAsset>();
            AssetDatabase.CreateAsset(mapAsset, mapAssetPath);
        }
        if (!File.Exists(customDataAssetPath))
        {
            MapCustomDataAsset customDataAsset = ScriptableObject.CreateInstance<MapCustomDataAsset>();
            AssetDatabase.CreateAsset(customDataAsset, customDataAssetPath);
        }
        if (!File.Exists(graphAssetPath))
        {
            GraphAsset graphAsset = ScriptableObject.CreateInstance<GraphAsset>();
            AssetDatabase.CreateAsset(graphAsset, graphAssetPath);
        }

        // Refresh the AssetDatabase
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
