

namespace HKUECT.HoloLens {
	/// <summary>
	/// Automatically creates & manages a static GestureRecognizer instance so you don't have to
	/// </summary>
	public static class GlobalGestures {
		static UnityEngine.XR.WSA.Input.GestureRecognizer _recognizer;
		public static UnityEngine.XR.WSA.Input.GestureRecognizer recognizerInstance {
			get {
				if ( _recognizer == null ) {
					_recognizer = new UnityEngine.XR.WSA.Input.GestureRecognizer();
					_recognizer.StartCapturingGestures();
				}
				return _recognizer;
			}
		}
	}
}