using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityOSC;

public class FakeOSC : MonoBehaviour {

	static OSCClient client;

	public int id;
	public bool flyAround = false;

	Vector3 center;

	// Use this for initialization
	void Start () {
		if (client == null) {
			client = new OSCClient(IPAddress.Parse("127.0.0.1"), 6200);
		}

		center = transform.position;
	}
	
	void OnDestroy() {
		if ( client != null ) {
			client.Close();
			client = null;
		}
	}
	
	void LateUpdate() {
		if (flyAround) {
			Vector3 newPos = center;
			newPos.x = Mathf.Cos(Time.time);
			newPos.z = Mathf.Sin(Time.time);
			transform.position = newPos;
		}

		OSCMessage msg = new OSCMessage("/rigidBody");

		msg.Append(id);
		msg.Append(name);

		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;

		//position
		msg.Append(pos.x);
		msg.Append(pos.y);
		msg.Append(pos.z);

		//rotation (quat)
		msg.Append(rot.x);
		msg.Append(rot.y);
		msg.Append(rot.z);
		msg.Append(rot.w);

		//velocity
		msg.Append(0f);
		msg.Append(0f);
		msg.Append(0f);

		//angvelocity
		msg.Append(0f);
		msg.Append(0f);
		msg.Append(0f);

		//isActive
		msg.Append(1); //always active

		client.Send(msg);
		client.Flush();
	}
}
