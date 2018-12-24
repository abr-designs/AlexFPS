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

	[SerializeField, ReadOnly]
	private Dictionary<int, int> ammo;

	private Vector3 MuzzlePosition
	{
		get { return gunTransform.TransformPoint(currentlyEquipped.localMuzzleOffset); }
	}
	
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
		
		UpdateUI();
	}

	private void SpawnGun()
	{
		gunTransform = Instantiate(currentlyEquipped.gunPrefab).transform;
		gunTransform.parent = playerCameraTransform;
		gunTransform.localPosition = currentlyEquipped.initialPositionOffset;
		gunTransform.localRotation= Quaternion.Euler(currentlyEquipped.initialRotationOffset);
	}

	public void Fire(Vector3 direction)
	{
		//We need to check for all of the legality of shooting before we try to actually fire
		if (currentlyEquipped)
		{
			if (!CanFire)
			{
				Debug.LogError("Cant Fire");
				return;
			}

			if (HasAmmo() == false)
			{
				Debug.LogError("No Ammo");
				return;
			}
            
			currentlyEquipped.Fire(MuzzlePosition, direction.normalized);
			SpendAmmo();
			lastFireTime = Time.time;
			UpdateUI();
		}
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

	private void OnDrawGizmos()
	{
		if (gunTransform == false)
			return;
		
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(gunTransform.TransformPoint(currentlyEquipped.localMuzzleOffset), 0.1f);
	}

}
