using Photon.Deterministic;
using System;

namespace Quantum
{
    partial class RuntimeConfig
    {
        public int AICount;
        public AssetRefGameConfig GameConfig;
        public AssetRefMap[] GameMaps;
        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref AICount);
            stream.SerializeArray(ref GameMaps, (ref AssetRefMap element) =>  stream.Serialize(ref element));
            stream.Serialize(ref GameConfig.Id);
        }
    }
}