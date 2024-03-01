using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // Initialize the input actions
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Enable the input actions
        playerInputActions.Enable();

        // Subscribe to Quantum callbacks
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    private void OnDisable()
    {
        // Disable the input actions when the object is disabled
        playerInputActions.Disable();
    }

    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input input = new Quantum.Input();

        // Read the Move action value (can be a joystick, arrow keys, WASD, or touch input)
        Vector2 move = playerInputActions.Gameplay.Move.ReadValue<Vector2>();
        var x = move.x;
        var y = move.y;

        // Input that is passed into the simulation needs to be deterministic that's why it's converted to FPVector2.
        input.Direction = new Vector2(x, y).ToFPVector2();

        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }
}