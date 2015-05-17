using UnityEngine;
using System.Collections;

public class WeaponTrick : MonoBehaviour {

	//private Vector3 beginPosition;
	// Use this for initialization
	void Start () {
		//beginPosition = gameObject.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionStay(Collision collisionInfo) {
		//Debug.Log (1111111111111);
		if (collisionInfo.gameObject.name.Equals ("Player")) {
			//GameObject go = GameObject.Find("Teleport 3");
		//	Debug.Log (2);
			//Vector3 t = go.transform.position;
			//t.y = 80;
			//Debug.Log(t.x + "-" + t.y + "-" + t.z);
			//go.transform.position = Vector3.Lerp(go.transform.position, t, 0);
			GameObject.Find("WeaponCube 1").GetComponent<Rigidbody> ().useGravity = true;
		}
	}

}