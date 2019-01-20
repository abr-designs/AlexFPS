using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GunPickupSpawner : PickupSpawner
{
	[SerializeField, Required, PropertyOrder(-1)]
	private ScriptableGun gunPickup;

	protected override void Start()
	{
		gameObject.name += "[" + gunPickup.gunPrefab.name + "]";
        
		transform = gameObject.transform;

		SpawnPickup();
	}
	protected override void SpawnPickup()
	{
		pickupTransform = Instantiate(gunPickup.gunPrefab).transform;

		pickupTransform.GetComponent<Collider>().enabled   = true;
		pickupTransform.GetComponent<Collider>().isTrigger = true;
		var pickup = pickupTransform.gameObject.AddComponent<WeaponPickup>();
		pickup.Init("Player", gunPickup);

		pickupTransform.position = transform.TransformPoint(spawnOffset);
		pickupTransform.rotation = transform.rotation * Quaternion.Euler(spawnRotationOffset);
	}
}

