using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkSoundSet", menuName = "Audio/Walking Sound Set", order = 1)]
public class WalkSoundScriptable : ScriptableObject
{
	[SerializeField, Range(0f,1f)]
	private float volume = 1f;
	
	[SerializeField]
	private AudioClip[] audioClips;

	

	public void PlayWalkingAudio(AudioSource audioSource)
	{

		if (audioClips.Length == 0)
		{
			Debug.LogError("No walking sounds set for "  + name);
			return;
		}
		audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], volume);
	}
}
