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
    }
}