using Quantum;

namespace Quantum;

public class LevelTriggerSystem : SystemSignalsOnly, ISignalOnTriggerEnter3D
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

        //if game state is not currently in sudden death, change to sudden death
        Log.Debug(f.GetSingleton<GameSessionManager>().CurrentGameState.ToString() );
        if (f.GetSingleton<GameSessionManager>().CurrentGameState != GameState.SuddenDeath)
        {
            f.GetSingleton<GameSessionManager>().ChangeGameState(f, GameState.SuddenDeath);
        }
    }
}