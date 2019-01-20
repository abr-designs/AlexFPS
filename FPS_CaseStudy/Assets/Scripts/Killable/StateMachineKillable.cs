using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachineBase))]
public class StateMachineKillable : KillableArmour
{
	private float waitTime = 4f;
	private bool waiting = false;


	public override void Kill()
	{
		if(waiting)
			return;

		waiting = true;
		
		Debug.LogFormat(gameObject, "{0} is ded", gameObject.name);
		//Destroy(gameObject);

		StartCoroutine(WaitToDespawn(waitTime));
	}

	private IEnumerator WaitToDespawn(float time)
	{
		yield return new WaitForSeconds(time);

		waiting = false;
		RespawnManager.Instance.Respawn(this, startPosition, startRotation, false);

	}
}
