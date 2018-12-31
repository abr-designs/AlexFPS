using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class TimedRecycle : MonoBehaviour, IRecyclable
{
    [SerializeField]
    private float lifeTime = 10f;
    private float timeEnabled = 999;

    private bool active = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        timeEnabled = Time.time;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active == false)
            return;
        
        if(Time.time - timeEnabled >= lifeTime)
            Recycle();
    }

    public void Recycle()
    {
        active = false;
        RecycleManager.Recycle<TimedRecycle>(gameObject);
    }

    public void OnRecycled()
    {
        Debug.Log("Recycled", gameObject);
    }
}
