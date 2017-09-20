using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldErrors : MonoBehaviour 
{
	static WorldErrors instance = null;

	public Object textObject;
	public int listSize = 10;
	public bool isEnabled = true;

	TextMesh[] textMeshes;
	//int current = 0;

	List<string> errors = new List<string>();

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		textMeshes = new TextMesh[listSize];
		for (int i = 0; i < listSize; ++i) {
			GameObject g = Instantiate (textObject) as GameObject;
			g.transform.position = transform.position + Vector3.up * i * .5f;
			textMeshes [i] = g.GetComponent<TextMesh> ();
		}
	}

	void Update() {
		while (errors.Count > 0) {
			for (int i = listSize - 1; i > 0; --i) {
				textMeshes [i].text = textMeshes [i - 1].text;
			}
			textMeshes [0].text = errors[0];
			errors.RemoveAt (0);
		}
	}

	public static void Print(string msg) {
		if (instance != null && instance.isEnabled)
			instance.PrintError (msg);
		Debug.Log (msg);
	}

	void PrintError(string msg) {
		errors.Add (msg);
	}
}