using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToTarget : MonoBehaviour {
	public Transform target;
	public float minDistance = .65f;
	public float maxDistance = 1f;
	
	void Start () {
		if (target == null) enabled = false;	
	}
	
	void LateUpdate () {
		float d = Vector3.Distance(transform.position, target.position);
		if ( d < minDistance || d > maxDistance ) {
			transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
		}
		Vector3 offset = transform.position - Camera.main.transform.position;
		transform.LookAt(transform.position + offset, Vector3.up);
	}
}
