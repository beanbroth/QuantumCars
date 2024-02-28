using System;
using Photon.Deterministic;
using Quantum.CarGame;

namespace Quantum
{
    public partial class VehicleController3DConfig
    {
        public Int32 numWheels = 4;
        public FP rideHeight = 2;
        public FPVector3[] wheelLocalPositions = new FPVector3[4];
        public bool[] wheelIsSteerable = new bool[4];
        
    }
}