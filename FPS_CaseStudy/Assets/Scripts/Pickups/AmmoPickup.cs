using UnityEngine;

public class AmmoPickup : PickupBase
{
	[SerializeField]
	private int ammoAmount = 25;

	protected override void OnPickedUp(GameObject other)
    {
        Equipment temp = other.GetComponent<Equipment>();

        //If the person picking this up can't even use armour, don't try and give them any
        if (temp == null)
            return;
		
        //Debug.Log("Adding weapon to arsenal");
		
        temp.AddAmmo(ammoAmount);
		
        base.OnPickedUp(other);
		
    }
}
