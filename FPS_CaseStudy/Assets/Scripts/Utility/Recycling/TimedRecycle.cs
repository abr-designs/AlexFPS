
using UnityEngine;

public class TimedRecycle : MonoBehaviour, IRecyclable
{
    [SerializeField]
    protected float lifeTime = 10f;
    protected float timeEnabled = 999;

    protected bool active = false;
    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        timeEnabled = Time.time;
        active = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (active == false)
            return;
        
        if(Time.time - timeEnabled >= lifeTime)
            Recycle();
    }

    public virtual void Recycle()
    {
        active = false;
        RecycleManager.Recycle<TimedRecycle>(gameObject);
    }

    public virtual void OnRecycled()
    {
        Debug.Log("Recycled " + gameObject.name);
    }
}
