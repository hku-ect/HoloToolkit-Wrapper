using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HKUECT.HoloLens {
	public class PlacableWorldAnchor : WrappedAnchor {
		public bool spatialMapIfNotAnchored = true;
		public WrappedPlacement placementType = WrappedPlacement.Place_OnFloor;
		public bool useCustomShape = false;
		public Vector3 customShape = Vector3.one;
		public float maxHeight = 2.4f;
		public List<GameObject> activateWhenPlaced;

		void OnDrawGizmos() {
			if (spatialMapIfNotAnchored && useCustomShape) {
				if (deferredParent == null)
					deferredParent = transform;
				Gizmos.DrawWireCube(deferredParent.transform.position, customShape);
			}
		}

		protected override void NoAnchor() {
			StartCoroutine(DoPlacement(0));
		}

		protected override void LoadedAnchor() {
			foreach (GameObject g in activateWhenPlaced) {
				g.SetActive(true);
			}
		}

		IEnumerator DoPlacement( int tries ) {
			if (tries > 1) yield break;

			if (spatialMapIfNotAnchored) {
				SpatialWrapper.RunSpatialMapping(false);
				while (!SpatialWrapper.SpatialInfoReady)
					yield return null;

				Vector3? customHalfDims = null;
				if (useCustomShape)
					customHalfDims = customShape * .5f;

				if (!PlacementWrapper.PlaceObject(deferredParent.gameObject, placementType, customHalfDims)) {
					WorldErrors.Print("Could not place object");
					//retry in a little while
					yield return new WaitForSeconds(10f);
					DoPlacement(tries++);
				}
				else {
					WorldErrors.Print("Placed Object: " + anchorName);
					foreach (GameObject g in activateWhenPlaced) {
						g.SetActive(true);
					}
				}
			}

			yield return null;
		}
	}
}