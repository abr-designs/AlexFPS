using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField, Range(1f,30f)]
    protected float respawnTime;

    private float despawnTimeCode;

    [SerializeField]
    protected Vector3 spawnOffset;
    [SerializeField]
    protected Vector3 spawnRotationOffset;

    [SerializeField,Required]
    protected GameObject pickupPrefab;

    protected Transform pickupTransform;

    
    protected new Transform transform;
    
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameObject.name += "[" + pickupPrefab.name + "]";
        
        transform = gameObject.transform;

        SpawnPickup();
    }


    protected virtual void SpawnPickup()
    {
        pickupTransform = Instantiate(pickupPrefab).transform;

        pickupTransform.GetComponent<Collider>().enabled = true;
        pickupTransform.GetComponent<Collider>().isTrigger = true;

        pickupTransform.position = transform.TransformPoint(spawnOffset);
        pickupTransform.rotation = transform.rotation * Quaternion.Euler(spawnRotationOffset);
    }

    protected void LateUpdate()
    {
        if (!transform)
            return;
        
        if (pickupTransform)
            return;

        if (despawnTimeCode == 0f)
            despawnTimeCode = Time.time;

        if (Time.time - despawnTimeCode >= respawnTime)
        {
            despawnTimeCode = 0f;
            SpawnPickup();
        }
    }
    
    #if UNITY_EDITOR

    protected virtual void OnDrawGizmos()
    {
        if (transform == null)
            transform = gameObject.transform;
        
        Gizmos.color = Color.red;
        //Gizmos.matrix = transform.worldToLocalMatrix;
        Gizmos.DrawSphere(transform.TransformPoint(spawnOffset), 0.1f);
        
    }

    #endif
}
