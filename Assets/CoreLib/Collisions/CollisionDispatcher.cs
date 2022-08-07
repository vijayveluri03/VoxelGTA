using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class CollisionDispatcher : Singleton<CollisionDispatcher>
    {

        public System.Action<Collidable, Collidable> CollsionHandlers;
        public void OnCollision(Collidable source, Collidable target)
        {

            if (CollsionHandlers == null)
            {
                Core.QLogger.LogWarning("there ar no listeners setup!!!! ", false);
                return;
            }
            CollsionHandlers.Invoke(source, target);

        }
    }
}
