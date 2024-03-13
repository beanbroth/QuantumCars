using System;
using Quantum;
using UnityEngine;

public class VehicleWheelAnimationController : QuantumCallbacks
{
    //TODO: this script can be better 
    //// Constants for tuning the rotation effect
    public float accelerationSensitivity = 10.0f; // Adjust as needed
    public float speedSensitivity = 100f; // Adjust as needed
    public GameObject[] wheelObjects; // Assign main wheel objects in Unity Editor
    public GameObject[] wheelChildObjects; // Assign child objects to rotate in Unity Editor
    private EntityView _entityView;
    private float[] wheelRotations; // Array to store rotation for each wheel
    [SerializeField] private float maxWheelSpinSpeed;

    private void Awake()
    {
        _entityView = GetComponentInParent<EntityView>();
        wheelRotations = new float[wheelObjects.Length];
        if (wheelObjects.Length == 0 || wheelObjects.Length != wheelChildObjects.Length)
        {
            Debug.LogError(
                "Wheel objects or wheel child objects are not properly assigned in the VehicleWheelAnimator script.");
        }

        // Initialize child objects if not set in the editor
        for (int i = 0; i < wheelObjects.Length; i++)
        {
            if (wheelChildObjects[i] == null && wheelObjects[i].transform.childCount > 0)
            {
                wheelChildObjects[i] = wheelObjects[i].transform.GetChild(0).gameObject;
            }
        }
    }

    public override void OnUpdateView(QuantumGame game)
    {
        var frame = game.Frames.Predicted;
        var vehicleController = frame.Get<VehicleController3D>(_entityView.EntityRef);
        for (int i = 0; i < wheelObjects.Length; i++)
        {
            var wheelInfo = vehicleController.WheelInfos[i];
            if (wheelInfo.IsGrounded)
            {
                Vector3 hitPoint = wheelInfo.LocalHitPoint.ToUnityVector3();
                wheelObjects[i].transform.localPosition = hitPoint;
            }
            ApplyTireSpin(vehicleController, i);
            ApplySteeringAngle(vehicleController, i);

        }
    }

    private void ApplySteeringAngle(VehicleController3D vehicleController, int i)
    {
        // Get the steering angle from the vehicle controller
        float steeringAngle = vehicleController.WheelInfos[i].TireAngle.AsFloat; // Assuming TireAngle is in radians, convert it to degrees

        // Apply the steering angle to the wheel object's y rotation
        if (wheelObjects[i] != null)
        {
            Quaternion currentRotation = wheelObjects[i].transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, steeringAngle, currentRotation.eulerAngles.z);
            wheelObjects[i].transform.localRotation = targetRotation;
        }
    }


    private void ApplyTireSpin(VehicleController3D vehicleController, int i)
    {
        // Update wheel rotation based on acceleration and speed
        float accelerationFactor = vehicleController.Throttle.AsFloat /* Get acceleration input */;
        float speedFactor = vehicleController.Velocity.Magnitude.AsFloat;
        float rotationThisFrame = CalculateWheelRotation(accelerationFactor, speedFactor);
        wheelRotations[i] += rotationThisFrame;

        // Apply the rotation to the wheel child object
        if (wheelChildObjects[i] != null)
        {
            wheelChildObjects[i].transform.localRotation = Quaternion.Euler(wheelRotations[i], 0, 0);
        }
    }

    private float CalculateWheelRotation(float accelerationInput, float speed)
    {
        float rotationSpeed = speed * speedSensitivity;

        // Check if the rotation speed is below a certain threshold
        if (rotationSpeed < maxWheelSpinSpeed)
        {
            // If below threshold, add extra rotation based on acceleration input
            rotationSpeed += accelerationInput * accelerationSensitivity;
        }

        float rotationThisFrame = rotationSpeed * Time.fixedDeltaTime;
        return rotationThisFrame;
    }

}