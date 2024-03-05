using Quantum.Core;

namespace Quantum;

public unsafe partial struct GameSessionManager : IComponent
{
    
    public void ChangeGameState(Frame f, GameState newState)
    {
        CurrentGameState = newState;
        f.Signals.OnGameStateChanged(newState);
    }
}