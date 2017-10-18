using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HKUECT;

/// <summary>
/// Attaches this transform to the OSCClient (OnStart)
/// Useful when the OSCClient is in a previous scene, and DontDestroyOnLoad
/// </summary>
public class ParentToMocapCenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;
		transform.parent = FindObjectOfType<OptiTrackOSCClient>().transform;
		transform.localPosition = pos;
		transform.localRotation = rot;
	}
}