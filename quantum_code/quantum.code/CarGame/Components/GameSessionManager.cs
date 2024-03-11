using System;
using Quantum.Core;

namespace Quantum;

public unsafe partial struct GameSessionManager : IComponent
{
    public void ChangeGameState(Frame f, GameState newState)
    {
        switch (newState)
        {
            case GameState.Setup:
                break;
            case GameState.CountDown:
                break;
            case GameState.GamePlay:
                break;
            case GameState.SuddenDeath:
                HandleSuddenDeath(f);
                break;
            case GameState.RoundOver:
                HandleRoundOver(f);
                break;
            case GameState.Reset:
                HandleReset(f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        CurrentGameState = newState;
        Log.Debug("GameSessionManager.ChangeGameState: " + newState.ToString());
        f.Signals.OnGameStateChanged(newState);
    }

    private void HandleReset(Frame f)
    {
        f.Map = f.FindAsset<Map>(f.RuntimeConfig.GameMaps[0].Id);

    }

    private void HandleRoundOver(Frame f)
    {
        ChangeGameState(f, GameState.Reset);
    }

    private void HandleSuddenDeath(Frame frame)
    {
        suddenDeathTimer =
            FrameTimer.CreateFromSeconds(frame, ConfigAssetsHelper.GetGameConfig(frame).SuddenDeathDuration);
    }

    public void CheckSuddenDeathTimer(Frame frame)
    {
        if (CurrentGameState != GameState.SuddenDeath)
            return;
        Log.Debug("GameSessionManager.CheckSuddenDeathTimer: " + suddenDeathTimer.RemainingTime(frame).ToString());
        if (suddenDeathTimer.CheckAndStopIfExpired(frame))
        {
            ChangeGameState(frame, GameState.RoundOver);
        }
    }
}