using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Material", menuName = "Shootable/Material", order = 1)]
public class ShootableMaterialScriptable : ScriptableObject
{
	public bool createBulletHole = true;

	public GameObject bulletHolePrefab;
	public GameObject bulletStrikeParticlePrefab;
}
