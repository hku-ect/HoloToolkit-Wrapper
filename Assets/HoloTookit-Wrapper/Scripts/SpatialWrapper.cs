using System.Collections;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA;


namespace HKUECT.HoloLens {
	public delegate void SpatialWrapperCallback();

	/// <summary>
	/// This class simplifies using the HoloToolkit's spatial understanding systems.
	/// Primary goals:
	///		- Reduce lines needed
	///		- Reduce use of nested types that increase length of code lines
	/// </summary>
	public class SpatialWrapper : MonoBehaviour {
		public static bool SpatialInfoReady {
			get;
			private set;
		}

		public static bool Running {
			get {
				return running;
			}
		}

		public static SpatialWrapperCallback ready;

		static SpatialWrapper wrapper;
		static bool running = false;
		static float timer = 0;
		static float targetTime = 60f;

		/// <summary>
		/// Starts spatial mapping, which will scan the room and visualize this until the target time is reached, or it is manually stopped.
		/// </summary>
		public static void RunSpatialMapping( bool overrideExisting = false, float timeToScan = 60f ) {
			WorldErrors.Print("WTF");
			if (wrapper == null) {
				wrapper = new GameObject("SpatialWrapper").AddComponent<SpatialWrapper>();
			}
			if (!running) {
				if (!SpatialInfoReady || overrideExisting) {
					targetTime = timeToScan;
					wrapper.StartCoroutine(DoMapping());
					WorldErrors.Print("DO MAPPING");
				}
				else {
					WorldErrors.Print("Eh: "+ SpatialInfoReady + ","+ overrideExisting);
				}
			}
			else {
				WorldErrors.Print("Already running");
			}
		}

		/// <summary>
		/// Manually stop a running spatial mapping
		/// </summary>
		public static void RequestFinish() {
			if (running) {
				if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) {
					SpatialUnderstanding.Instance.RequestFinishScan();
					SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = false;
				}
			}
		}
		
		static IEnumerator DoMapping() {
			if (SpatialInfoReady) {
				WorldErrors.Print("Spatial Info Already There");
				yield break;
			}

			//turn off rendering if we're not scanning, store old mode
			SpatialMappingRenderer smRend = FindObjectOfType<SpatialMappingRenderer>();
			SpatialMappingRenderer.RenderState oldState = SpatialMappingRenderer.RenderState.Occlusion;
			if (smRend != null) {
				oldState = smRend.renderState;
				smRend.renderState = SpatialMappingRenderer.RenderState.None;	
			}
			running = true;

			//is there an instance present?
			SpatialUnderstanding puInst = SpatialUnderstanding.Instance;
			if (puInst == null || !puInst.AllowSpatialUnderstanding) {
				Debug.LogError("No Spatial Understanding Instance, or not supported in build.");
				WorldErrors.Print("No Instance");
				yield break;
			}

			//can it scan?
			while (!SpatialUnderstanding.IsInitialized) {
				WorldErrors.Print("Not initialized");
				yield return null;
			}

			puInst.UnderstandingCustomMesh.DrawProcessedMesh = true;

			bool requestedFinish = false;
			WorldErrors.Print("" + puInst.AutoBeginScanning + ", " + puInst.ScanState);
			if (!puInst.AutoBeginScanning && (puInst.ScanState == SpatialUnderstanding.ScanStates.ReadyToScan || puInst.ScanState == SpatialUnderstanding.ScanStates.None || puInst.ScanState == SpatialUnderstanding.ScanStates.Done )) {
				WorldErrors.Print("Request Begin Scan");
				puInst.RequestBeginScanning();
			}

			//yield until scan automatically finishes, or it is stopped, or time runs out
			while (puInst.ScanState != SpatialUnderstanding.ScanStates.Done) {
				timer += Time.deltaTime;
				if (!requestedFinish && timer > targetTime) {
					puInst.RequestFinishScan();
					requestedFinish = true;
					puInst.UnderstandingCustomMesh.DrawProcessedMesh = false;
				}
				yield return null;
			}

			//signal we're ready to share data
			SpatialInfoReady = true;
			if (ready != null) {
				ready();
			}

			//reset rendering again if we have a mapping renderer
			if (smRend != null) {
				smRend.renderState = oldState;
			}

			running = false;
		}
	}
}