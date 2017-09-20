using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HandDraggable))]
public class DraggableRigidbody : MonoBehaviour {

	HandDraggable hd;
	Rigidbody rBody;
	Vector3? lastPosition;
	Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		hd = GetComponent<HandDraggable> ();
		rBody = GetComponent<Rigidbody> ();

		hd.StoppedDragging += StopDrag;
		hd.StartedDragging += StartDrag;
	}

	void StartDrag() {
		rBody.isKinematic = true;
		rBody.useGravity = false;
	}

	void StopDrag() {
		rBody.isKinematic = false;
		rBody.useGravity = true;
		//apply stored velocity
		rBody.AddForce( velocity = velocity / Time.fixedDeltaTime, ForceMode.VelocityChange);
	}

	void FixedUpdate() {
		//keep an updated velocity
		//reset when not dragging
		if ( rBody.isKinematic ) {
			if ( lastPosition == null ) {
				lastPosition = rBody.position;
			}
			velocity = rBody.position - lastPosition.Value;
			lastPosition = rBody.position;
		}
		else if ( lastPosition != null ) {
			lastPosition = null;
		}
	}
}
