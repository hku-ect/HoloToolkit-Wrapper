using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HKUECT.HoloLens {
	public enum WrappedPlacement {
		Place_OnFloor,
		Place_OnWall,
		Place_OnCeiling,
		Place_OnShape,
		Place_OnEdge,
		Place_OnFloorAndCeiling,
		Place_RandomInAir,
		Place_InMidAir,
		Place_UnderPlatformEdge,
	}

	/// <summary>
	/// This class makes it simpler to use the HoloToolkit's object placement functionality.
	/// It's main goals are:
	///		- Reduce lines of code needed
	///		- Reduce length of code lines needed (due to nested names / enums / etc) to improve legibility
	/// </summary>
	public class PlacementWrapper : MonoBehaviour {

		static bool solverInitialized = false;
		static PlacementWrapper wrapper;

		static bool Init() {
			if (solverInitialized)
				return true;
			if (SpatialUnderstandingDllObjectPlacement.Solver_Init() == 1)
				solverInitialized = true;
			return solverInitialized;
		}

		/// <summary>
		/// Attempts to place an object given the parameters.
		/// Returns false if object could not be placed, or spatial mapping could not be initialized.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="position"></param>
		/// <param name="customHalfDims"></param>
		/// <param name="minDistFromOthers"></param>
		/// <param name="minDistFromPlayer"></param>
		/// <param name="maxDistFromPlayer"></param>
		/// <returns></returns>
		public static bool PlaceObject(GameObject target, WrappedPlacement position, Vector3? customHalfDims = null, float minDistFromOthers = 3f, float minDistFromPlayer = .25f, float maxDistFromPlayer = 4.0f) {
			if (!Init())
				return false;

			Vector3 halfDims = Vector3.one * .5f;
			Renderer r = target.GetComponentInChildren<Renderer>();
			if (r != null) {
				halfDims = (r.bounds.size * .5f);
				WorldErrors.Print("halfDims: " + (r.bounds.size * .5f).ToString());
			}

			if (customHalfDims != null)
				halfDims = customHalfDims.Value;

			List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
			SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects (halfDims.magnitude * minDistFromOthers)
		};

			List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint>() {
			SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearPoint (Camera.main.transform.position, minDistFromPlayer, maxDistFromPlayer)
		};

			SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition def = new SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition();

			switch (position) {
				case WrappedPlacement.Place_InMidAir:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_InMidAir(halfDims);
					break;
				case WrappedPlacement.Place_OnCeiling:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnCeiling(halfDims);
					break;
				case WrappedPlacement.Place_OnEdge: {
						Vector3 halfDimsBot = halfDims;
						halfDimsBot.y *= .5f;
						def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnEdge(halfDims, halfDimsBot);
					}
					break;
				case WrappedPlacement.Place_OnFloor:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(halfDims);
					break;
				case WrappedPlacement.Place_OnFloorAndCeiling: {
						Vector3 halfDimsBot = halfDims;
						halfDimsBot.y *= .5f;
						def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloorAndCeiling(halfDims, halfDimsBot);
					}
					break;
				case WrappedPlacement.Place_OnShape:
					Debug.LogWarning("Not supported");
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnShape(halfDims, "DefaultShape", 0);
					break;
				case WrappedPlacement.Place_OnWall:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(halfDims, halfDims.y, 2.4f);
					break;
				case WrappedPlacement.Place_RandomInAir:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_RandomInAir(halfDims);
					break;
				case WrappedPlacement.Place_UnderPlatformEdge:
					def = SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_UnderPlatformEdge(halfDims);
					break;
			}

			//SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult result = new SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult();
			if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(target.name,
				SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(def),
				placementRules.Count, SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementRules.ToArray()),   //rules
				placementConstraints.Count, SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementConstraints.ToArray()),   //constraints
				SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()
			) > 0) {
				target.transform.position = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult().Position;
				return true;
			}

			return false;
		}

		SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType FromWrapped(WrappedPlacement position) {
			return (SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType)((int)position);
		}
	}
}