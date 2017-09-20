using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HKUECT.HoloLens;
using UnityEngine.SceneManagement;

public class ScanForNextScene : MonoBehaviour {

	public string sceneName = "";

	// Use this for initialization
	IEnumerator Start () {
		//give it a frame to settle
		yield return null;

		WorldErrors.Print("Running spacial mapper");
		SpatialWrapper.RunSpatialMapping(false, 30f);
		while (!SpatialWrapper.SpatialInfoReady) {
			
			yield return null;
		}

		WorldErrors.Print("Loading scene");

		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(sceneName);

		yield return null;
	}
}
