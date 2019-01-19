using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrail : TimedRecycleName
{
    private new LineRenderer renderer;
    [SerializeField]
    private float startLineSize = 0.01f;
    [SerializeField]
    private float endLineSize = 0f;
    
    public void Init(Vector3 position, Vector3 direction)
    {
        active = true;
        
        if(!renderer)
            renderer = GetComponent<LineRenderer>();
        
        renderer.SetPosition(0, position);
        renderer.SetPosition(1, position + (direction.normalized * 100f));
    }

    protected override void Update()
    {
        base.Update();

        if (!active)
            return;

        renderer.widthMultiplier = Mathf.Lerp(startLineSize, endLineSize, (Time.time - timeEnabled) / lifeTime);

    }
}
