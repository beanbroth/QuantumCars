using Photon.Deterministic;
using System;

namespace Quantum
{
    partial class RuntimeConfig
    {
        public AssetRefGameConfig GameConfig;
        
        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref GameConfig.Id);
        }
    }
}