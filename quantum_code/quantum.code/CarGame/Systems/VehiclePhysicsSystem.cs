using System.Collections.Generic;
using Quantum.Core;

namespace Quantum.CarGame;

public unsafe class VehiclePhysicsSystem : SystemMainThreadFilter<VehiclePhysicsSystem.Filter>
{
    public struct Filter
    {
        public EntityRef Entity;
        public VehicleController3D* VehicleController3D;
        public Transform3D* Transform3D;
        public PhysicsBody3D* PhysicsBody3D;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var rigConfig = f.FindAsset<VehicleController3DConfig>(filter.VehicleController3D->RigConfig.Id);
        var wheelConfigs = LoadWheelConfigs(f, filter.VehicleController3D, rigConfig);
        filter.VehicleController3D->UpdatePhysics(f, filter.PhysicsBody3D, filter.Transform3D, rigConfig, wheelConfigs);
    }

    private List<WheelControllerConfig> LoadWheelConfigs(FrameBase frame, VehicleController3D* vehicleController,
        VehicleController3DConfig rigConfig)
    {
        var resolvedWheelConfigs = frame.ResolveList(vehicleController->WheelConfigs);
        var wheelConfigs = new List<WheelControllerConfig>();
        for (int i = 0; i < rigConfig.numWheels; i++)
        {
            wheelConfigs.Add(frame.FindAsset<WheelControllerConfig>(resolvedWheelConfigs[i].Id));
        }

        return wheelConfigs;
    }
}