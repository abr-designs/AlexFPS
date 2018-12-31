using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField, Range(1f,30f)]
    private float respawnTime;

    private float despawnTimeCode;

    [SerializeField]
    private Vector3 spawnOffset;
    [SerializeField]
    private Vector3 spawnRotationOffset;

    [SerializeField,Required]
    private GameObject pickupPrefab;

    private Transform pickupTransform;

    
    private new Transform transform;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        gameObject.name += "[" + pickupPrefab.name + "]";
        
        transform = gameObject.transform;

        SpawnPickup();
    }


    private void SpawnPickup()
    {
        pickupTransform = Instantiate(pickupPrefab).transform;

        pickupTransform.position = transform.TransformPoint(spawnOffset);
        pickupTransform.rotation = transform.rotation * Quaternion.Euler(spawnRotationOffset);
    }

    private void LateUpdate()
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

    private void OnDrawGizmos()
    {
        if (transform == null)
            transform = gameObject.transform;
        
        Gizmos.color = Color.red;
        //Gizmos.matrix = transform.worldToLocalMatrix;
        Gizmos.DrawSphere(transform.TransformPoint(spawnOffset), 0.1f);
        
    }

    #endif
}
