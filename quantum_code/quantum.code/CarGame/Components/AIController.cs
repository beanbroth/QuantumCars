using System;
using System.Collections.Generic;
using Photon.Deterministic;
using System.Runtime.InteropServices;

#nullable disable
namespace Quantum
{
    public unsafe partial struct AIController : IComponent
    {
        public FPVector2 CalculateSteeringDirection(Transform3D* currentTransform3D, Transform3D targetTransform3D)
        {
            FPVector3 targetPosition = targetTransform3D.Position;
            FPVector3 currentPosition = currentTransform3D->Position;
            FPVector3 forwardDirection = currentTransform3D->Rotation * FPVector3.Forward;
            FPVector3 toTarget = (targetPosition - currentPosition).Normalized;

            // Calculate turning input (X)
            FP angleToTarget = FPVector3.Angle(forwardDirection, toTarget);
            FP turnInput = FPMath.Clamp(angleToTarget / FP.Pi, -1, 1); 

            // Check if the target is to the left or right of the forward direction
            FPVector3 crossProduct = FPVector3.Cross(forwardDirection, toTarget);
            if (crossProduct.Y < 0) // Negative Y indicates target is to the right
            {
                turnInput = -turnInput;
            }

            // Calculate acceleration input (Y)
            // For simplicity, we use a constant value, but it could be a function of distance or alignment.
            FP accelerationInput = 1;

            Draw.Ray(currentPosition, toTarget * 3, ColorRGBA.Magenta);

            return HelperFunctions.ClampVectorComponentsMinus1To1(new FPVector2(turnInput, accelerationInput));
        }
    }
}