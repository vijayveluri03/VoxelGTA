using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GTA
{
    
    public class CollisionContext:MonoBehaviour, Core.ICollisionContext
    {
        public string Name;
        public Constants.CollidableObjects.Types m_CollidableObjectType;
        public AudioClip Audio;
        public GameObject ParticleFX;
        public Object Owner;
    }

   
}
