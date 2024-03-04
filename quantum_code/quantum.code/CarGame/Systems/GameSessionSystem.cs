using System;
using Quantum;

namespace Quantum;

public unsafe class GameSessionSystem : SystemMainThreadFilter<GameSessionSystem.Filter>, ISignalOnPlayerDataSet
{
    public struct Filter {
        public EntityRef Entity;
        public GameSessionManager* GameSession;
    }

    public override void OnInit(Frame f)
    {

    }


    public override void Update(Frame f, ref Filter filter)
    {
        GameSessionManager* gameSession = f.Unsafe.GetPointerSingleton<GameSessionManager>();
        if (gameSession == null)
        {
            Log.Error("GameSessionSystem.Update: GameSession is null");
            return;
        }

        
    }

    public void OnPlayerDataSet(Frame f, PlayerRef player)
    {
        GameSessionManager* gameSession = f.Unsafe.GetPointerSingleton<GameSessionManager>();
        if (gameSession == null)
        {
            Log.Error("GameSessionSystem.Update: GameSession is null");
            return;
        }
        gameSession->ChangeGameState(f, GameState.GameSetup);
        Log.Debug("GameSessionSystem.OnInit");
    }
}