using Photon.Deterministic;
using System;

namespace Quantum
{
    partial class RuntimeConfig
    {
        public AssetRefGameConfig GameConfig;
        public AssetRefMap[] GameMaps;
        partial void SerializeUserData(BitStream stream)
        {
            stream.SerializeArray(ref GameMaps, (ref AssetRefMap element) =>  stream.Serialize(ref element));
            stream.Serialize(ref GameConfig.Id);
        }
    }
}