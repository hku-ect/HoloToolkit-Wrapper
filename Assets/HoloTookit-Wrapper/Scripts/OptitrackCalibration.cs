using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;

namespace HKUECT.HoloLens {
	/// <summary>
	/// Asks the user to do the following
	///		- Make sure there are 3 tracked rigidbodies in the volume at (XZ / -XZ / X-Z)
	///			- Confirm when these are found
	///		- Request the user to place 3 virtual anchors at those places, corresponding with the correct position (XZ / -XZ / X-Z)
	///		-  
	/// </summary>
	public class OptitrackCalibration : MonoBehaviour {
		const string strXZ = "xz";
		const string strMinXMinZ = "minxminz";
		const string strCenter = "optitrackCenter";

		static Vector3 vCenter;
		//anchor positions
		static Vector3 vXZ;
		static Vector3 vMinXMinZ;
		//osc object positions
		static Vector3 vXZOsc;
		static Vector3 vMinXMinZOsc;
		static float yOffset;

		public Canvas feedbackCanvas;
		public Text feedbackText;
		public Button feedbackButton;
		public WrappedAnchor goXZ, goMinXMinZ;
		public Transform optitrackCenter;
		public SceneReference followupScene;

		WorldAnchor anchorXZ, anchorMinXMinZ, anchorCenter;

		WorldAnchorStore store;
		bool done = false;
		bool readyToCalibrate = false;

		void Awake() {
			//setup correct anchor names before they start doing their thing...
			goXZ.anchorName = strXZ;
			goMinXMinZ.anchorName = strMinXMinZ;

			StartCoroutine(DoCalibration());
		}

		IEnumerator DoCalibration(bool recalibrate = false) {
#if !UNITY_EDITOR
			while (!WorldAnchorManager.IsInitialized) {
				yield return null;
			}

			//give it some time
			yield return new WaitForSeconds(2f);

			//get anchor store
			if (store == null) {
				WorldAnchorStore.GetAsync(GotStore);
				while (store == null) {
					//Debug.Log("Bla");
					yield return null;
				}
			}

			//check if we have stored anchorData for this App & WIFI
			anchorXZ = store.Load(strXZ, goXZ.gameObject);
			anchorMinXMinZ = store.Load(strMinXMinZ, goMinXMinZ.gameObject);
			anchorCenter = store.Load(strCenter, optitrackCenter.gameObject);
			if (anchorCenter != null) {
				//for now always recalibrate until it works...
				/*
				if (!recalibrate) {
					//calibration already exists, only continue if we've forced re-calibration
					//store current data (the actual positions will change every time you boot)
					StartCoroutine(StoreCalibrationData());
					yield break;
				} */
			}
#endif

			if (anchorCenter != null) {
				WorldAnchorManager.Instance.RemoveAnchor(optitrackCenter.gameObject);
				yield return new WaitForSeconds(.25f);
				optitrackCenter.transform.position = Vector3.zero;
				optitrackCenter.transform.rotation = Quaternion.identity;
			}

			//display message "please place exactly 2 rigidbodies at XZ, -X-Z (two corners of the room)"
			feedbackCanvas.enabled = true;
			feedbackText.text = "Place rigidbodies called \"xz\" and \"minxminz\" at positive and negative corners of the mocap space (found: 0).";
			feedbackButton.enabled = true;

			List<RigidbodyDefinition> defList = null;

			done = false;
			bool canContinue = false;
			while (!canContinue) {
				//check if there are three rigidbodies, and they are in the desired quadrants
				if (OptiTrackOSCClient.GetAllRigidbodies(out defList)) {
					int activeCount = 0;
					for (int i = 0; i < defList.Count; ++i) {
						if (defList[i].isActive) activeCount++;
					}

					feedbackText.text = "Place rigidbodies called \"xz\" and \"minxminz\" at positive and negative corners of the mocap space (found: " + activeCount + ").";

					if (activeCount >= 2) {
						bool XZ = false, minXMinZ = false;
						foreach (RigidbodyDefinition def in defList) {
							if (def.position.x > 0 && def.position.z > 0 && def.name == "xz") {
								XZ = true;
								vXZOsc = def.position;
							}
							if (def.position.x < 0 && def.position.z < 0 && def.name == "minxminz") {
								minXMinZ = true;
								vMinXMinZOsc = def.position;
							}
						}
						if (XZ && minXMinZ && done) {
							canContinue = true;
							continue;
						}
					}
				}
				yield return null;
			}

			//display message: "align these three anchors with their correct rigidbody as best you can (height & rotation don't need to be exact)"
			//activate button
			feedbackCanvas.enabled = true;
			feedbackText.text = "Align the two virtual anchors with the real objects you just placed. Say \"Next\" when you are done.";
			feedbackButton.enabled = true;

			//activate anchors that user must place
			goXZ.gameObject.SetActive(true);
			goMinXMinZ.gameObject.SetActive(true);

			//re-create/activate their world anchors
			ReAnchor(ref anchorXZ, ref goXZ);
			ReAnchor(ref anchorMinXMinZ, ref goMinXMinZ);

			//make sure they are movable
			goXZ.movable = true;
			goMinXMinZ.movable = true;

			done = false;
			while (!done) {
				readyToCalibrate = true;
				yield return null;
			}

			readyToCalibrate = false;

			//make sure we can't move the anchors anymore...
			goXZ.movable = false;
			goMinXMinZ.movable = false;

			StartCoroutine(StoreCalibrationData());

			feedbackCanvas.enabled = false;

			yield return null;
		}

