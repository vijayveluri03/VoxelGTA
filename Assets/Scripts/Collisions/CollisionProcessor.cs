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
                Core.QLogger.LogWarning("Collidable in cooldown in ProcessCollision. If this happens often, we need a different approach!");
                return;
            }

            foreach (var collisionProcessor in collisionProcessors)
            {
                if (collisionProcessor.TryProcess(this, a, b))
                    break;
            }

            //so that the same collision doesnt trigger again in the current frame
            // TODO - not ideal way to do this. a player who is in cooldown wouldnt collide with anything else for that frame.
            // instead make the other collidable to cooldown. like itemonmap can enter cooldown, which wouldnt need to collided again, but not the player
            a.EnterCooldown();
            b.EnterCooldown();

            Core.QLogger.LogWarning("Collision unhandled!");
        }

        private List<ICollisionProcessor> collisionProcessors = new List<ICollisionProcessor>();
        private Session session;


        // todo - need a better name 
        interface ICollisionProcessor
        {
            bool TryProcess(CollisionProcessor mainProcessor, Core.Collidable a, Core.Collidable b);
        }
        public class PlayerCollisionProcessor : ICollisionProcessor
        {
            public bool TryProcess( CollisionProcessor processor, Core.Collidable a, Core.Collidable b)
            {
                GTACollisionContext aCollisionContext = a.CollisionContext as GTACollisionContext;
                GTACollisionContext bCollisionContext = b.CollisionContext as GTACollisionContext;

                if (aCollisionContext.Type != Constants.Collision.Type.PLAYER_CHARACTER && bCollisionContext.Type != Constants.Collision.Type.PLAYER_CHARACTER)
                    return false;

                if (aCollisionContext.Type == Constants.Collision.Type.PLAYER_CHARACTER)
                    Process( processor, aCollisionContext, bCollisionContext);
                else
                    Process(processor, bCollisionContext, aCollisionContext);
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
            public bool TryProcess(CollisionProcessor processor, Core.Collidable a, Core.Collidable b)
            {
                return false;
                //GTACollisionContext aCollisionContext = a.CollisionContext as GTACollisionContext;
                //GTACollisionContext bCollisionContext = b.CollisionContext as GTACollisionContext;

                //if (aCollisionContext.Type != Constants.Collision.Type.PLAYER_CHARACTER && bCollisionContext.Type != Constants.Collision.Type.PLAYER_CHARACTER)
                //    return false;

                //if (aCollisionContext.Type == Constants.Collision.Type.PLAYER_CHARACTER)
                //    Process(b);
                //else
                //    Process(a);
                //return true;
            }
            public void Process(Core.Collidable other)
            {

            }
        }
    }

}