using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Core;
using System.Runtime.CompilerServices;
using Quantum.Collections;
using Quantum.Physics3D;

#nullable disable
namespace Quantum
{
    public unsafe partial struct VehicleController3D : IComponent
    {
        public void UpdateInternalVariables(FrameBase frame, EntityRef entity, VehicleController3DConfig rigConfig,
            FPVector2 input)
        {
            SteeringAngle = input.X * rigConfig.maxSteerAngle;
            Throttle = input.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePhysics(FrameBase frame, PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, List<WheelControllerConfig> wheelConfigs)
        {
            if (carPhysicsBody == null || transform == null)
            {
                Log.Error("CarPhysicsBody or Transform3D is null.");
                return;
            }

            for (int i = 0; i < rigConfig.numWheels; i++)
            {
                ProcessWheel(frame, carPhysicsBody, transform, rigConfig, wheelConfigs[i], i);
            }
        }

        private void ProcessWheel(FrameBase frame, PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, int wheelIndex)
        {
            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;
            
            var tireHit = frame.Physics3D.Raycast(wheelWorldPosition, FPVector3.Down, rigConfig.rideHeight);
            if (!tireHit.HasValue)
                return;
            ApplySuspensionForces(carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
            
            ApplyTireShearForces(frame, carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
            
            ApplyEngineForces(carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
            
        }

        private void ApplyTireShearForces(FrameBase frame, PhysicsBody3D* carPhysicsBody, Transform3D* transform, VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            // Convert the Unity types and methods to Quantum's equivalent
            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;

            // Quantum uses FPVector3 instead of Vector3
            FPVector3 steeringDirection = transform->Right;

            // Getting the point velocity at the wheel's position
            FPVector3 tireWorldVel = carPhysicsBody->GetPointVelocity(wheelWorldPosition, transform);

            // Quantum uses FP (fixed point) instead of float
            FP steeringVel = FPVector3.Dot(steeringDirection, tireWorldVel);

            // Use Quantum's FPMath.Abs for absolute value
            FP desiredVelChange = -steeringVel * wheelConfig.tireGripFactor.Evaluate(FPMath.Abs(steeringVel));

            // Quantum uses its own time step, typically accessed through frame.DeltaTime
            FP desiredAccel = desiredVelChange / frame.DeltaTime;

            // Apply force at wheel's position
            carPhysicsBody->AddForceAtPosition(steeringDirection * (wheelConfig.tireMass * desiredAccel), wheelWorldPosition, transform);
        }

        private void ApplySuspensionForces(PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            var springStrength = wheelConfig.springStrength;
            var springDamper = wheelConfig.springDamper;
            var rideHeight = rigConfig.rideHeight;
            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;
            FPVector3 springDirection = transform->Up;
            FPVector3 tireWorldVel = carPhysicsBody->GetPointVelocity(wheelWorldPosition, transform);
            
            FP offset = rideHeight - tireHit.CastDistanceNormalized * rideHeight;

 
            FP vel = FPVector3.Dot(tireWorldVel, springDirection);
            FP springForce = (offset * springStrength) - (vel * springDamper);

    
            carPhysicsBody->AddForceAtPosition(springDirection * springForce, wheelWorldPosition, transform);

   
        }

        private void ApplyEngineForces(PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            FPVector3 wheelWorldPosition =
                transform->Position + transform->Rotation * rigConfig.wheelLocalPositions[wheelIndex];

            FPVector3 accelDirection = transform->Forward;
            if (rigConfig.wheelIsSteering[wheelIndex])
            {
                FP steerAngleRadians = FP.Deg2Rad * SteeringAngle;
                FPVector3 carForward = transform->Forward;
                FPVector3 carRight = transform->Right;
                accelDirection =
                    FPMath.Cos(steerAngleRadians) * carForward + FPMath.Sin(steerAngleRadians) * carRight;
            }
      
            
            FP carDrivingDirectionSpeed = FPVector3.Dot(carPhysicsBody->Velocity, transform->Forward);
            FP VelocityPercentOfMax = FPMath.Clamp01(FPMath.Abs(carDrivingDirectionSpeed) / rigConfig.maxSpeed);

            // if normalized speed is greater than 1, we are going faster than max speed, so we should not apply any more force
            if (VelocityPercentOfMax > FP._0_99)
            {
                return;
            }

            // evaluate torque curve based on normalized speed (enforce max speed, launch control)
            FP availableTorque = Throttle * EvaluateSpeedTorqueCurve(rigConfig.speedTorqueCurve, VelocityPercentOfMax) *
                                 rigConfig.maxTorque;
            carPhysicsBody->AddForceAtPosition(accelDirection * availableTorque, wheelWorldPosition, transform);
            // lastEngineForce equivalent might be needed if you use this value elsewhere
        }

        private FP EvaluateSpeedTorqueCurve(FPAnimationCurve curve, FP velocityPercent)
        {
            // Convert the FP to float for the Unity AnimationCurve evaluation
            FP percent = velocityPercent;
            FP evaluatedValue = curve.Evaluate(percent);

            // Convert the result back to FP
            return evaluatedValue;
        }
    }
}