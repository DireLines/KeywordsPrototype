using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager {
	public static int level = 0;

	public static void GoToNextLevel(){
		level++;
		GameObject gm = GameObject.Find ("GM");
		gm.GetComponent<Words> ().UpdateLevelWords (level);
		gm.GetComponent<MakeWalls> ().MakeFloor ();
	}
}
