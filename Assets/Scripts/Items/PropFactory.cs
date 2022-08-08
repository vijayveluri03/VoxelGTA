using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public static class PropFactory
    {
        // Refactor needed. just temp code
        public static GameObject Spawn(eInventoryItem itemOnMap)
        {
            if (itemOnMap == eInventoryItem.Pistol)
            {
                return GameObject.Instantiate(GetObject(itemOnMap));
            }
            Core.QLogger.Assert(false);
            return null;
        }

        public static UnityEngine.GameObject GetObject(eInventoryItem itemOnMap)
        {
            if (itemOnMap == eInventoryItem.Pistol)
            {
                return (Core.ResourceManager.Instance.LoadAsset<GameObject>("Characters/Items/Pistol/Pistol"));
            }
            Core.QLogger.Assert(false);
            return null;
        }

        public static void MakeObjectHover(GameObject gameObject)
        {
            HoverAndRotate hr = gameObject.AddComponent<HoverAndRotate>();
            hr.hoverRange = 0.2f;
            hr.minHoverDistanceAboveGround = 0.2f;
        }
    }
}