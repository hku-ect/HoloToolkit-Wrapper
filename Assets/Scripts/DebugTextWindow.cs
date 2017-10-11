using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HKUECT.HoloLens;

public class DebugTextWindow : MonoBehaviour {

	[Tooltip("This must be a scene object")]
	public GameObject scenePrefab;
	public OptitrackCalibration calibrationScript;

	Vector3 center, tr, bl, osctr, oscbl;
	float scale, yOffset, angle;

	Dictionary<string, Text> textObjects = new Dictionary<string, Text>();
	
	// Update is called once per frame
	void LateUpdate () {
		if ( calibrationScript.MockCalibration( ref center, ref angle, ref scale, ref tr, ref bl, ref osctr, ref oscbl, ref yOffset ) ) {
			SetText("center", center.ToString());
			SetText("angle", angle.ToString());
			SetText("scale", scale.ToString());
			SetText("XZ", tr.ToString());
			SetText("-X-Z", bl.ToString());
			SetText("OSC XZ", osctr.ToString());
			SetText("OSC -X-Z", oscbl.ToString());
			SetText("yOffset", yOffset.ToString());
		}
	}

	void SetText( string key, string value) {
		if (!textObjects.ContainsKey(key)) {
			GameObject newObject = Instantiate(scenePrefab);
			newObject.SetActive(true);
			newObject.transform.parent = scenePrefab.transform.parent;
			newObject.transform.localScale = Vector3.one;
			newObject.transform.localRotation = Quaternion.identity;
			newObject.transform.localPosition = Vector3.zero;
			textObjects.Add(key, newObject.GetComponent<Text>());
		}

		textObjects[key].text = key + ": " + value;
	}
}
