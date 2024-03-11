using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.CarGame;

namespace Quantum;

public unsafe class SpawnSystem : SystemSignalsOnly, ISignalOnGameStateChanged, ISignalOnPlayerDataSet
{
    public void OnPlayerDataSet(Frame frame, PlayerRef player)
    {
        var data = frame.GetPlayerData(player);

        // Resolve the reference to the avatar prototype.
        var prototype = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
        ComponentSet componentSet = new ComponentSet();
        componentSet.Add<PlayerLink>();
        var aiFilter = frame.Filter<VehicleController3D>(componentSet);
        aiFilter.Next(out var entity, out var vehicleController);
        frame.Remove<AIController>(entity);

        //When a player joins, use a filter to find a vehicle controller and add a player link to the entity
        var playerLink = new PlayerLink() { Player = player, };
        frame.Add(entity, playerLink);
    }

    private static void MoveEntityToSpawnPoint(Frame f, EntityRef entityRef, int spawnPointIndex)
    {
        var data = f.FindAsset<MapCustomData>(f.Map.UserAsset);
        data.SetEntityToSpawnPoint(f, entityRef, spawnPointIndex);
    }

    public static EntityRef SpawnFromPrototype(Frame f, EntityPrototype prototype, int spawnPointIndex)
    {
        var spawnedEntityRef = f.Create(prototype);
        MoveEntityToSpawnPoint(f, spawnedEntityRef, spawnPointIndex);
        return spawnedEntityRef;
    }

    public void OnGameStateChanged(Frame f, GameState newState)
    {
        switch (newState)
        {
            case GameState.Setup:
                SpawnAllAI(f);
                break;
            case GameState.GamePlay:
            case GameState.RoundOver:
                break;
            case GameState.Reset:
                CleanUpAllDynamicEntities(f);
                break;
        }
    }

    private void CleanUpAllDynamicEntities(Frame frame)
    {
        List<EntityRef> allEntities = new();
        frame.GetAllEntityRefs(allEntities);
        foreach (EntityRef entity in allEntities)
        {
            if(frame.Has<GameSessionManager>(entity))
                continue;
            if (!frame.Exists(entity))
                continue;
            frame.Destroy(entity);
        }

        frame.GetSingleton<GameSessionManager>().ChangeGameState(frame, GameState.Setup);
    }

    private void SpawnAllAI(Frame frame)
    {
        AssetGuid prototypeID = ConfigAssetsHelper.GetGameConfig(frame).CarPrototype.Id;
        var spawnPointIndex = 0; // Initialize a spawn point index
        var data = frame.FindAsset<MapCustomData>(frame.Map.UserAsset);
        for (spawnPointIndex = 0;
             spawnPointIndex < data.SpawnPoints.Length;
             spawnPointIndex++) // Replace SOME_DEFINED_LIMIT with the actual limit
        {
            EntityRef aiCar = SpawnFromPrototype(frame, frame.FindAsset<EntityPrototype>(prototypeID), spawnPointIndex);
            var mapAIGraph = ConfigAssetsHelper.GetAINavigationGraph(frame);
            var aiController = new AIController()
            {
                CurrentNode = mapAIGraph.GetNode(mapAIGraph.nodes[0].Neighbors[0]), NodeReachedDistance = 50
            };
            frame.Add(aiCar, aiController);
        }
    }
}