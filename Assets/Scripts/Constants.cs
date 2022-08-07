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
        };

        public static class CollidableObjects
        {
            public enum Types
            {
                PLAYER_CHARACTER,
                WOOD,
                WATER,
                ITEM_ON_MAP,

            };
        };
    }

}