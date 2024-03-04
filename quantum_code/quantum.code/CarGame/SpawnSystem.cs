using System;
using Quantum;

namespace Quantum;

public unsafe class SpawnSystem : SystemSignalsOnly, ISignalOnGameStateChanged
{
    public override void OnInit(Frame f)
    {
        Log.Debug("AISpawnSystem.OnInit");
        base.OnInit(f);
    }

    private static void MoveEntityToValidSpawnPoint(Frame f, EntityRef entityRef, ref bool hasBeenPlaced)
    {
        var spawnsFilter = f.Filter<Transform3D, SpawnPoint>();
        Transform3D* entityTransform = f.Unsafe.GetPointer<Transform3D>(entityRef);
        Transform3D* fallbackSpawnTransform = null;
        SpawnPoint* fallbackSpawnPoint = null;
        while (spawnsFilter.NextUnsafe(out var _, out var spawnTransform, out var spawnPoint))
        {
            if (fallbackSpawnTransform == null)
            {
                fallbackSpawnTransform = spawnTransform;
                fallbackSpawnPoint = spawnPoint;
            }

            if (!hasBeenPlaced && spawnPoint->hasSpawnedFlag == false)
            {
                entityTransform->Position = spawnTransform->Position;
                entityTransform->Rotation = spawnTransform->Rotation;
                spawnPoint->hasSpawnedFlag = true;
                hasBeenPlaced = true;
                break;
            }
        }

        if (!hasBeenPlaced && fallbackSpawnTransform != null)
        {
            entityTransform->Position = fallbackSpawnTransform->Position;
            entityTransform->Rotation = fallbackSpawnTransform->Rotation;
            if (fallbackSpawnPoint != null)
            {
                fallbackSpawnPoint->hasSpawnedFlag = true;
            }
        }
    }

    public static void RepositionEntity(Frame f, EntityRef entityToReposition)
    {
        bool hasRepositioned = false;
        MoveEntityToValidSpawnPoint(f, entityToReposition, ref hasRepositioned);
    }

    public static EntityRef SpawnFromPrototype(Frame f, EntityPrototype prototype)
    {
        var spawnedEntityRef = f.Create(prototype);
        bool hasSpawned = false;
        MoveEntityToValidSpawnPoint(f, spawnedEntityRef, ref hasSpawned);
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
        }
    }

    private void SpawnAllAI(Frame frame)
    {
        var aiFilter = frame.Filter<SpawnPoint>();
        while (aiFilter.Next(out var _, out var aiSpawnPoint))
        {
            AssetGuid prototypeID = ConfigAssetsHelper.GetGameConfig(frame).aiCarPrototype.Id;
            SpawnFromPrototype(frame, frame.FindAsset<EntityPrototype>(prototypeID));
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