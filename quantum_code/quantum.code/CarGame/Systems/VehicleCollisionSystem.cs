using Quantum;

namespace Quantum;

public unsafe class VehicleCollisionSystem : SystemSignalsOnly, ISignalOnCollision3D
{
    public void OnCollision3D(Frame f, CollisionInfo3D info)
    {
        if (!f.Has<VehicleDamage>(info.Entity))
            return;
        if (!f.Has<VehicleDamage>(info.Other))
            return;
        
        var vehicleDamage = f.Get<VehicleDamage>(info.Entity);
        var otherVehicleDamage = f.Get<VehicleDamage>(info.Other);
        
        //TODO: Implement vehicle damage logic
    }
}