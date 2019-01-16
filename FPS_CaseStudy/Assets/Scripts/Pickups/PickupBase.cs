using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupBase : MonoBehaviour
{
    [SerializeField]
    protected string lookingForTag = "Player";
    
    protected new Collider collider;
    
    // Start is called before the first frame update
    protected void Start()
    {
        collider = GetComponent<Collider>();
        
        if(collider.isTrigger == false)
            throw new InvalidOperationException("Collider Must be a trigger on collider");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals(lookingForTag))
            OnPickedUp(other.gameObject);
    }

    public virtual void Init(string tag)
    {
        
    }

    protected virtual void OnPickedUp(GameObject other)
    {
        Debug.LogFormat(gameObject, "{0} picked up {1}", other.name, this.name);
        Destroy(gameObject);
    }
}
