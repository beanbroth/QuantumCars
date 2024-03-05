using System;
using Photon.Deterministic;
using Quantum;

namespace Quantum;

public unsafe class SpawnSystem : SystemSignalsOnly, ISignalOnGameStateChanged
{
    public override void OnInit(Frame f)
    {
        Log.Debug("AISpawnSystem.OnInit");
        base.OnInit(f);
    }

    private static void MoveEntityToValidSpawnPoint(Frame f, EntityRef entityRef)
    {
        var spawnsFilter = f.Filter<Transform3D, SpawnPoint>();
        Transform3D* entityTransform = f.Unsafe.GetPointer<Transform3D>(entityRef);

        // Variables for the first pass
        //bool foundUnspawned = false;
        int spawnCount = 0;
        Transform3D* selectedSpawnTransform = null;
        SpawnPoint* selectedSpawnPoint = null;

        // First pass: Check for unspawned spawn points and count total spawn points
        while (spawnsFilter.NextUnsafe(out var _, out var spawnTransform, out var spawnPoint))
        {
            if (spawnPoint->hasSpawnedFlag == false)
            {
                entityTransform->Position = spawnTransform->Position;
                entityTransform->Rotation = spawnTransform->Rotation;
                spawnPoint->hasSpawnedFlag = true;
                return;
            }

            spawnCount++;
        }

        // If no unspawned spawn points were found, proceed to randomly select one
        // if (!foundUnspawned && spawnCount > 0)
        // {
        //     int randomIndex = new Random().Next(spawnCount);
        //     int currentIndex = 0;
        //     spawnsFilter = f.Filter<Transform3D, SpawnPoint>(); // Reset filter to iterate again
        //
        //     // Second pass: Select a random spawn point
        //     while (spawnsFilter.NextUnsafe(out var _, out var spawnTransform, out var spawnPoint))
        //     {
        //         if (currentIndex == randomIndex)
        //         {
        //             selectedSpawnTransform = spawnTransform;
        //             selectedSpawnPoint = spawnPoint;
        //             break;
        //         }
        //
        //         currentIndex++;
        //     }
        //
        //     if (selectedSpawnTransform != null)
        //     {
        //         entityTransform->Position = selectedSpawnTransform->Position;
        //         entityTransform->Rotation = selectedSpawnTransform->Rotation;
        //         if (selectedSpawnPoint != null)
        //         {
        //             selectedSpawnPoint->hasSpawnedFlag = true;
        //         }
        //     }
        // }
    }

    public static void RepositionEntity(Frame f, EntityRef entityToReposition)
    {
        MoveEntityToValidSpawnPoint(f, entityToReposition);
    }

    public static EntityRef SpawnFromPrototype(Frame f, EntityPrototype prototype)
    {
        var spawnedEntityRef = f.Create(prototype);
        MoveEntityToValidSpawnPoint(f, spawnedEntityRef);
        return spawnedEntityRef;
    }

    public void OnGameStateChanged(Frame f, GameState newState)
    {
        switch (newState)
        {
            case GameState.GameSetup:
                SpawnAllAI(f);
                RepositionAllVehicles(f);
                break;
            case GameState.GameRunning:
            case GameState.GameEnd:
                ResetAllSpawnPointFlags(f);
                break;
            case GameState.GameReset:
                RepositionAllVehicles(f);
                break;
        }
    }

    private void SpawnAllAI(Frame frame)
    {
        var aiFilter = frame.Filter<SpawnPoint>();

        //frame.PlayerCount;
        
        
        while (aiFilter.Next(out var _, out var aiSpawnPoint))
        {
    
            AssetGuid prototypeID = ConfigAssetsHelper.GetGameConfig(frame).aiCarPrototype.Id;
           
           EntityRef aiCar = SpawnFromPrototype(frame, frame.FindAsset<EntityPrototype>(prototypeID));
            
            var aiController = new AIController()
            {
            
            };
            frame.Add(aiCar, aiController);
        }
    }

    private void ResetAllSpawnPointFlags(Frame frame)
    {
        var spawnsFilter = frame.Filter<SpawnPoint>();
        while (spawnsFilter.Next(out var _, out var spawnPoint))
        {
            spawnPoint.hasSpawnedFlag = false;
        }
    }

    private void RepositionAllVehicles(Frame frame)
    {
        var vehiclesFilter = frame.Filter<VehicleController3D>();
        while (vehiclesFilter.Next(out var vehicleEntity, out var vehicle))
        {
            RepositionEntity(frame, vehicleEntity);
        }
    }
}