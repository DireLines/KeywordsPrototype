using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInvis : MonoBehaviour {

	public void Invis(GameObject obj, int playerNum){
		obj.layer = LayerMask.NameToLayer("Hidden" + playerNum);
	}
}
