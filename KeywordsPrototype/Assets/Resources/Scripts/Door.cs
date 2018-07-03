using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public int keyNum; //how many keys does the player need to unlock the door?
	private AudioSource unlockDoorSFX;
	private bool locked;

	void Start(){
		unlockDoorSFX = GameObject.Find ("UnlockDoorSFX").GetComponent<AudioSource>();
		locked = true;
	}

	public void Unlock(){
		if (locked) {
			locked = false;
			unlockDoorSFX.Play ();
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
