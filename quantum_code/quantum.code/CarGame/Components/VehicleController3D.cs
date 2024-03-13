using System;
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
            input.X = FPMath.Clamp(input.X, -1, 1);
            input.Y = FPMath.Clamp(input.Y, -1, 1);
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
            
            Velocity = carPhysicsBody->Velocity;
        }

        private void ProcessWheel(FrameBase frame, PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, int wheelIndex)
        {
            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;
            var tireRadius = wheelConfig.tireRadius; // Added tire radius parameter

            // Increase raycast length by the tire radius
            var raycastLength = rigConfig.rideHeight + tireRadius;
    
            var tireHit = frame.Physics3D.Raycast(wheelWorldPosition, FPVector3.Down, raycastLength, -1, QueryOptions.HitStatics);

            WheelInfos[wheelIndex].TireAngle = rigConfig.wheelIsSteering[wheelIndex] ? SteeringAngle : 0;
            
            if (tireHit.HasValue)
            {
                WheelInfos[wheelIndex].IsGrounded = true;
                ApplySuspensionForces(carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
                ApplyTireShearForces(frame, carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
                ApplyEngineForces(carPhysicsBody, transform, rigConfig, wheelConfig, tireHit.Value, wheelIndex);
            }
            else
            {
                WheelInfos[wheelIndex].IsGrounded = false;
                WheelInfos[wheelIndex].LocalHitPoint = FPVector3.Zero;
            }
        }


        private void ApplyTireShearForces(FrameBase frame, PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            //Draw.Ray(origin, direction, color)
            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;

            // Determine if the wheel is a steering wheel
            FPVector3 steeringDirection = transform->Right;
            if (rigConfig.wheelIsSteering[wheelIndex])
            {
                // Calculate the steering direction based on the steering angle
                FP steerAngleRadians = FP.Deg2Rad * -SteeringAngle;
                FPVector3 carForward = transform->Forward;
                FPVector3 carRight = transform->Right;
                steeringDirection = FPMath.Cos(steerAngleRadians) * carRight +
                                    FPMath.Sin(steerAngleRadians) * carForward;
            }

            FPVector3 tireWorldVel = carPhysicsBody->GetPointVelocity(wheelWorldPosition, transform);
            FP steeringVel = FPVector3.Dot(steeringDirection, tireWorldVel);
            
            FP desiredVelChange = -steeringVel * wheelConfig.tireGripFactor.Evaluate(FPMath.Abs(steeringVel));
            FP desiredAccel = desiredVelChange / frame.DeltaTime;
            carPhysicsBody->AddForceAtPosition(steeringDirection * (wheelConfig.tireMass * desiredAccel),
                wheelWorldPosition, transform);

// Draw wheel's right vector
            Draw.Ray(wheelWorldPosition, (transform->Right), ColorRGBA.Red);

// Draw steering direction
            Draw.Ray(wheelWorldPosition, (steeringDirection), ColorRGBA.Blue);

// Draw tire world velocity
            Draw.Ray(wheelWorldPosition, (tireWorldVel), ColorRGBA.Green);

// Draw force applied on the tire
            FPVector3 forceVector = (steeringDirection * (wheelConfig.tireMass * desiredAccel));
            Draw.Ray(wheelWorldPosition, forceVector, ColorRGBA.Yellow);
        }

        private void ApplySuspensionForces(PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            var springStrength = wheelConfig.springStrength;
            var springDamper = wheelConfig.springDamper;
            var tireRadius = wheelConfig.tireRadius; // Added tire radius parameter
            var rideHeight = rigConfig.rideHeight + tireRadius; // Adjusting rideHeight based on tire radius

            FPVector3 wheelLocalPosition = rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 wheelWorldPosition = transform->Position + transform->Rotation * wheelLocalPosition;
            FPVector3 springDirection = transform->Up;
            FPVector3 tireWorldVel = carPhysicsBody->GetPointVelocity(wheelWorldPosition, transform);

            WheelInfos[wheelIndex].LocalHitPoint = transform->InverseTransformPoint(tireHit.Point);

            FP offset = rideHeight - tireHit.CastDistanceNormalized * rideHeight;
            FP vel = FPVector3.Dot(tireWorldVel, springDirection);
            FP springForce = (offset * springStrength) - (vel * springDamper);
            carPhysicsBody->AddForceAtPosition(springDirection * springForce, wheelWorldPosition, transform);
        }


        private void ApplyEngineForces(PhysicsBody3D* carPhysicsBody, Transform3D* transform,
            VehicleController3DConfig rigConfig, WheelControllerConfig wheelConfig, Hit3D tireHit, int wheelIndex)
        {
            if(!rigConfig.wheelIsPowered[wheelIndex])
                return;
            FPVector3 wheelWorldPosition =
                transform->Position + transform->Rotation * rigConfig.wheelLocalPositions[wheelIndex];
            FPVector3 accelDirection = transform->Forward;
            if (rigConfig.wheelIsSteering[wheelIndex])
            {
                FP steerAngleRadians = FP.Deg2Rad * SteeringAngle;
                FPVector3 carForward = transform->Forward;
                FPVector3 carRight = transform->Right;
                accelDirection = FPMath.Cos(steerAngleRadians) * carForward + FPMath.Sin(steerAngleRadians) * carRight;
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