using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class View : MonoBehaviour
{
	private static Transform[] mGlobalTargets;

	public Transform originTransform;

	public float MaxViewDistance;
	
	[Space(10f)]
	public Transform[] targets;

	[Header("Views"), SerializeField]
	ViewProfile[] viewProfiles;


	new Transform transform;

	// Use this for initialization
	void Start ()
	{
		if(originTransform == null)
        {
            transform = gameObject.transform;
        }
        else
        {
            transform = originTransform;
        }

		//Checks to see if static Targets list is set, if not assign current targets
		if (mGlobalTargets == null)
			mGlobalTargets = targets;
		//If our current targets list is null or empty assign to our default static list
		else if(targets == null || targets.Length == 0)
			targets = mGlobalTargets;

		MaxViewDistance = viewProfiles.Max(x => x.viewDistance);

	}

	/// <summary>
	/// Returns a bool indicating if this View can see any of the listed targets. If returned true, a list of which targets
	/// that can be seen will out.
	/// </summary>
	/// <param name="activeTargets"></param>
	/// <returns></returns>
	public bool CanSeeTargets(out List<Transform> activeTargets)
	{
		activeTargets = null;

		//For each target
		for(int i = 0; i < targets.Length; i++)
		{
			//Loop through our View Profiles
			for(int j = 0; j < viewProfiles.Length; j++)
			{
				//Check first to see if the target is close enough to even be seen
				if (Vector3.Distance(targets[i].position, transform.position) <= viewProfiles[j].viewDistance)
				{
					float dotValue = 1f - ((float)viewProfiles[j].viewAngle / 90f);
					Vector3 dir = (targets[i].position - transform.position).normalized;

					//Then check to see if the target is within the profile field of view
					if (Vector3.Dot(transform.forward.normalized, dir) >= dotValue)
					{
						//Finally check to see if there's an obstacle in the way (Wall, etc.)
						if(IsViewBlocked(transform.position, dir, viewProfiles[j].viewDistance) == false)
                        {
                            if (activeTargets == null)
                            {
                                activeTargets = new List<Transform>();
                            }

							//Add our confirmed target to the list
                            activeTargets.Add(targets[i]);
                        }
					}
				}
			}
		}

        return activeTargets != null;
	}

	private static bool IsViewBlocked(Vector3 startPostion, Vector3 direction, float distance)
	{
		RaycastHit hit;
		if (Physics.Raycast(new Ray(startPostion, direction), out hit, distance))
		{
			if (hit.transform.tag == "Player")
			{
				//Debug.Log("I SEE PLAYER!!!");
				Debug.DrawLine(startPostion, hit.point, Color.green, 2f);
				return false;
			}
			else
			{
				Debug.Log("I dont see anything." + hit.transform.tag + " " + hit.transform.name, hit.transform.gameObject);
				Debug.DrawLine(startPostion, hit.point, Color.yellow, 2f);

				return true;

			}

		}

		return false;
	}


#if UNITY_EDITOR

	[SerializeField, FoldoutGroup("Editor Properties")]
	private Color ringColor = Color.white;

	[SerializeField, FoldoutGroup("Editor Properties")]
	private Color viewAngleColor = Color.green;
	void OnDrawGizmos()
	{
		if (viewProfiles == null || viewProfiles.Length <= 0)
			return;

		if (transform == null)
			transform = gameObject.transform;

		for (int i = 0; i < viewProfiles.Length; i++)
		{
			UnityEditor.Handles.color = ringColor;
			UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, viewProfiles[i].viewDistance);

			//Draws the View Angle/Range
			Gizmos.color = viewAngleColor;
			Vector3 startPosition = transform.position;
			Vector3 leftPostion;
			Vector3 rightPosition;

			float topDist = Mathf.Tan(Mathf.Deg2Rad * (viewProfiles[i].viewAngle)) * viewProfiles[i].viewDistance;

			Vector3 leftDir = ((transform.forward.normalized * viewProfiles[i].viewDistance) + (-transform.right.normalized * topDist)).normalized * viewProfiles[i].viewDistance;
			Vector3 rightDir = ((transform.forward.normalized * viewProfiles[i].viewDistance) + (transform.right.normalized * topDist)).normalized * viewProfiles[i].viewDistance;

			leftPostion = startPosition + leftDir;
			rightPosition = startPosition + rightDir;

			Gizmos.DrawLine(startPosition, leftPostion);
			Gizmos.DrawLine(rightPosition, startPosition);
		}
	}
#endif

	[System.Serializable]
	public class ViewProfile
	{
		public string name;

		public float viewDistance;
		[Range(0, 90)]
		public float viewAngle;
	}
}
