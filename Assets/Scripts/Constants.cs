using System.Collections;
using System.Collections.Generic;


namespace GTA
{
    public static class Constants
    {
        public static class SOKeys
        {
            public const string UIManager = "uimanager";
            public const string CameraSystem = "cameraSystem";
            public const string InputSystem = "inputSystem";
        };

        public static class Collision
        {
            public enum Type
            {
                PLAYER_CHARACTER,
                WOOD,
                WATER,
                ITEM_ON_MAP,
            };

            public const string CollidableLayer = "Collidable";
            public const string GroundLayer = "Ground";

            public static UnityEngine.LayerMask CollidableLayerMask
            {
                get
                {
                    if (!collidableMask.HasValue)
                        collidableMask = UnityEngine.LayerMask.GetMask( new string[]{ CollidableLayer, GroundLayer });
                    return collidableMask.Value;
                }
            }
            private static UnityEngine.LayerMask? collidableMask = null;

            public const int MaxDistanceForWeaponTargetCheck = 100;
        };
    }
}