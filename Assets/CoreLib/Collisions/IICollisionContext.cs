namespace Core
{
    public interface ICollisionContext
    {
        void SetCollidable( Collidable collidable);
    }

    public class ICollisionContextMono : UnityEngine.MonoBehaviour , ICollisionContext
    {
        public void SetCollidable(Collidable collidable)
        {
            this.collidable = collidable;
        }
        public Collidable collidable;
    }
}