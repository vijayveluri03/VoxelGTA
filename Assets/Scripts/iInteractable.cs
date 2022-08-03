using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public abstract class iInteractable : MonoBehaviour
    {
        public virtual void Init(System.Action<iInteractable, iInteractable> collisionProcessor, iListener listener)
        {
            this.collisionProcessor = collisionProcessor;
            this.listener = listener;

            Core.QLogger.Assert(this.collisionProcessor != null, "This is not registered. So youwill not log this collision");
        }
        public Collider GetCollider()
        {
            if (collidr == null)
                collidr = GetComponent<Collider>();

            Core.QLogger.Assert(collidr != null);

            return collidr;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (collisionProcessor != null)
                collisionProcessor(this, other.GetComponent<iInteractable>());
            else
            {
                if (IsThisPlayer)
                    Core.QLogger.LogWarning("Collision processor not set for type " + GetName());
            }
        }

        public abstract string GetName();
        public virtual bool IsThisPlayer { get { return false; } }
        private System.Action<iInteractable, iInteractable> collisionProcessor = null;
        public Collider collidr { get; private set; }
        public iListener listener { get; private set; }
    }
}