using System;
using Quantum;

namespace Quantum;

public unsafe class GameSessionSystem : SystemMainThread, ISignalOnMapChanged
{
    public override void OnInit(Frame f)
    {
        f.GetOrAddSingleton<GameSessionManager>();
        
        GameSessionManager* gameSession = f.Unsafe.GetPointerSingleton<GameSessionManager>();
        if (gameSession == null)
        {
            Log.Error("GameSessionSystem.Update: GameSession is null");
            return;
        }

       // gameSession->CurrentGameState = GameState.CountDown;
        gameSession->ChangeGameState(f, GameState.CountDown);
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
            
            gameSession->CheckSuddenDeathTimer(f);
    }

    public void OnMapChanged(Frame f, AssetRefMap previousMap)
    {
        Log.Debug( "GameSessionSystem.OnMapChanged");
    }
}