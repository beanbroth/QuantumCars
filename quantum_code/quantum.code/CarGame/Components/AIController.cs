using System;
using System.Collections.Generic;
using Photon.Deterministic;
using System.Runtime.InteropServices;

#nullable disable
namespace Quantum
{
    public unsafe partial struct AIController : IComponent
    {
        public FPVector2 SteerTowardsCurrentNode(Transform3D* currentTransform3D)
        {
            FPVector3 targetPosition = CurrentNode.Position;
            return CalculateSteeringDirection(currentTransform3D->Position, targetPosition, currentTransform3D->Rotation);
        }

        private FPVector2 CalculateSteeringDirection(FPVector3 currentPosition, FPVector3 targetPosition, FPQuaternion currentRotation)
        {
            FPVector3 forwardDirection = currentRotation * FPVector3.Forward;
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
            FP accelerationInput = 1;

            Draw.Ray(currentPosition, toTarget * 3, ColorRGBA.Magenta);

            return HelperFunctions.ClampVectorComponentsMinus1To1(new FPVector2(turnInput, accelerationInput));
        }

        public bool ReachedNode(Transform3D* currentTransform3D)
        {
            FPVector3 targetPosition = CurrentNode.Position;
            FP distanceToTarget = FPVector3.Distance(currentTransform3D->Position, targetPosition);
            return distanceToTarget < NodeReachedDistance;
        }
        public void SetCurrentNode(Graph graph)
        {
            if (CurrentNode.NeighborCount == 0)
            {
                return;
            }

            int randomIndex = new Random().Next(CurrentNode.NeighborCount);

    
            int nextNodeId = CurrentNode.Neighbors[randomIndex];
            if (nextNodeId == 0)
            {
                Log.Error("Next node ID is 0, meaning the car is trying to go to the origin node. Bad!");
            }
            
            CurrentNode = graph.nodes[nextNodeId];
        }
    }
}