using UnityEngine;
using System.Collections;

public class ActionRPGCamera : MonoBehaviour
{
	
	public Transform target;
	public float distance = 10.0f;
	public float height = 5.0f;
	public float heightDamping = 2.0f;
	
	// Use this for initialization
	void Start ()
	{
		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		}
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (!target)
			return;
		
		var wantedHeight = target.position.y + height;
			
		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;
	
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
				
		// Always look at the target
		transform.LookAt (target);
	}
}