using Photon.Deterministic;
using Quantum.Core;
using System.Runtime.CompilerServices;
using Quantum.Physics3D;

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

            //get transform
            Transform3D* transform = null;
            if (frame.Unsafe.TryGetPointer(entity, out transform) == false)
            {
                return;
            }
            
            VehicleController3DConfig config = frame.FindAsset<VehicleController3DConfig>(RigConfig.Id);
            

            // Raycast down to check if the entity is grounded
            Hit3D? hit = frame.Physics3D.Raycast(transform->Position, FPVector3.Down, config.rideHeight);
            if (hit.HasValue)
            {
                // If the entity is grounded, apply a force to move it
                pb->AddForce(FPVector3.Up * 200);
            }

          
            Draw.Ray(transform->Position, FPVector3.Down * config.rideHeight, ColorRGBA.Red);

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