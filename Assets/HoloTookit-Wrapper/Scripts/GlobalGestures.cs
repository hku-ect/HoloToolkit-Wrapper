using UnityEngine.VR.WSA.Input;

namespace HKUECT.HoloLens {
	/// <summary>
	/// Automatically creates & manages a static GestureRecognizer instance so you don't have to
	/// </summary>
	public static class GlobalGestures {
		static GestureRecognizer _recognizer;
		public static GestureRecognizer recognizerInstance {
			get {
				if ( _recognizer == null ) {
					_recognizer = new GestureRecognizer();
					_recognizer.StartCapturingGestures();
				}
				return _recognizer;
			}
		}
	}
}