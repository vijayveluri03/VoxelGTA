using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HoverAndRotate : MonoBehaviour
{
    public float rotationSpeedPerSecInDegrees = 180;
    public float hoverSpeedPerSec = 180;
    public float minHoverDistanceAboveGround = 2;
    public float hoverRange = 2;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        currentHoverSpeed += hoverSpeedPerSec * Time.deltaTime;
        hoverDelta = Utils.Utils.ConvertToNewRange( -1, 1, 0, 1, Mathf.Sin ( Mathf.Deg2Rad * currentHoverSpeed ) );

        transform.localPosition = originalPosition + Vector3.up * (minHoverDistanceAboveGround + hoverDelta * hoverRange);
        transform.Rotate ( Vector3.up, rotationSpeedPerSecInDegrees * Time.deltaTime );
    }

    private float currentRotation;
    private float currentHoverSpeed;
    private float hoverDelta;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
}
