using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Equipment/Gun", order = 1)]
public class ScriptableGun : ScriptableObject
{
   
   [Header("General Information")]
   public float damage;
   public float range;
   [Range(1f,10f)]
   public float accuracy;
   public AnimationCurve damageDropoff = new AnimationCurve();

   [Header("Reload Information")] 
   public int ammoID;
   public float fireCooldown;

   public float muzzleFlashTime = 0.1f;
   //TODO Need to think if i even want reloading in the game
   //public float reloadTime;
   //public int magazineSize;

   [Header("Visuals")] 
   public GameObject gunPrefab;

   public GameObject muzzleFlashPrefab;
   public GameObject bulletHolePrefab;

   [Header("Local Transform Information")]
   public Vector3 initialPositionOffset;
   public Vector3 initialRotationOffset;
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

         CreateBulletHole(hit.point, hit.normal);
         Debug.DrawLine(position, hit.point, Color.green, 3f);

      }
      else
        Debug.DrawRay(position, direction * range, Color.red, 3f);

   }

   private void CreateBulletHole(Vector3 position, Vector3 direction)
   {
      var temp = Instantiate(bulletHolePrefab).transform;
      temp.position = position + (direction.normalized * 0.05f);
      temp.localRotation = Quaternion.AngleAxis(Random.Range(0, 360f), direction);
      temp.forward = -direction;
   }

}
