using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eInventoryItem 
{
    Unknown, 
    Pistol,
    Grenade,
    ShotGun
}
public class Inventory 
{
    private Dictionary<eInventoryItem, int> inventoryItems = new Dictionary<eInventoryItem, int>();

    public bool CanICollect ( eInventoryItem item )
    {
        return CanICollect( item, 1 );
    }

    public bool CanICollect ( eInventoryItem item, int count )
    {
        return (GetItemCount( item ) + count) <= GetMaxCollectable( item );
    }

    public int GetItemCount ( eInventoryItem item )
    {
        if ( inventoryItems.ContainsKey (item ))
            return inventoryItems[item];
        return 0;
    }

    // todo move this to design data serialized file. 
    public int GetMaxCollectable ( eInventoryItem item )
    {
        switch ( item )
        {
            case eInventoryItem.Pistol:
                return 2;
                case eInventoryItem.ShotGun:
                return 1;
                case eInventoryItem.Grenade:
                return 5;
        }

        return int.MaxValue;
    }

    public void Collect ( eInventoryItem item, int count )
    {
        if ( inventoryItems.ContainsKey( item ) ) 
            Core.QLogger.Assert( (inventoryItems[item] + count) <= GetMaxCollectable ( item ) );
        else 
        {
            Core.QLogger.Assert ( count <= GetMaxCollectable ( item ));
            inventoryItems[item] = 0;
        }
        inventoryItems[item] += count;
    }

}
