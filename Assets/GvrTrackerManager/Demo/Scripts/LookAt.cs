// =================================
//
//	LookAt.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	#region Inspector Settings
	[SerializeField] private Transform lookTarget;
	[Header("Follow Angles")]
	[SerializeField] private bool x = true;
    [SerializeField] private bool y = true;
    [SerializeField] private bool z = true;
	#endregion // Inspector Settings


	#region Member Field

	#endregion // Member Field

	
	#region MonoBehaviour Methods

	private void Awake ()
	{
		
	}

	private void Start ()
	{
		
	}
	
	private void Update ()
	{
		Vector3 r = transform.eulerAngles;
		transform.LookAt(lookTarget);

		if(x)
		{
			r = new Vector3(transform.eulerAngles.x, r.y, r.z);
		}
		if(y)
        {
            r = new Vector3(r.x, transform.eulerAngles.y, r.z);
		}
		if(z)
        {
            r = new Vector3(r.x, r.y, transform.eulerAngles.z);
		}
		transform.rotation = Quaternion.Euler(r);
	}

	#endregion // MonoBehaviour Methods


	#region Member Methods

	#endregion // Member Methods
}