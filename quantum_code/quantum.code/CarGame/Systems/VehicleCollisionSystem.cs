using Quantum;
using Quantum.Physics2D;

namespace Quantum;

public unsafe class VehicleCollisionSystem : SystemSignalsOnly, ISignalOnCollisionEnter3D
{
    public void OnCollisionEnter3D(Frame f, CollisionInfo3D info)
    {
        if (!f.Has<VehicleDamage>(info.Entity))
            return;
        // if (!f.Has<VehicleDamage>(info.Other))
        //     return;
        
        // var vehicleDamage = f.Get<VehicleDamage>(info.Entity);
        // var otherVehicleDamage = f.Get<VehicleDamage>(info.Other);
        
        f.Events.VehicleCollision(info.Entity, info.ContactNormal, info.ContactPoints.First, 100);
        
        Log.Debug("collision detected!");
        
        //TODO: Implement vehicle damage logic
    }
}