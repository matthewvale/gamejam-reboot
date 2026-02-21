/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public interface IDamageHandler
    {
        public void DoDamage(float amount, out bool destroySource, out bool targetDestroyed, Vector3? hitPoint);

        public float GetHealth();

        public float GetSecondaryHealth();

        public float GetMaxHealth();

        public float GetThreatLevel();
    }

    public struct DamageData
    {
        public IDamageHandler DamageHandler;
        public Transform TargetTransform;
        public Vector3 TargetPoint;
        public Rigidbody TargetRigidbody;

        public readonly bool IsValid => (Object)DamageHandler != null;
    }

}