using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Gun", menuName = "Equipment/Gun", order = 1)]
public class ScriptableGun : ScriptableObject
{
   
   [FoldoutGroup("General Information")]
   public float damage;
   [FoldoutGroup("General Information")]
   public float range;
   [Range(1f,10f)][FoldoutGroup("General Information")]
   public float accuracy;
   [FoldoutGroup("General Information")]
   public AnimationCurve damageDropoff = new AnimationCurve();
   [FoldoutGroup("General Information")]
   public bool repeatable;

   [FoldoutGroup("Reload Information")] 
   public int ammoID;
   [FoldoutGroup("Reload Information")] 
   public float fireCooldown;

   [FoldoutGroup("Reload Information")] 
   public float muzzleFlashTime = 0.1f;
   //TODO Need to think if i even want reloading in the game
   //public float reloadTime;
   //public int magazineSize;

   [FoldoutGroup("Visuals")] 
   public GameObject gunPrefab;

   [FoldoutGroup("Visuals")]
   public GameObject muzzleFlashPrefab;
   //public GameObject bulletHolePrefab;

   [FoldoutGroup("Local Transform Information")]
   public Vector3 initialPositionOffset;
   [FoldoutGroup("Local Transform Information")]
   public Vector3 initialRotationOffset;
   [FoldoutGroup("Local Transform Information")]
   public Vector3 localMuzzleOffset;

   
   public void Fire(Vector3 position, Vector3 direction)
   {
     
      RaycastHit hit;
      //TODO Need to incorporate the accuracy here

      
      //TODO Maybe i need to used a boxcast of some kind?
      if (Physics.Raycast(position, direction, out hit, range))
      {
         KillableBase killable = hit.transform.GetComponent<KillableBase>();
         
         //I want to make sure i do negative health (Damage)
         if(killable)
            killable.ChangeHealth(-damage);

         //TODO Need some sort of default material to fallback onto
         var temp = hit.transform.GetComponent<Shootable>();
         if(temp)
            temp.Hit(hit);

         //CreateBulletHole(hit.point, hit.normal);
         Debug.DrawLine(position, hit.point, Color.green, 3f);

      }
      else
        Debug.DrawRay(position, direction * range, Color.red, 3f);

   }


}
