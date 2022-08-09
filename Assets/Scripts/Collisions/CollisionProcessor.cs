using System.Collections;
using System.Collections.Generic;


namespace GTA
{
    public class CollisionProcessor
    {
        public void Init(Session session)
        {
            collisionProcessors.Add(new PlayerCollisionProcessor());
            collisionProcessors.Add(new ProjectileCollisionProcessor());

            this.session = session;
        }

        public void ProcessCollision(Core.Collidable a, Core.Collidable b)
        {
            Core.QLogger.Assert(a != null && b != null);

            // Assuming that collisioncontext is manditory in this game!
            Core.QLogger.Assert(a.CollisionContext != null && b.CollisionContext != null && a.CollisionContext is GTACollisionContext && b.CollisionContext is GTACollisionContext);

            if (a.IsInCoolDown() || b.IsInCoolDown())
            {
                //@todo - enable this 
                //Core.QLogger.LogWarning("Collidable in cooldown in ProcessCollision. If this happens often, we need a different approach!");
                return;
            }

            var aCollisionContext = a.CollisionContext as GTACollisionContext;
            var bCollisionContext = b.CollisionContext as GTACollisionContext;

            bool handled = false;
            foreach (var collisionProcessor in collisionProcessors)
            {
                if (collisionProcessor.TryProcess(this, aCollisionContext, bCollisionContext))
                {
                    handled = true;
                    break;
                }
            }

            //so that the same collision doesnt trigger again in the current frame
            // TODO - not ideal way to do this. a player who is in cooldown wouldnt collide with anything else for that frame.
            // instead make the other collidable to cooldown. like itemonmap can enter cooldown, which wouldnt need to collided again, but not the player
            a.EnterCooldown();
            b.EnterCooldown();

            if( !handled )
                Core.QLogger.LogWarning("Collision unhandled! " + aCollisionContext.Type + "," + bCollisionContext.Type);
        }

        private List<ICollisionProcessor> collisionProcessors = new List<ICollisionProcessor>();
        private Session session;


        // todo - need a better name 
        interface ICollisionProcessor
        {
            bool TryProcess(CollisionProcessor mainProcessor, GTACollisionContext a, GTACollisionContext b);
        }
        public class PlayerCollisionProcessor : ICollisionProcessor
        {
            public bool TryProcess( CollisionProcessor processor, GTACollisionContext a, GTACollisionContext b)
            {
                if (a.Type != Constants.Collision.Type.PLAYER_CHARACTER && b.Type != Constants.Collision.Type.PLAYER_CHARACTER)
                    return false;

                if (a.Type == Constants.Collision.Type.PLAYER_CHARACTER)
                    Process( processor, a, b);
                else
                    Process(processor, b, a);
                return true;
            }
            public void Process(CollisionProcessor mainProcessor, GTACollisionContext me, GTACollisionContext other)
            {
                var player = mainProcessor.session.player;

                if (other.Type == Constants.Collision.Type.ITEM_ON_MAP)
                {
                    Core.QLogger.Assert(other.Owner is PropOnMap);
                    PropOnMap itemOnMap = other.Owner as PropOnMap;

                    if (player.WeaponInventory.CanICollect(itemOnMap.itemOnMapType, itemOnMap.count))
                    {
                        player.WeaponInventory.Collect(itemOnMap.itemOnMapType, itemOnMap.count);
                        itemOnMap.OnItemCollected();
                        player.OnWeaponCollected(itemOnMap.itemOnMapType);

                        Core.QLogger.LogInfo("Item collected : " + other.Name + " of type " + other.Type);
                    }
                }
                else
                    Core.QLogger.Assert(false);// unhandled!
            }
        }

        public class ProjectileCollisionProcessor : ICollisionProcessor
        {
            public bool TryProcess(CollisionProcessor processor, GTACollisionContext a, GTACollisionContext b)
            {
                if (a.Type != Constants.Collision.Type.PROJECTILE && b.Type != Constants.Collision.Type.PROJECTILE)
                    return false;

                if (a.Type == Constants.Collision.Type.PROJECTILE)
                    Process(processor, a, b);
                else
                    Process(processor, b, a);
                return true;
            }

            public void Process(CollisionProcessor mainProcessor, GTACollisionContext me, GTACollisionContext other)
            {
                if (other.Type == Constants.Collision.Type.WOOD)
                {
                    (me as Bullet).SelfDestroy();
                    // todo - particles
                }
                else if (other.Type == Constants.Collision.Type.GROUND)
                {
                    (me as Bullet).SelfDestroy();
                    // todo - particles
                }
                else
                    Core.QLogger.Assert(false);// unhandled!
            }
        }
    }

}