		public bool MockCalibration(ref Vector3 center, ref float rot, ref float scale, ref Vector3 tr, ref Vector3 bl, ref Vector3 osctr, ref Vector3 oscbl, ref float yOffset ) {
			if (!readyToCalibrate) {
				return false;
			}
			
			//re-get the OSC objects
			RigidbodyDefinition def_xz, def_minxminz;
			if (OptiTrackOSCClient.GetRigidbody(strXZ, out def_xz)) {
				osctr = def_xz.position;
			}
			if (OptiTrackOSCClient.GetRigidbody(strMinXMinZ, out def_minxminz)) {
				oscbl = def_minxminz.position;
			}

			//store our anchored calibration data (virtual positions of 
			tr = goXZ.gameObject.transform.position;
			bl = goMinXMinZ.gameObject.transform.position;

			//store average y-offset for all anchors (gives user some leeway for error)... do we even need this?
			yOffset += tr.y - osctr.y;
			yOffset += bl.y - oscbl.y;
			yOffset = yOffset * .5f;

			//IDEA 1b
			//determine offset between vXZOsc and anchor
			Vector3 vXZOffset = osctr - tr;

			//compare the "same" line, unrotated v rotated, to determine angle between
			Vector3 v1, v2;
			v1 = bl - tr;           //rotated
			v2 = oscbl - osctr;     //unrotated
											//align the height values
			v1.y = v2.y;

			rot = Vector3.Angle(v1, v2);    //does this return a signed value? NOPE
			Vector3 cross = Vector3.Cross(v1, v2);
			if (cross.y < 0) rot = -rot;    //hope this works

			//rotate world center around this angle (how do I know which direction, does this matter?
			//optitrackCenter.transform.rotation = Quaternion.Euler(0, -rot, 0);

			//rotate the optitrack XZ over the same angle
			Vector3 rotatedXZ = Quaternion.Euler(0, -rot, 0) * osctr;
			//subtract this from XZ Object position to get "real center" and position world center there
			center = tr - rotatedXZ * (v1.magnitude / v2.magnitude); //take scale diff into account

			//apply scale
			//OptiTrackOSCClient oscClient = optitrackCenter.GetComponent<OptiTrackOSCClient>();
			//oscClient.scale = (v1.magnitude / v2.magnitude);
			scale = (v1.magnitude / v2.magnitude);

			return true;
		}

		IEnumerator StoreCalibrationData() {
			//re-get the OSC objects
			RigidbodyDefinition def_xz, def_minxminz;
			if ( OptiTrackOSCClient.GetRigidbody(strXZ, out def_xz) ) {
				vXZOsc = def_xz.position;
			}
			if (OptiTrackOSCClient.GetRigidbody(strMinXMinZ, out def_minxminz)) {
				vMinXMinZOsc = def_minxminz.position;
			}

			//store our anchored calibration data (virtual positions of 
			vXZ = goXZ.gameObject.transform.position;
			vMinXMinZ = goMinXMinZ.gameObject.transform.position;

			//store average y-offset for all anchors (gives user some leeway for error)... do we even need this?
			yOffset += vXZ.y - vXZOsc.y;
			yOffset += vMinXMinZ.y - vMinXMinZOsc.y;
			yOffset = yOffset * .5f;

			//IDEA 1b
			//determine offset between vXZOsc and anchor
			Vector3 vXZOffset = vXZOsc - vXZ;

			//compare the "same" line, unrotated v rotated, to determine angle between
			Vector3 v1, v2;
			v1 = vMinXMinZ - vXZ;			//rotated
			v2 = vMinXMinZOsc - vXZOsc;		//unrotated
			//align the height values
			v1.y = v2.y;

			float angle = Vector3.Angle(v1, v2);    //does this return a signed value? NOPE
			Vector3 cross = Vector3.Cross(v1, v2);
			if (cross.y < 0) angle = -angle;    //hope this works

			//clear old anchor!
			if (anchorCenter != null) {
				WorldAnchorManager.Instance.RemoveAnchor(optitrackCenter.gameObject);
				yield return new WaitForSeconds(.25f);
				store.Delete(strCenter);
				yield return new WaitForSeconds(.25f);
			}

			//HACK
			WorldAnchorManager.Instance.AttachAnchor(optitrackCenter.gameObject, strCenter);
			yield return new WaitForSeconds(.25f);
			WorldAnchorManager.Instance.RemoveAnchor(optitrackCenter.gameObject);
			yield return new WaitForSeconds(.25f);

			//rotate world center around this angle (how do I know which direction, does this matter?
			optitrackCenter.transform.rotation = Quaternion.Euler(0, -angle, 0);
			//rotate the optitrack XZ over the same angle
			Vector3 rotatedXZ = Quaternion.Euler(0, -angle, 0) * vXZOsc;
			//subtract this from XZ Object position to get "real center" and position world center there
			optitrackCenter.position = vXZ - rotatedXZ * (v1.magnitude / v2.magnitude); //take scale diff into account

			//apply scale
			OptiTrackOSCClient oscClient = optitrackCenter.GetComponent<OptiTrackOSCClient>();
			oscClient.scale = (v1.magnitude / v2.magnitude);
			
			//for some reason this snaps it to an earlier position? UGH
			WorldAnchorManager.Instance.AttachAnchor(oscClient.gameObject, strCenter);

			yield return new WaitForSeconds(5f);

			//if followup scene is not null, go there...
			if (followupScene != null ) {
				SceneManager.LoadScene(followupScene);
			}
		}

		void ReAnchor( ref WorldAnchor anchor, ref WrappedAnchor go ) {
			if (anchor == null) {
				go.ReAnchor();
			}
		}

		public void ButtonPressed() {
			done = true;
		}

		void GotStore(WorldAnchorStore newStore) {
			Debug.Log("GOT STORE");
			store = newStore;
		}
	}
}