using System;
using System.Collections.Generic;
using UnityEngine;

public class RecycleManager : Singleton<RecycleManager>
{
	private static Dictionary<Type, Stack<GameObject>> recyclingBins;
	private static Transform transform;
	
	public static void Recycle<T>(GameObject gameObject) where T: MonoBehaviour, IRecyclable
	{
		if (recyclingBins == null)
		{
			recyclingBins = new Dictionary<Type, Stack<GameObject>>();
			transform = Instance.gameObject.transform;
		}

		if (!recyclingBins.ContainsKey(typeof(T)))
		{
			recyclingBins.Add(typeof(T), new Stack<GameObject>());
			recyclingBins[typeof(T)].Push(gameObject);
		}
		
		recyclingBins[typeof(T)].Push(gameObject);

		gameObject.GetComponent<IRecyclable>().OnRecycled();
		Deactivate(gameObject);

	}

	public static bool TryGetItem(Type type, out GameObject gameObject, bool returnEnabled = true)
	{
		gameObject = null;
		
		if (recyclingBins == null)
			return false;
		if (!recyclingBins.ContainsKey(type))
			return false;
		if (recyclingBins[type].Count <= 0)
			return false;

		gameObject = recyclingBins[type].Pop();
		//gameObject.SetActive(returnEnabled);
		Reactivate(gameObject, returnEnabled);
		
		return true;
	}

	public static T GetItem<T>(out GameObject gameObject, bool returnEnabled) where T : MonoBehaviour
	{

		if(TryGetItem(typeof(T), out gameObject, returnEnabled))
			return gameObject.GetComponent<T>();
		
		return null;
	}

	private static void Deactivate(GameObject gameObject)
	{
		if (transform == null)
			transform = Instance.gameObject.transform;
		
		gameObject.transform.parent = transform;
		gameObject.SetActive(false);
	}
	private static void Reactivate(GameObject gameObject, bool state)
	{
		gameObject.transform.parent = null;
		gameObject.SetActive(state);
	}
}

public interface IRecyclable
{
	void Recycle();
	void OnRecycled();
}
