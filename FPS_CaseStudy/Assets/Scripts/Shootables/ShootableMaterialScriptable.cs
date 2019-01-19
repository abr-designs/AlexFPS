using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Material", menuName = "Shootable/Material", order = 1)]
public class ShootableMaterialScriptable : ScriptableObject
{
	public bool createBulletHole = true;

	public GameObject bulletHolePrefab;
	public GameObject bulletStrikeParticlePrefab;

	[Range(0f,1f)]
	public float volume = 1f;
	public AudioClip[] impactAudioClips;

	public void PlayImpactSound(AudioSource audioSource)
	{
		if (audioSource == null || impactAudioClips.Length == 0)
			return;
		
		audioSource.PlayOneShot(impactAudioClips[Random.Range(0,impactAudioClips.Length)], volume);
	}
}
