using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    using CollisionDelegate = System.Action<Collidable, Collidable>;

    public class CollisionDispatcher : Singleton<CollisionDispatcher>
    {
        private CollisionDelegate collisionHandlers;

        public void Register (CollisionDelegate callback)
        {
            collisionHandlers += callback;
        }
        public void UnRegister(CollisionDelegate callback)
        {
            collisionHandlers -= callback;
        }

        public void OnCollisionNotify(Collidable source, Collidable target)
        {
            if (collisionHandlers == null)
            {
                QLogger.LogWarning("there are no listeners setup ", false);
                return;
            }
            if (collisionHandlers != null)
                collisionHandlers.Invoke(source, target);
        }
    }
}
