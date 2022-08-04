﻿using System.Collections;
using System.Collections.Generic;


namespace GTA
{
    public class CollisionProcessor 
{
    // Collision processor needs access to session as it needs to process multiple things in the future 
    public void Init( InGameState session )
    {
        this.session = session;
    }
    public void ProcessCollision ( iInteractable a, iInteractable b )
    {
        Core.QLogger.LogError("I have received an event to process collisions");
        Core.QLogger.Assert ( a != null && b != null );

        if ( a.IsThisPlayer )
        {
            Core.QLogger.Assert ( a.listener != null && a.listener is iPlayer );

            iPlayer player = a.listener as iPlayer;
            Core.QLogger.LogWarning("Receieved a collision event from " + b.GetName());
            if ( b is ItemOnMap )
            {
                ItemOnMap itemOnMap = b as ItemOnMap;
                 if ( player.WeaponInventory.CanICollect( itemOnMap.itemOnMapType, itemOnMap.count )  )
                 {
                    player.WeaponInventory.Collect ( itemOnMap.itemOnMapType, itemOnMap.count );
                    itemOnMap.OnItemCollected();
                    
                    Core.QLogger.LogWarning("Item collected : " + b.GetName());
                 }
            }
        }
    }

    private InGameState session;
}
}