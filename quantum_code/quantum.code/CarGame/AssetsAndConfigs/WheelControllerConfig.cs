using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    public partial class WheelControllerConfig
    {
        [Header("Suspension Settings")]
        [Tooltip(
            "Strength of the suspension springs. Higher will make you like a trampoline, lower will make you like a pillow.")]
        public FP springStrength = 3100;

        [Tooltip(
            "Damping factor for the suspension. Works against strength to dampen osclations. Higher values feel like honey in springs, lower values feel like nothing in springs.")]
        public FP springDamper = 734;
        
        [Header("Tire Dynamics")]
        [Tooltip(
            "Curve representing how tire grip changes with different tire angle. 0 is driving straight, 1 is full lock(sliding sideways)")]
        public FPAnimationCurve tireGripFactor;

        [Tooltip(
            "Mass of each tire, used in force calculations. Change this for more grip, without changing the grip curve")]
        public FP tireMass = 2;

       
    }
}