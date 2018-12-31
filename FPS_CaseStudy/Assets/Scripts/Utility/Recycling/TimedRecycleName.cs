using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedRecycleName : TimedRecycle
{
	[SerializeField]
	private string name;

	public override void Recycle()
	{
		active = false;
		RecycleManager.Recycle(name, gameObject);
	}
}
