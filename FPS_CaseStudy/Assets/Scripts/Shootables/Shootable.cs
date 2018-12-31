using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shootable : MonoBehaviour
{
   [SerializeField, Required]
   private ShootableMaterialScriptable shootMaterial;

   private static readonly float offset = 0.05f;

   public void Hit(RaycastHit info)
   {
      if (!shootMaterial)
      {
         Debug.LogError("Trying to use shoot material, but none assigned", gameObject);
         return;
      }
      
      if(shootMaterial.createBulletHole)
         CreateAt(shootMaterial.bulletHolePrefab, info.point, info.normal, true);
      
      CreateAt(shootMaterial.bulletStrikeParticlePrefab, info.point, info.normal);
   }

   private static void CreateAt(GameObject prefab, Vector3 position, Vector3 normal, bool flipFacing = false)
   {
      var temp = Instantiate(prefab).transform;
      temp.position = position + (normal.normalized * offset);
      temp.forward = flipFacing ? -normal : normal;
   }
}
