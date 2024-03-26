using System;
using Deform;
using UnityEngine;
using Quantum;
using Unity.Burst;
using Unity.Mathematics;
using LayerMask = UnityEngine.LayerMask;

[BurstCompile]
public class S_VehicleDeformationController : MonoBehaviour
{
    [SerializeField] private EntityView entityView;
    [SerializeField] private Deformable deformer;
    [SerializeField] private MyLatticeDeformer latticeDeformer;
    [SerializeField] private float deformationStrength = 0.1f;
    [SerializeField] private float deformationRadius = 1.0f;
    private bool isDeforming = false; // To prevent continuous deformation

    private void Start()
    {
        QuantumEvent.Subscribe<EventVehicleCollision>(this, OnVehicleCollision);
    }

    private void OnDestroy()
    {
        QuantumEvent.UnsubscribeListener(this);
    }

    private void OnVehicleCollision(EventVehicleCollision callback)
    {
        if (isDeforming)
            return; // Exit if already deforming

        if (entityView.EntityRef!= callback.Entity)
            return;
        
        // Calculate impact force and check if it exceeds a threshold
        float impactForce = callback.ForceScale.AsFloat;
        isDeforming = true; // Set deforming flag
        float scaledStrength = deformationStrength * impactForce;
        Vector3 collisionPoint = callback.Position.ToUnityVector3();
        Vector3 collisionNormal = callback.Normal.ToUnityVector3();
        DeformControlPoints(collisionPoint, collisionNormal, scaledStrength);

        // Reset deforming flag after a delay
        Invoke(nameof(ResetDeformation), 0.01f); // Adjust delay as needed
        deformer.ForceImmediateUpdate();
    }

    private void DeformControlPoints(Vector3 collisionPoint, Vector3 collisionNormal, float strength)
    {
        if (latticeDeformer == null)
            return;
        var localCollisionPoint = latticeDeformer.transform.InverseTransformPoint(collisionPoint);
        var localCollisionPointF3 = new float3(localCollisionPoint.x, localCollisionPoint.y, localCollisionPoint.z);
        var localCollisionNormal = latticeDeformer.transform.InverseTransformDirection(collisionNormal);
        for (int i = 0; i < latticeDeformer.ControlPoints.Length; i++)
        {
            var controlPoint = latticeDeformer.ControlPoints[i];
            float distance = math.distance(localCollisionPointF3, controlPoint);
            if (distance <= deformationRadius)
            {
                var deformationStrength = Mathf.Lerp(strength, 0, distance / deformationRadius);
                var deformationDirection = math.normalize(localCollisionNormal);
                controlPoint += deformationDirection * deformationStrength;
                latticeDeformer.SetControlPointFromIndex(i, controlPoint);
            }
        }
    }

    private void ResetDeformation()
    {
        isDeforming = false;
    }
}