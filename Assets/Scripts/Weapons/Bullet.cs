﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    public class Bullet : GTACollisionContext
    {
        public override bool DisableOnStart { get { return false; } }
        public float speed;

        public void Init(Vector3 direction, float damage, float range)
        {
            this.direction = Vector3.Normalize(direction);
            this.damage = damage;
            this.range = range;
        }
        // Update is called once per frame
        void Update()
        {
            distanceCovered = this.speed * Time.deltaTime;
            this.transform.position += this.speed * Time.deltaTime * this.direction;

            if (distanceCovered > range)
                SelfDestroy();
        }
        public void SelfDestroy()
        {
            GameObject.Destroy(gameObject);
        }


        private float distanceCovered = 0;
        private Vector3 direction;
        private float damage;
        private float range;
    }
}