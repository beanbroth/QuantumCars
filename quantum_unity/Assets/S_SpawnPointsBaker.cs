using Quantum;
using UnityEditor;
using UnityEngine;

public class S_SpawnPointsBaker : MapDataBakerCallback
{
    public override void OnBeforeBake(MapData data)
    {
    }

    public override void OnBake(MapData data)
    {
        Debug.Log("Baking spawn points");
        var customData = UnityDB.FindAsset<MapCustomDataAsset>(data.Asset.Settings.UserAsset.Id);
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (customData == null || spawnPoints.Length == 0)
        {
            return;
        }

        customData.Settings.SpawnPoints = new MapCustomData.SpawnPointData[spawnPoints.Length];
        for (var i = 0; i < spawnPoints.Length; i++)
        {
            customData.Settings.SpawnPoints[i].Position = spawnPoints[i].transform.position.ToFPVector3();
            customData.Settings.SpawnPoints[i].Rotation = spawnPoints[i].transform.rotation.ToFPQuaternion();
        }


#if UNITY_EDITOR
        EditorUtility.SetDirty(customData);
#endif
    }
}