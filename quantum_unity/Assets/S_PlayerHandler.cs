using System.Collections;
using UnityEngine;
using Cinemachine;
using Quantum;

public class S_PlayerHandler : MonoBehaviour
{
    [SerializeField] EntityView entityView;
    private bool hasSet;

    private void Start()
    {
        Debug.Log("Awake called in S_PlayerHandler");

        // Subscribe to the PlayerAttachedToVehicleEvent using the provided syntax
        QuantumEvent.Subscribe<EventPlayerAttachedToVehicleEvent>(this, handler: OnEventPlayerAttachedToVehicle);
        //yield return new WaitForSeconds(0.4f);
        
        SetPlayerCamera();
    }

    private void OnEventPlayerAttachedToVehicle(EventPlayerAttachedToVehicleEvent e)
    {
        Debug.Log("PlayerAttachedToVehicleEvent received");

        if (QuantumRunner.Default.Game.PlayerIsLocal(e.Player))
        {
            Debug.Log("Player is local, setting camera");
            SetPlayerCamera();
        }
        else
        {
            Debug.Log("Player is not local");
        }
    }

    public void SetPlayerCamera()
    {
        Debug.Log("SetPlayerCamera called");

        if (hasSet)
        {
            Debug.Log("Camera has already been set, returning");
            return;
        }

        QuantumGame game = QuantumRunner.Default.Game;
        Frame frame = game.Frames.Verified;

        if (frame.TryGet(entityView.EntityRef, out PlayerLink playerLink))
        {
            Debug.Log("PlayerLink found");

            if (game.PlayerIsLocal(playerLink.Player))
            {
                Debug.Log("Player is local, configuring camera");

                CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();

                if (vcam != null)
                {
                    vcam.Follow = entityView.transform;
                    vcam.LookAt = entityView.transform;
                    hasSet = true;
                }
                else
                {
                    Debug.LogError("CinemachineVirtualCamera not found in the scene");
                }
            }
            else
            {
                Debug.Log("Player is not local, camera not set");
            }
        }
        else
        {
            Debug.LogError("Failed to find PlayerLink for the given EntityRef");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy called in S_PlayerHandler");
        QuantumEvent.UnsubscribeListener(this);
    }
}
