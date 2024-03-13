
using Photon.Deterministic;

namespace Quantum
{
    //TODO: Implement vehicle damage logic
    public unsafe partial struct VehicleDamage : IComponent
    {

        public void TakeDamage(PhysicsBody3D* carPhysicsBody, Transform3D* transform, FPVector3 impactVector, FPVector3 pointOfImpact)
        {
            // Calculate damage based on the magnitude of the impact vector
            //FP damage = CalculateDamage(impactVector);
            //ApplyDamage(damage);

            // Apply force at the point of impact
            ApplyImpactForce(carPhysicsBody, transform, impactVector, pointOfImpact);
        }
        
        private void ApplyImpactForce(PhysicsBody3D* carPhysicsBody, Transform3D* transform, FPVector3 impactVector, FPVector3 pointOfImpact)
        {
            if (carPhysicsBody != null)
            {
                FPVector3 worldPointOfImpact = transform->Position + pointOfImpact;
                FPVector3 force = impactVector; // Adjust this based on how you want to apply the force

                carPhysicsBody->AddForceAtPosition(force, worldPointOfImpact, transform);
            }
        }
    }
}