using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToTarget : MonoBehaviour {
	public Transform target;
	
	void Start () {
		if (target == null) enabled = false;	
	}
	
	void LateUpdate () {
		transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
		Vector3 offset = transform.position - Camera.main.transform.position;
		transform.LookAt(transform.position + offset, Vector3.up);
	}
}
