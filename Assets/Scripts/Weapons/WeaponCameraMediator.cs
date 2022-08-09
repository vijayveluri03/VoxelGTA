using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public class WeaponCameraMediator
    {

        public WeaponCameraMediator(ThirdPersonCamera camera, Weapon controller)
        {
            this.camera = camera;
            this.weaponController = controller;
        }

        private ThirdPersonCamera camera;
        private Weapon weaponController;

        public Vector3 GetWeaponDirection( Vector3 weaponPosition )
        {
            Ray ray = new Ray(camera.Position, camera.LookAtDirection);
            RaycastHit hitInfo;
            Vector3 targetPosition;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo, Constants.Collision.MaxDistanceForWeaponTargetCheck, Constants.Collision.CollidableLayerMask))
            {
                targetPosition = camera.Position + camera.LookAtDirection * hitInfo.distance;
            }
            else
                targetPosition = camera.Position + camera.LookAtDirection * Constants.Collision.MaxDistanceForWeaponTargetCheck;

            return (targetPosition - weaponPosition).normalized;
        }
    }
}
