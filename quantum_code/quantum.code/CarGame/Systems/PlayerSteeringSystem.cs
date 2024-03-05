using Quantum.Core;

namespace Quantum.CarGame;

public unsafe class PlayerSteeringSystem : SystemMainThreadFilter<PlayerSteeringSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public VehicleController3D* VehicleController3D;
        public PlayerLink* PlayerLink;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        // gets the input for the correct player
        Input input = default;
        input = *f.GetPlayerInput(filter.PlayerLink->Player);
        var rigConfig = f.FindAsset<VehicleController3DConfig>(filter.VehicleController3D->RigConfig.Id);
        
        filter.VehicleController3D->UpdateInternalVariables(f, filter.Entity, rigConfig, input.Direction);
    }
}