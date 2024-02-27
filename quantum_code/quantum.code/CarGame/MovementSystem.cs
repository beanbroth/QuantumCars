namespace Quantum.CarGame;

public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public VehicleController3D* VehicleController3D;
        //public PlayerLink* Link;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        // gets the input for correct player
        Input input = default;
        if(f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
        {
            input = *f.GetPlayerInput(playerLink->Player);
        }

        filter.VehicleController3D->Move(f, filter.Entity, input.Direction.XOY);
    }
}