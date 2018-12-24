using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickupBase
{
	[SerializeField]
	private float healthAmount = 10f;

	protected override void OnPickedUp(GameObject other)
	{
		KillableBase temp = other.GetComponent<KillableBase>();

		//If the person picking this up can't even use armour, don't try and give them any
		if (temp == null)
			return;
		
		temp.ChangeHealth(healthAmount);
		
		base.OnPickedUp(other);
		
	}
}
