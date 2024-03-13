using System;
using Quantum.Core;

namespace Quantum;

public unsafe partial struct GameSessionManager : IComponent
{
    public void ChangeGameState(Frame f, GameState newState, bool withDelay = false)
    {
        if(!f.IsVerified)
            return;
        if (withDelay)
        {
            // Start the timer with the delay duration based on the state
            var delayDuration = ReferenceHelper.GetGameConfig(f).GetStateDuration(newState);
            stateChangeTimer = FrameTimer.CreateFromSeconds(f, delayDuration);
            // Store the intended state to transition after the delay
            PendingGameState = newState;
        }
        else
        {
            // Immediate state transition
            UpdateGameState(f, newState);
        }
    }

    private void UpdateGameState(Frame f, GameState newState)
    {
        CurrentGameState = newState;
        Log.Debug("GameSessionManager.ChangeGameState: " + newState.ToString());
        f.Signals.OnGameStateChanged(newState);
        HandleStateChange(f, newState);
    }

    private void HandleStateChange(Frame f, GameState newState)
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
    }

    public void CheckStateChangeTimer(Frame f)
    {
        if(!f.IsVerified)
            return;
        if (stateChangeTimer.CheckAndStopIfExpired(f))
        {
            UpdateGameState(f, PendingGameState);
        }
    } 

    public void ChangeGameStateIfStateIsNew(Frame f, GameState newState)
    {
        if (CurrentGameState == newState)
            return;
        ChangeGameState(f, newState);
    }

    private void HandleReset(Frame f)
    {
        ChangeGameState(f, GameState.Setup, true);
    }

    private void HandleRoundOver(Frame f)
    {
        ChangeGameState(f, GameState.Reset);
    }

    private void HandleSuddenDeath(Frame f)
    {
        ChangeGameState(f, GameState.RoundOver, true);
    }
}