using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Collidable : MonoBehaviour
    {

        protected virtual void OnTriggerEnter(Collider other)
        {
            Core.CollisionDispatcher.Instance.OnCollision(this, other.GetComponent<Collidable>());
        }

        public virtual string GetName() { return "CollisionListener from gameobject :" + gameObject.name; }
        private ICollisionContext m_CollisionContext;
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
            
    }
}