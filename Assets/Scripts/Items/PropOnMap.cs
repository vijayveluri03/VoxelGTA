using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    //todo add an editor script to this to make it snap to ground whereever its placed
    public class PropOnMap : MonoBehaviour
    {
        public eInventoryItem itemOnMapType;
        public int count = 1;
        public bool hover;
        public bool isCollectable = false;

        public PropOnMap() : base()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            SpawnAndAlign();

            if (hover)
                PropFactory.MakeObjectHover(wrapperObject);

            if (isCollectable)
                MakeItCollectable();
        }

        private void SpawnAndAlign()
        {
            Core.QLogger.Assert(spawnedItem == null && wrapperObject == null );

            // Wrapper object, to add any custom scripts like hover, or particles 
            wrapperObject = new GameObject();
            wrapperObject.name = "wrapper";
            wrapperObject.transform.position = transform.position;
            wrapperObject.transform.rotation = transform.rotation;
            wrapperObject.transform.parent = transform;

            spawnedItem = PropFactory.Spawn(itemOnMapType);
            spawnedItem.transform.position = transform.position;
            spawnedItem.transform.rotation = transform.rotation;
            spawnedItem.transform.parent = wrapperObject.transform;
        }

        private void MakeItCollectable()
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radiusOfCollider;
            collider.isTrigger = true;

            //adding collidables
            var colidable = gameObject.AddComponent<Core.Collidable>();
            gameObject.AddComponent<GTACollisionContext>();
            (colidable.CollisionContext as GTACollisionContext).Owner = this;
            (colidable.CollisionContext as GTACollisionContext).Type = Constants.Collision.Type.ITEM_ON_MAP;
        }

        public void OnItemCollected()
        {
            StartCoroutine(ShowPEAndDestroySelf(1));
        }

        IEnumerator ShowPEAndDestroySelf( int waitForSeconds )
        {
            yield return new WaitForSeconds(waitForSeconds);

            // TODO - particles

            GameObject.Destroy(this.gameObject);
            yield break;
        }

        private GameObject wrapperObject = null;
        private GameObject spawnedItem = null;
        private const float radiusOfCollider = 0.75f;
    }
}