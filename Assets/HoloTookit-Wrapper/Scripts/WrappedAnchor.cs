﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Persistence;
using HoloToolkit.Unity.InputModule;

namespace HKUECT.HoloLens {

	//[RequireComponent(typeof(Collider))]
	public class WrappedAnchor : MonoBehaviour {
		static WorldAnchorStore store;
		static bool GettingStore = false;
		//static bool solverInitialised = false;
		//static bool scanStarted = false;

		public string anchorName = "DefaultUnityAnchor";
		public Transform deferredParent;
		[Tooltip(	"Can you move this with hand-gestures? \n" +
					"This requires a collider to be present on the object.")]
		public bool movable = false;
		public HandDraggable.RotationModeEnum rotationMode = HandDraggable.RotationModeEnum.Default;

		//Collider mCol;
		WorldAnchor anchor;

		void Awake() {
			if (movable) {
				HandDraggable dragComp = gameObject.AddComponent<HandDraggable>();
				dragComp.StartedDragging += StartDrag;
				dragComp.StoppedDragging += StopDrag;
				dragComp.RotationMode = rotationMode;
			}
		}

		// Use this for initialization
		IEnumerator Start() {
			//mCol = GetComponent<Collider>();

			if (deferredParent == null)
				deferredParent = transform;

			while (!WorldAnchorManager.IsInitialized) {
				yield return null;
			}
			WorldErrors.Print("World Anchor Manager Initialized");

			if (store == null) {    //store should be loaded once
				WorldErrors.Print("Getting Store");
				if (!GettingStore) {
					GettingStore = true;
					WorldAnchorStore.GetAsync(GotStore);
				}
				while (store == null)
					yield return null;

				WorldErrors.Print("Got Store");
			}

			WorldAnchor wa = store.Load(anchorName, gameObject);
			if (wa == null) {   //no anchor found
				WorldErrors.Print("No Anchor, creating one");
				NoAnchor();
				WorldAnchorManager.Instance.AttachAnchor(deferredParent.gameObject, anchorName);
				StartCoroutine(AnchorTest());
			}
			else {
				WorldErrors.Print("Loaded Anchor");
				LoadedAnchor();
			}

			yield return null;
		}

		protected virtual void NoAnchor() {

		}

		protected virtual void LoadedAnchor() {

		}

		void GotStore(WorldAnchorStore newStore) {
			store = newStore;
			GettingStore = false;
		}

		void StartDrag() {
			//remove anchor
			WorldErrors.Print("Remove Anchor");
			WorldAnchorManager.Instance.RemoveAnchor(gameObject);
		}

		void StopDrag() {
			//re-add anchor
			WorldErrors.Print("Attach Anchor");
			WorldAnchorManager.Instance.AttachAnchor(gameObject, anchorName);
			StartCoroutine(AnchorTest());
		}

		IEnumerator AnchorTest() {
			yield return new WaitForSeconds(5f);

			WorldAnchor wa = store.Load(anchorName, gameObject);
			if (wa == null) {
				WorldErrors.Print("Anchor null");
			}
			else {
				WorldErrors.Print("Anchor exists");
			}
		}
	}
}