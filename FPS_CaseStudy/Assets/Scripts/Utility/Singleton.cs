using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	public static T Instance
	{
		get { return _instance; }
	}

	private static T _instance;
	
	private void Awake()
	{
		if (_instance != null)
		{
			Debug.LogError("Trying to create multiple instances of " + typeof(T).Name);
			enabled = false;
			return;
		}

		_instance = (T)this;
	}
}
