using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    //todo add an editor script to this to make it snap to ground whereever its placed
    public class ItemOnMap : MonoBehaviour
    {
        public eInventoryItem itemOnMapType;
        public int count = 1;

        public bool makeObjectCollectable;

        public ItemOnMap() : base()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            GameObject spawnedItem = ItemFactory.Spawn(itemOnMapType);

            spawnedItem.transform.position = transform.position;
            spawnedItem.transform.rotation = transform.rotation;

            spawnedItem.transform.parent = transform;

            if (makeObjectCollectable)
                ItemFactory.MakeObjectHover(spawnedItem);

            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = radiusOfCollider;
            collider.isTrigger = true;

            //adding collidables
            var colidable = gameObject.AddComponent<Core.Collidable>();
            gameObject.AddComponent<CollisionContext>();
            (colidable.CollisionContext as CollisionContext).Owner = this;
            (colidable.CollisionContext as CollisionContext).Type = Constants.Collision.Type.ITEM_ON_MAP;




            //UnityEngine.Component.Destroy ( this );
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnItemCollected()
        {
            StartCoroutine(ShowPEAndDestroySelf());
        }

        IEnumerator ShowPEAndDestroySelf()
        {
            // todo : show some particle effect and destroy self slowly 

            GameObject.Destroy(this.gameObject);
            yield break;
        }

        

        private const float radiusOfCollider = 0.75f;
    }
}