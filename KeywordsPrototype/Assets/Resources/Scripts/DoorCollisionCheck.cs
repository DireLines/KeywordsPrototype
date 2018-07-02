using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollisionCheck : MonoBehaviour {

	public Transform doors;//door container object
	public int keys;//how many keys does the player have?

	void Start(){
		keys = 0;
		print ("number of keys: " + keys);
	}
	void Update(){
		SetDoorCollisions ();
	}
	private void SetDoorCollisions(){
		foreach (Transform child in doors) {
			Door door = child.gameObject.GetComponent<Door> ();
			if (keys >= door.keyNum) {
//				print ("Ayy");
				Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), door.GetComponent<BoxCollider2D> ());
			}
			else {
//				print ("yyA");
				Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), door.GetComponent<BoxCollider2D> (),false);
			}
		}
	}

}
