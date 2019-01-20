using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RespawnManager : Singleton<RespawnManager>
{
	[SerializeField, SuffixLabel("sec")]
	private float respawnTime = 5f;

	public void Respawn(KillableBase killable, Vector3 position, Quaternion rotation, bool isPlayerRespawn)
	{
		StartCoroutine(RespawnPlayerCoroutine(killable, position, rotation, respawnTime, isPlayerRespawn));
			
	}

	private IEnumerator RespawnPlayerCoroutine(KillableBase killable, Vector3 position, Quaternion rotation, float time, bool player)
	{
		if(killable.gameObject.activeInHierarchy == false)
			yield break;

		

		var o = killable.gameObject;
		var r = o.GetComponent<IRespawnable>();
		
		if ( r == null)
			yield break;
		
		r.OnDespawn();
		o.SetActive(false);

		if (player)
		{
			float _t = 0f;

			while (_t < time)
			{
				UIManager.Instance.ShowRespawn(true, "Time till respawn " + (int) (time - _t));
				_t += Time.deltaTime;

				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(time);
		}


		o.transform.position = position;
		o.transform.rotation = rotation;

		UIManager.Instance.ShowRespawn(false, string.Empty);
		killable.Reset();
		o.SetActive(true);
		r.OnRespawn();
	}
}
