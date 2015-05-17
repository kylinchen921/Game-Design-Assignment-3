using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {
	private int count = 1;
	private GameObject theCamera;
	public bool fightFlag = false;
	[SerializeField]
	private float waitSec;
	//private bool updateFlag = false;
	// Use this for initialization
	IEnumerator Start(){
		theCamera = Camera.main.gameObject;
		if (waitSec == 0) {
			waitSec = 7f;
		}
		//GameObject.Find("BossTrap42").GetComponent<Renderer>().enabled=false;
		//go.SetActive (true);
		GetComponent<Renderer> ().material.color = Color.blue;
		//gameObject.SetActive (false);
		while (true) {
			//Debug.Log("start----" + (waitSec % 5 == 5) + " -- " + count);
			//Debug.Log(count % 5 == 0 && fightFlag);
			if(count % 5 == 0){
				fightFlag = false;
			}
			if(!fightFlag){
				//GameObject lift = GameObject.Find("Lift 2");
				//Vector3 target = new Vector3();
				//target.x = lift.transform.position.x;
				//target.y = lift.transform.position.y;
				//target.z = 55.5f;
				//lift.transform.position = Vector3.Lerp(lift.transform.position, target, 500);
				GameObject.Find("Lift 1").GetComponent<Renderer>().enabled = true;
				GameObject.Find("Lift 2").GetComponent<Renderer>().enabled = true;
				yield return new WaitForSeconds(waitSec);
				//yield return new WaitForSeconds(waitSec);

				//break;
				//waitSec = 15f;
				/*
				GameObject fight = GameObject.Find("BossFightGround 2");

				foreach(Transform child in fight.transform){
					if(child.name.Equals("WeaponCube 1")){
						Vector3 target = new Vector3();
						target.x = 40;
						target.y = 9;
						target.z = 69;
						child.transform.Translate(target);
						child.GetComponent<Rigidbody>().useGravity = true;
					}
					child.gameObject.SetActive(true);
				}
				*/
			//	Debug.Log("if----" + count);
			}else{
				//Debug.Log("-=====");
				yield return new WaitForSeconds(waitSec);
				yield return StartCoroutine(rebuildTrap());
				count ++;
			}
			//waitSec = 7f;
		//GameObject go = GameObject.Find ("BossTrap42");
		//go.SetActive (false);
		//Renderer[] r = go.GetComponents<Renderer>();
		//foreach(Renderer m in r){
		//	m.material.color = Color.blue;
		//	m.enabled = false;
		//}
			//updateFlag = true;
		}
	}

	void Update(){
	 	
	}


	IEnumerator rebuildTrap(){
		//GameObject go = GameObject.Find ("BossTrap42");
		//go.SetActive (true);


		ArrayList seeds = new ArrayList ();
		seeds.Add("BossTrap" + Random.Range(0, 11));
		seeds.Add("BossTrap" + Random.Range(11, 31));
		seeds.Add("BossTrap" + Random.Range(31, 51));
		seeds.Add("BossTrap" + Random.Range(51, 71));
		seeds.Add("BossTrap" + Random.Range(71, 81));
		//Debug.Log ("b--------------");
		GameObject[] gos = GameObject.FindGameObjectsWithTag ("BossTrap");
		foreach (GameObject go in gos) {
			if(!seeds.Contains(go.name)) {
				//go.SetActive(true);
				go.GetComponent<Renderer>().enabled = true;
				go.GetComponent<Rigidbody>().useGravity = true;

			}
		}

		yield return null;


	}

	void OnCollisionEnter(Collision collisionInfo) {
		//Debug.Log (222222222222222);
		if (collisionInfo.gameObject.tag.Equals ("Weapon")) {
			
			GameObject player = GameObject.Find("Player");
			//Vector3 target = GameObject.Find("Teleport 2").transform.position;
			Vector3 target = new Vector3();
			//Debug.Log("x: " + target.x + " y: " + target.y + " z: " + target.z);
			target.x = 23;
			target.y = 15;
			target.z = 54;
			//Debug.Log("x: " + target.x + " y: " + target.y + " z: " + target.z);
			//player.transform.Translate(target);

			//GameObject.Find("Player").transform.position = Vector3.Lerp(GameObject.Find("Player").transform.position, target, 1);

			//Debug.Log(target.y + "----");
			player.transform.position = Vector3.Lerp(player.transform.position, target, 1);
			GameObject.Destroy(collisionInfo.gameObject);
			//Debug.Log("boss attavck");
			//GameObject weapon = GameObject.Find("WeaponCube 1");
			//Vector3 t = weapon.transform.position;
			//t.x = 40;
			//t.y = 40;
			//t.z = 69;
			//t.y = 100;
			//weapon.transform.position = Vector3.Lerp(weapon.transform.position, t, 1);
			//GameObject.Find("WeaponCube 1").GetComponent<Rigidbody>().useGravity = false;
			//weapon.GetComponent<Rigidbody> ().useGravity = false;
			//weapon.GetComponent<Rigidbody>().isKinematic = true;
			//theCamera.GetComponent<CameraShake>().Shake();
		}
	}
}
