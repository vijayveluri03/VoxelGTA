using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Collidable : MonoBehaviour
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            var otherCollidable = other.GetComponent<Collidable>();
            if( otherCollidable == null)
            {
                Core.QLogger.LogWarning("Collidable script not found in collider. So collision ignored for " + other.name);
                return;
            }

            if (logTriggers || true)
                Core.QLogger.LogWarning("Collision trigger enter (Collidable) : " + this.name);

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
                if (m_CollisionContext == null)
                    m_CollisionContext = GetComponent<ICollisionContext>();
                return m_CollisionContext;
            }

            set
            {
                m_CollisionContext = value;
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

        private ICollisionContext m_CollisionContext;
        private bool logTriggers = false;
        private bool isInCoolDownTillEndOfFrame = false;
    }
}