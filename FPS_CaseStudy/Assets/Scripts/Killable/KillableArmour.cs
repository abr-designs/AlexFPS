using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KillableArmour : KillableBase
{
	[SerializeField, ProgressBar(0, 100, 0.5f, 0.5f, 0f)]
	protected float armour = 0;

	public virtual void ChangeArmour(float amount)
	{
		if (amount < 0f)
		{
			Debug.LogError("If you want to minus armor, use change health");
			return;
		}

		armour = Mathf.Clamp(armour + amount, 0f, 100f);
	}

	public override void ChangeHealth(float amount)
	{
		if (amount < 0)
		{
			if (armour > 0)
			{
				armour -= Mathf.Abs(amount);

				if (armour < 0)
				{
					amount = armour;
					armour = 0f;
				}
				else
					return;
			}
		}

		base.ChangeHealth(amount);
	}

	protected override void UpdateUI()
	{
		base.UpdateUI();
		//TODO Update the Armour UI
	}
}
