using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public int keyNum; //how many keys does the player need to unlock the door?
	private AudioSource unlockDoorSFX;
	private bool locked;
	private SetInvis invis;
	private GameObject lockedSprite;//child sprite object showing door as locked to players who haven't unlocked it

	void Start(){
		invis = GameObject.Find ("GM").GetComponent<SetInvis> ();
		unlockDoorSFX = GameObject.Find ("UnlockDoorSFX").GetComponent<AudioSource>();
		locked = true;
		lockedSprite = transform.GetChild (0).gameObject;
	}

	public void Unlock(GameObject ByWhom){
		if (locked) {
			locked = false;
			ByWhom.GetComponent<PlayerInfo> ().SetInvis (lockedSprite);
			unlockDoorSFX.Play ();
		}
	}
}
