using System;
using Photon.Deterministic;
using Quantum.CarGame;
using Quantum.Inspector;

namespace Quantum
{
    public partial class VehicleController3DConfig
    {
        public Int32 numWheels = 4;
        public FP rideHeight = 2;
        public FPVector3[] wheelLocalPositions = new FPVector3[4];
        public bool[] wheelIsSteering = new bool[4];
        public bool[] wheelIsPowered = new bool[4];
        public FP maxSteerAngle = 30;
        
        [Header("Engine and Torque")]
        [Tooltip("Maximum torque output of the engine. Change this to accelerate more")]
        public FP maxTorque = 100;

        [Header("Engine and Torque")]
        [Tooltip(
            "Hard capped maximum velocity of the car. Change this to increase top speed. torque will be applied along the speed torque curve to reach this speed")]
        public FP maxSpeed = 100;

        [Tooltip("Curve representing the torque output relative to the speed of the car compared to max speed.")]
        public FPAnimationCurve speedTorqueCurve;
        
    }
}