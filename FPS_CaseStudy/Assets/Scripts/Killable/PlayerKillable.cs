using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillable : KillableArmour, IUIUpdate, IRespawnable
{
	
	
	
	protected override void Start()
	{
		base.Start();
		
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

	public override void Kill()
	{
		RespawnManager.Instance.Respawn(this, startPosition, startRotation, true);
	}

	public override void Reset()
	{
		base.Reset();
		UpdateUI();
	}

	public void OnDespawn()
	{
		
	}

	public void OnRespawn()
	{
	}
}
