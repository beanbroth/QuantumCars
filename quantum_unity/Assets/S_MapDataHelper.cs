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

        // Determine the path of the current scene
        string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string folderPath = Path.GetDirectoryName(scenePath) + "/LevelData";

        // Create the "LevelData" subfolder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(scenePath), "LevelData");
        }

        // Naming convention
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