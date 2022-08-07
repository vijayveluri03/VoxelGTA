using System.Collections;
using System.Collections.Generic;


namespace GTA
{
    public class CollisionProcessor
    {
        public void Init()
        {
        }

        public void ProcessCollision(Core.Collidable a, Core.Collidable b)
        {
            Core.QLogger.Assert(a != null && b != null);
            var ojecttype = (a.CollisionContext as CollisionContext).Type;
            switch (ojecttype)
            {
                case Constants.Collision.Type.PLAYER_CHARACTER:

                    //if((b.CollisionContext as CollisionContext).Owner is ItemOnMap)
                    //{
                    //    iPlayer player = (a.CollisionContext as PlayerCollisionContext).Player;
                    //    ItemOnMap itemOnMap = (b.CollisionContext as CollisionContext).Owner as ItemOnMap;
                    //    if (player.WeaponInventory.CanICollect(itemOnMap.itemOnMapType, itemOnMap.count))
                    //    {
                    //        player.WeaponInventory.Collect(itemOnMap.itemOnMapType, itemOnMap.count);
                    //        itemOnMap.OnItemCollected();

                    //        Core.QLogger.LogWarning("Item collected : " + b.GetName());
                    //    }
                    //}
                    break;
                case Constants.Collision.Type.WATER:
                    break;
                case Constants.Collision.Type.WOOD:
                    break;
            }
            //if (a.IsThisPlayer)
            {
                //Core.QLogger.Assert ( a.listener != null && a.listener is iPlayer );

                //iPlayer player = a.listener as iPlayer;
                //Core.QLogger.LogWarning("Receieved a collision event from " + b.GetName());
                //if ( b is ItemOnMap )
                //{
                //    ItemOnMap itemOnMap = b as ItemOnMap;
                //     if ( player.WeaponInventory.CanICollect( itemOnMap.itemOnMapType, itemOnMap.count )  )
                //     {
                //        player.WeaponInventory.Collect ( itemOnMap.itemOnMapType, itemOnMap.count );
                //        itemOnMap.OnItemCollected();

                //        Core.QLogger.LogWarning("Item collected : " + b.GetName());
                //     }
                //}
            }
        }
    }
}