﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GTA
{
    public static class PropFactory
    {
        // Refactor needed. just temp code
        public static GameObject Spawn(eInventoryItem itemOnMap)
        {
            return GameObject.Instantiate(GetObject(itemOnMap));
        }

        public static UnityEngine.GameObject GetObject(eInventoryItem itemOnMap)
        {
            if (itemOnMap == eInventoryItem.Pistol)
            {
                return (Core.ResourceManager.Instance.LoadAsset<GameObject>("Characters/Items/Pistol/Pistol"));
            }
            else if (itemOnMap == eInventoryItem.Automatic_Pistol)
            {
                return (Core.ResourceManager.Instance.LoadAsset<GameObject>("Characters/Items/Pistol/Auto_Pistol"));
            }
            else if (itemOnMap == eInventoryItem.Deagle)
            {
                return (Core.ResourceManager.Instance.LoadAsset<GameObject>("Characters/Items/Pistol/DEagle"));
            }

            Core.QLogger.Assert(false);
            return null;
        }

        //@todo - why is this here ?
        public static void MakeObjectHover(GameObject gameObject)
        {
            HoverAndRotate hr = gameObject.AddComponent<HoverAndRotate>();
            hr.hoverRange = 0.2f;
            hr.minHoverDistanceAboveGround = 0.2f;
        }
    }
}