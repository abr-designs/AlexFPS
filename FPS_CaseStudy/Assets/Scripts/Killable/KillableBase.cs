using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class KillableBase : MonoBehaviour
{
	public Action<Vector3> onHitCallback;

	//This is assuming that we have a max health of 100 all the time
	[SerializeField, ProgressBar(0, 100, 1f, 0.1f, 0f)]
	protected float health = 100f;

	public virtual void ChangeHealth(float amount, Vector3 fromPosition)
	{
		health = Mathf.Clamp(health + amount, 0f, 100f);

		if (amount < 0)
			onHitCallback(fromPosition);

		if (health <= 0f)
			Kill();
	}
	public virtual void ChangeHealth(float amount)
	{
		health = Mathf.Clamp(health + amount, 0f, 100f);

		if (health <= 0f)
			Kill();
	}

	public virtual void Kill()
	{
		Debug.LogFormat(gameObject, "{0} is ded", gameObject.name);
		Destroy(gameObject);
	}

	protected virtual void UpdateUI()
	{

	}

}
