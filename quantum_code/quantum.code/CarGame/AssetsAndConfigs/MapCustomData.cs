using System;
using Photon.Deterministic;
using Quantum.Core;
using Quantum.Inspector;

namespace Quantum
{
    public unsafe partial class MapCustomData
    {
        [Serializable]
        public struct SpawnPointData
        {
            public FPVector3 Position;
            [Degrees] public FPQuaternion Rotation;
        }

        public SpawnPointData DefaultSpawnPoint;
        public SpawnPointData[] SpawnPoints;
        
        public AssetRefGraph AINavigationGraph;
        
        public void SetEntityToSpawnPoint(Frame f, EntityRef entity, Int32? index) {
            var transform  = f.Unsafe.GetPointer<Transform3D>(entity);
            var spawnPoint = index.HasValue && index.Value < SpawnPoints.Length ? SpawnPoints[index.Value] : DefaultSpawnPoint;
            transform->Position = spawnPoint.Position;
            transform->Rotation = spawnPoint.Rotation;
        }
    }
}