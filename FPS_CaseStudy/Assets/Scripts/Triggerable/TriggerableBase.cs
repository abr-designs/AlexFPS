using System;
using Sirenix.OdinInspector;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
public abstract class TriggerableBase : MonoBehaviour
{
    
    [SerializeField,Required, BoxGroup("Basic Trigger Information")]
    protected new Collider collider;
    protected new Transform transform;
    
    [SerializeField, BoxGroup("Basic Trigger Information")]
    protected string lookingForTag = "Player";
    
    private void Awake()
    {
        transform = gameObject.transform;
        
        //collider = GetComponent<Collider>();
        
        if(collider.isTrigger == false)
            throw new InvalidOperationException("Collider must be Trigger " + gameObject.name);
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals(lookingForTag))
            Triggered(other.gameObject);
            
    }

    protected abstract void Start();
    protected abstract void Triggered(GameObject other);
}
