using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Core;

namespace Quantum.CarGame;

public unsafe class AISteeringSystem : SystemMainThreadFilter<AISteeringSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public VehicleController3D* VehicleController3D;
        public AIController* AIController;
        public Transform3D* Transform3D;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var rigConfig = f.FindAsset<VehicleController3DConfig>(filter.VehicleController3D->RigConfig.Id);


        if (filter.AIController->ReachedNode(filter.Transform3D))
        {
            filter.AIController->SetCurrentNode(ConfigAssetsHelper.GetAINavigationGraph(f));
        }
        
        
        FPVector2 SteeringDirection = filter.AIController->SteerTowardsCurrentNode(filter.Transform3D);
        
        
        
        
        filter.VehicleController3D->UpdateInternalVariables(f, filter.Entity, rigConfig, SteeringDirection);
    }
}