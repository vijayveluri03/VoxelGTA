using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public class GTACollisionContext : MonoBehaviour, Core.ICollisionContext
    {
        public void Start()
        {
            this.enabled = false; // because we dont need the context to update everyframe
        }

        public string Name;
        public Constants.Collision.Type Type;
        public AudioClip Audio;
        public GameObject ParticleFX;
        public System.Object Owner;
    }
}
