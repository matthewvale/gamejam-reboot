/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public abstract class BulletBase : MonoBehaviour
    {

        public abstract float Damage { get; set; }

        public abstract void OnTriggerEnter(Collider collisionData);

        public abstract void OnCollisionEnter(Collision collisionData);

    }

}