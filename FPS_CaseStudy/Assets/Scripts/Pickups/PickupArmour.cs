using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupArmour : PickupBase
{
	[SerializeField]
	private float armourAmount = 10f;

	protected override void OnPickedUp(GameObject other)
	{
		KillableArmour temp = other.GetComponent<KillableArmour>();

		//If the person picking this up can't even use armour, don't try and give them any
		if (temp == null)
			return;
		
		temp.ChangeArmour(armourAmount);
		
		base.OnPickedUp(other);
		
	}
}
