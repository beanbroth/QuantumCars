using Photon.Deterministic;
using Quantum.Core;
using System.Runtime.CompilerServices;


#nullable disable
namespace Quantum
{
    public unsafe partial struct VehicleController3D : IComponent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Move(FrameBase frame, EntityRef entity, FPVector3 direction)
        {
            PhysicsBody3D* pb = null;
            if (frame.Unsafe.TryGetPointer(entity, out pb) == false)
            {
                return;
            }

            // Use the provided direction for moving the entity
            pb->AddForce(direction * 100);
        }

        public void Jump(FrameBase frame, EntityRef entity, FP jumpForce)
        {
            PhysicsBody3D* pb = null;
            if (frame.Unsafe.TryGetPointer(entity, out pb) == false)
            {
                return;
            }
        }
    }
}