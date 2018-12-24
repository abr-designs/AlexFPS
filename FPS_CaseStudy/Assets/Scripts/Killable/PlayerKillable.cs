using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillable : KillableArmour, IUIUpdate
{
	private void Start()
	{
		UpdateUI();
	}

	public override void ChangeHealth(float amount)
	{
		base.ChangeHealth(amount);
		UpdateUI();
	}

	public override void ChangeArmour(float amount)
	{
		base.ChangeArmour(amount);
		UpdateUI();
	}

	public void UpdateUI()
	{
		UIManager.Instance.SetArmour(armour);
		UIManager.Instance.SetHealth(health);
	}
}
