using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    public class Bullet : GTACollisionContext
    {
        public override bool DisableOnStart { get { return false; } }
        public float Speed { get { return speed; } }
        public Vector3 Direction {  get { return direction; } }
        public Vector3 Position { get { return transform.position; } }

        public void Init(Vector3 direction, float damage, float range)
        {
            this.direction = Vector3.Normalize(direction);
            this.damage = damage;
            this.range = range;
        }
        // Update is called once per frame
        void Update()
        {
            distanceCovered = this.Speed * Time.deltaTime;
            this.transform.position += this.Speed * Time.deltaTime * this.direction;

            if (distanceCovered > range)
                SelfDestroy();
        }
        public void SelfDestroy()
        {
            GameObject.Destroy(gameObject);
        }


        private float distanceCovered = 0;
        private Vector3 direction;
        [SerializeField] private float speed;
        private float damage;
        private float range;
    }
}