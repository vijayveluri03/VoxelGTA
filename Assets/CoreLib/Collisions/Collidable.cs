using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Collidable : MonoBehaviour
    {
        protected void OnTriggerEnter(Collider other)
        {
            var otherCollidable = other.GetComponent<Collidable>();
            if( otherCollidable == null)
            {
                Core.QLogger.LogWarning("Collidable script not found in collider. So collision ignored for " + other.name);
                return;
            }

            if (logTriggers)
                Core.QLogger.LogWarning("Collision trigger enter (Collidable) : " + this.name);

            CollisionDispatcher.Instance.OnCollisionNotify(this, other.GetComponent<Collidable>());
        }

        void OnCollisionEnter(Collision collision)
        {
            var other = collision.collider;
            var otherCollidable = other.GetComponent<Collidable>();
            if (otherCollidable == null)
            {
                Core.QLogger.LogWarning("Collidable script not found in collider. So collision ignored for " + other.name);
                return;
            }

            if (logTriggers)
                Core.QLogger.LogWarning("Collision enter (Collidable) : " + this.name);

            CollisionDispatcher.Instance.OnCollisionNotify(this, other.GetComponent<Collidable>());
        }

        public virtual string GetName()
        {
            return gameObject.name;
        }

        public ICollisionContext CollisionContext
        {
            get
            {
                if (m_collisionContext == null)
                {
                    if (m_collisionContextScript != null)
                        m_collisionContext = m_collisionContextScript;
                    else 
                        m_collisionContext = GetComponent<ICollisionContext>();
                }
                return m_collisionContext;
            }

            set
            {
                m_collisionContext = value;
            }
        }

        public void LateUpdate()
        {
            isInCoolDownTillEndOfFrame = false;
        }

        public void EnterCooldown()
        {
            isInCoolDownTillEndOfFrame = true;
        }
        public bool IsInCoolDown()
        {
            return isInCoolDownTillEndOfFrame;
        }

        private ICollisionContext m_collisionContext;
        [SerializeField] private ICollisionContextMono m_collisionContextScript; 
        private bool logTriggers = false;
        private bool isInCoolDownTillEndOfFrame = false;
    }
}