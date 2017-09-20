using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
	}
}