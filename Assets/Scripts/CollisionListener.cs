using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : iInteractable
{

    public override bool IsThisPlayer { get { return isThisPlayer; }}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetName ()
    {
        return "CollisionListener from gameobject :" + gameObject.name;
    }

    [SerializeField] bool isThisPlayer = false;
}
