using Quantum;

namespace Quantum;

public unsafe class LevelTriggerSystem : SystemSignalsOnly, ISignalOnTriggerEnter3D
{
    public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
    {
        //Log.Debug("Trigger Enter");
        if (!f.Has<FinishLine>(info.Entity))
            return;
        if (!f.Has<VehicleController3D>(info.Other))
            return;
        f.Remove<VehicleController3D>(info.Other);
        if (f.Has<AIController>(info.Other))
        {
            f.Destroy(info.Other);
        }
        
        ReferenceHelper.GetGameSessionManager(f)->ChangeGameStateIfStateIsNew(f, GameState.SuddenDeath);
    }
}