using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo add an editor script to this to make it snap to ground whereever its placed
public class ItemOnMap : iInteractable
{
    public eInventoryItem itemOnMapType;
    public int count = 1;

    public bool makeObjectCollectable;

    public ItemOnMap( ) : base ( )
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnedItem = ItemFactory.Spawn ( itemOnMapType );
        
        spawnedItem.transform.position = transform.position;
        spawnedItem.transform.rotation = transform.rotation;

        spawnedItem.transform.parent = transform;
        
        if ( makeObjectCollectable ) 
            ItemFactory.MakeObjectHover( spawnedItem );

        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = radiusOfCollider;
        collider.isTrigger = true;


        //UnityEngine.Component.Destroy ( this );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnItemCollected () 
    {
        StartCoroutine ( ShowPEAndDestroySelf() );
    }

    IEnumerator ShowPEAndDestroySelf ()
    {
        // todo : show some particle effect and destroy self slowly 

        GameObject.Destroy( this.gameObject );
        yield break;
    }

    public override string GetName()
    {
        return "ItemOnMap:" + itemOnMapType;
    }

    private const float radiusOfCollider = 0.75f;
}
