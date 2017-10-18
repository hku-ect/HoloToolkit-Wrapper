using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionText : MonoBehaviour {
	public Transform target;
	public TextMesh text;

	// Update is called once per frame
	void Update () {
		if ( target && text ) {
			text.text = target.position.ToString();
		}	
	}
}
