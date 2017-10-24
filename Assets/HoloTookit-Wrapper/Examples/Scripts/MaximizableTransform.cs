using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HKUECT.HoloLens;


/// <summary>
/// An example illustrating how to use the GlobalGestures to create minimize/maximize functionality in the hierarchy
/// The biggest challenge is how to register taps on deeply nested colliders in the correct place (here)
/// </summary>
public class MaximizableTransform : MonoBehaviour {

	public GameObject max;
	public GameObject min;
	public bool startMinimized = false;

	bool minimized = false;

	Collider[] nestedColliders;

	void Awake() {
		min.SetActive(true);
		max.SetActive(true);

		List<Collider> cols = new List<Collider>();
		cols.AddRange(min.GetComponentsInChildren<Collider>());
		cols.AddRange(max.GetComponentsInChildren<Collider>());

		nestedColliders = cols.ToArray();
	}
	
	void Start () {
		max.SetActive (!startMinimized);
		min.SetActive (startMinimized);

		minimized = startMinimized;

		GlobalGestures.recognizerInstance.TappedEvent += Tapped;
	}

	void OnDestroy() {
		GlobalGestures.recognizerInstance.TappedEvent -= Tapped;
	}
	
	void Tapped(UnityEngine.XR.WSA.Input.InteractionSourceKind source, int tapCount, Ray headRay) {
		RaycastHit hitInfo;
		if ( Physics.Raycast(headRay, out hitInfo )) {
			bool found = false;
			for (int i = 0; i < nestedColliders.Length; ++i) {
				if (hitInfo.collider == nestedColliders[i]) {
					found = true;
					break;
				}
			}
			if (!found) return;

			minimized = !minimized;

			max.SetActive(!minimized);
			min.SetActive(minimized);
		}
	}
}
