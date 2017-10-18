using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBox : MonoBehaviour {

	public GameObject open, closed;
	public Object duplicationPrefab;
	bool isOpen = true;

	void OnTriggerEnter( Collider other ) {
		if (isOpen) {
			StopAllCoroutines();
			StartCoroutine(CloseBox(other.transform));
		}
	}

	void OnTriggerExit( Collider other ) {
		StopAllCoroutines();
		open.SetActive(true);
		closed.SetActive(false);
		isOpen = true;
	}

	IEnumerator CloseBox( Transform target ) {
		yield return new WaitForSeconds(1f);
		open.SetActive(false);
		closed.SetActive(true);
		isOpen = false;

		//spawn the same object at the target position\
		Instantiate(duplicationPrefab, target.position, target.rotation);
	}
}
