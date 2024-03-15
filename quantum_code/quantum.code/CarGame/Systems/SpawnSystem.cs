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
        //FindValidTargetAndAttachPlayer(frame, player);
    }

    private void AttachAllPlayersToVehicles(Frame frame)
    {
        for (int i = 0; i < frame.PlayerCount; i++)
        {
            PlayerRef player = i;

            if (player.IsValid)
            {
                //Log.Error("Player not found when respawning " + i);
            }
            else
            {
                Log.Debug("Attaching player " + player);
                FindValidTargetAndAttachPlayer(frame, player);
            }
        }
    }

    private void FindValidTargetAndAttachPlayer(Frame frame, PlayerRef player)
    {
        ComponentSet componentSet = new ComponentSet();
        componentSet.Add<PlayerLink>();
        var aiFilter = frame.Filter<VehicleController3D>(componentSet);
        
        if (aiFilter.Next(out var entity, out var vehicleController))
        {
            frame.Remove<AIController>(entity);
            var playerLink = new PlayerLink() { Player = player };
            frame.Add(entity, playerLink);
            
            frame.Events.PlayerAttachedToVehicleEvent(player);
        }
        else
        {
            Log.Error("No valid vehicle found for player " + player);
            
        }
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
                AttachAllPlayersToVehicles(f);
                break;
            case GameState.GamePlay:
            case GameState.RoundOver:
                break;
            case GameState.Reset:
                CleanUpAllDynamicEntities(f);
                f.Map = GetNextMap(f);
                break;
        }
    }

    public Map GetNextMap(Frame f)
    {
        // Get the current map using the current index
        var currentMap = f.FindAsset<Map>(f.RuntimeConfig.GameMaps[f.Global->MapIndex].Id);

        Log.Debug("Next map: " + f.Global->MapIndex);
        
        // Increment the map index and use modulo to ensure it stays within array bounds
        f.Global->MapIndex = (f.Global->MapIndex + 1) % f.RuntimeConfig.GameMaps.Length;

        // Return the current map
        return currentMap;
    }


    private void CleanUpAllDynamicEntities(Frame frame)
    {
        List<EntityRef> allEntities = new();
        frame.GetAllEntityRefs(allEntities);
        foreach (EntityRef entity in allEntities)
        {
            if (frame.Has<GameSessionManager>(entity))
                continue;
            // if (frame.Has<PlayerLink>(entity))
            //     continue;
            if (!frame.Exists(entity))
                continue;
            frame.Destroy(entity);
        }
    }

    private void SpawnAllAI(Frame frame)
    {
        AssetGuid prototypeID = ReferenceHelper.GetGameConfig(frame).CarPrototype.Id;
        var spawnPointIndex = 0; // Initialize a spawn point index
        var data = frame.FindAsset<MapCustomData>(frame.Map.UserAsset);
        for (spawnPointIndex = 0;
             spawnPointIndex < data.SpawnPoints.Length;
             spawnPointIndex++) // Replace SOME_DEFINED_LIMIT with the actual limit
        {
            EntityRef aiCar = SpawnFromPrototype(frame, frame.FindAsset<EntityPrototype>(prototypeID), spawnPointIndex);
            var mapAIGraph = ReferenceHelper.GetAINavigationGraph(frame);
            var aiController = new AIController()
            {
                CurrentNode = mapAIGraph.GetNode(mapAIGraph.nodes[0].Neighbors[0]), NodeReachedDistance = 50
            };
            frame.Add(aiCar, aiController);
        }
    }
}