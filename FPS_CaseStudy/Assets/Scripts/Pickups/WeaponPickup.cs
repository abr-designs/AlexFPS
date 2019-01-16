using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : PickupBase
{
	[SerializeField]
	private ScriptableGun weapon;

	public void Init(string tag, ScriptableGun gun)
	{
		base.Init(tag);
		weapon = gun;
	}

	protected override void OnPickedUp(GameObject other)
	{
		Equipment temp = other.GetComponent<Equipment>();

		//If the person picking this up can't even use armour, don't try and give them any
		if (temp == null)
			return;
		
		//Debug.Log("Adding weapon to arsenal");
		
		temp.AddWeapon(weapon);
		
		base.OnPickedUp(other);
		
	}
}
