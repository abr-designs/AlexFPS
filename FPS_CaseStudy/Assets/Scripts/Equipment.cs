using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Equipment : MonoBehaviour, IUIUpdate
{
	[SerializeField, Required]
	private Transform playerCameraTransform;
	
	[Header("Equipment")]
	public ScriptableGun currentlyEquipped;
	private Transform gunTransform;
	private Transform muzzleParticleTransform;

	[SerializeField, ReadOnly]
	private Dictionary<int, int> ammo;

	private Vector3 MuzzlePosition
	{
		get { return gunTransform.TransformPoint(currentlyEquipped.localMuzzleOffset); }
	}

	private ParticleSystem[] muzzleParticles;
	
	private float lastFireTime = 0f;
	private bool CanFire
	{
		get
		{
			//Debug.LogFormat("{0} - {1} >= {2} is {3}", Time.time, lastFireTime, currentlyEquipped.fireCooldown, (Time.time - lastFireTime >= currentlyEquipped.fireCooldown));
			return (Time.time - lastFireTime >= currentlyEquipped.fireCooldown);
		}
	}
	
	//TODO Need to actually add in the other options
	private void Start()
	{
		ammo = new Dictionary<int, int>();
		if (currentlyEquipped)
		{
			SpawnGun();
			ammo.Add(currentlyEquipped.ammoID, 50);
		}

		//We can just apply the audio source onto the player
		UpdateUI();
	}

	public void AddWeapon(ScriptableGun weapon)
	{
		if (currentlyEquipped == weapon)
		{
			AddAmmo(25);
		}
		else
		{
			currentlyEquipped = weapon;

			if (ammo.ContainsKey(currentlyEquipped.ammoID) == false)
				ammo.Add(currentlyEquipped.ammoID, 50);
			else
				AddAmmo(25);
			
			SpawnGun();
		}
		//TODO Check to see if its already equipped
			//If it is, then we just need to add ammo
		
		//TODO If not already equipped, spawn new gun
		
		UpdateUI();
	}

	public void AddAmmo(int amount)
	{
		ammo[currentlyEquipped.ammoID] += amount;
		UpdateUI();
	}

	private void SpawnGun()
	{
		//IF there's already a gun equipped, we'll destroy it.
		//FIXME What i think that should happen is i only disable the object, then that way i only spawn as i need
		if(gunTransform)
			Destroy(gunTransform.gameObject);
		
		gunTransform = Instantiate(currentlyEquipped.gunPrefab).transform;
		gunTransform.parent = playerCameraTransform;
		gunTransform.localPosition = currentlyEquipped.initialPositionOffset;
		gunTransform.localRotation= Quaternion.Euler(currentlyEquipped.initialRotationOffset);
		
		if(!muzzleParticleTransform)
			//Creates the instance of the muzzle flash, attaching it to the player as a child object
			muzzleParticleTransform = Instantiate(currentlyEquipped.muzzleFlashPrefab).transform;
		
		//this will reposition the muzzle flash wherever the weapon requires, even if the object already exists
		muzzleParticleTransform.parent = playerCameraTransform;
		muzzleParticleTransform.position = MuzzlePosition;
		muzzleParticleTransform.forward = playerCameraTransform.forward;

		muzzleParticles = muzzleParticleTransform.GetComponentsInChildren<ParticleSystem>();
		SetParticles(false);
		//Need to store them in some sort of array/list
		//Need a coroutine to actually only play the animation for X amount of time
	}

	public bool Fire(Vector3 position, Vector3 direction, AudioSource audioSource)
	{
		//We need to check for all of the legality of shooting before we try to actually fire
		if (currentlyEquipped)
		{
			if (!CanFire)
			{
				//Debug.LogError("Cant Fire");
				return false;
			}

			if (HasAmmo() == false)
			{
				Debug.LogError("No Ammo");
				return false;
			}
            
			currentlyEquipped.Fire(position, direction.normalized, audioSource);
			SpendAmmo();
			lastFireTime = Time.time;
			UpdateUI();

			//Trying to prevent the double muzzle flash
			if(!flashing)
				StartCoroutine(MuzzleFlashCoroutine(currentlyEquipped.muzzleFlashTime));
		}

		return true;
	}

	private void SpendAmmo()
	{
		if (ammo.ContainsKey(currentlyEquipped.ammoID))
		{
			ammo[currentlyEquipped.ammoID]--;
		}
	}

	private bool HasAmmo()
	{
		if (ammo.ContainsKey(currentlyEquipped.ammoID))
		{
			return (ammo[currentlyEquipped.ammoID] > 0);
		}

		return false;
	}

	public void UpdateUI()
	{
		UIManager.Instance.SetAmmo(ammo[currentlyEquipped.ammoID]);
	}

	private bool flashing = false;
	private IEnumerator MuzzleFlashCoroutine(float time)
	{
		flashing = true;
		SetParticles(true);

		yield return new WaitForSeconds(time);

		SetParticles(false);
		flashing = false;
	}

	private void SetParticles(bool play)
	{
		for (int i = 0; i < muzzleParticles.Length; i++)
		{
			if(play)
				muzzleParticles[i].Play();
			else
			{
				muzzleParticles[i].Stop();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (gunTransform == false)
			return;
		
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(gunTransform.TransformPoint(currentlyEquipped.localMuzzleOffset), 0.1f);
	}

}
