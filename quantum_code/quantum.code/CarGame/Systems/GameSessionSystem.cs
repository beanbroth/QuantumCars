using System;
using Quantum;

namespace Quantum;

public unsafe class GameSessionSystem : SystemMainThread, ISignalOnMapChanged
{
    public override void OnInit(Frame f)
    { 
        f.GetOrAddSingleton<GameSessionManager>();

        GameSessionManager* gameSession = ReferenceHelper.GetGameSessionManager(f);
        if (gameSession == null)
        {
            Log.Error("GameSessionSystem.Update: GameSession is null");
            return;
        }

        f.Global->MapIndex = 0;
        gameSession->ChangeGameState(f, GameState.Setup, true);
        Log.Debug("GameSessionSystem.OnInit");
    }

    public override void Update(Frame f)
    {
           GameSessionManager* gameSession = f.Unsafe.GetPointerSingleton<GameSessionManager>();
            if (gameSession == null)
            {
                Log.Error("GameSessionSystem.Update: GameSession is null");
                return;
            }
            
            gameSession->CheckStateChangeTimer(f);
    }

    public void OnMapChanged(Frame f, AssetRefMap previousMap)
    {
        Log.Debug( "GameSessionSystem.OnMapChanged");
    }
}