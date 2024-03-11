using System;
using Photon.Deterministic;
using Quantum.CarGame;
using Quantum.Inspector;

namespace Quantum
{
    public partial class GameConfig
    {
        public AssetRefEntityPrototype CarPrototype;
        public FP SuddenDeathDuration = 30;
    }
}