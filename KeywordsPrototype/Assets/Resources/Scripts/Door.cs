using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public int keyNum; //how many keys does the player need to unlock the door?
	private AudioSource unlockDoorSFX;
	private bool[] locked;
	private GameObject lockedSprite;//child sprite object showing door as locked to players who haven't unlocked it

	void Start(){
		unlockDoorSFX = GameObject.Find ("UnlockDoorSFX").GetComponent<AudioSource>();
		locked = new bool[4]{true,true,true,true};
		lockedSprite = transform.GetChild (0).gameObject;
	}

	public void Unlock(int playerNum){
		if (locked[playerNum-1] == true) {
			locked[playerNum-1] = false;
			Game.SetInvis (lockedSprite, playerNum);
			unlockDoorSFX.Play ();
		}
	}
}
