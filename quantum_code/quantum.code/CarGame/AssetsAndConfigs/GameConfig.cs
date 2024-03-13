using System;
using Photon.Deterministic;
using Quantum.CarGame;
using Quantum.Inspector;

namespace Quantum
{
    public partial class GameConfig
    {
        public AssetRefEntityPrototype CarPrototype;

        // Duration variables for each state
        public FP SetupDuration = 3;
        public FP CountDownDuration = 3;
        public FP GamePlayDuration = 180;
        public FP SuddenDeathDuration = 30;
        public FP RoundOverDuration = 5;
        public FP ResetDuration = 3;

        public FP GetStateDuration(GameState newState)
        {
            switch (newState)
            {
                case GameState.Setup:
                    return SetupDuration;
                case GameState.CountDown:
                    return CountDownDuration;
                case GameState.GamePlay:
                    return GamePlayDuration;
                case GameState.SuddenDeath:
                    return SuddenDeathDuration;
                case GameState.RoundOver:
                    return RoundOverDuration;
                case GameState.Reset:
                    return ResetDuration;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }
}