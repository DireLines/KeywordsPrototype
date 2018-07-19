using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SetInvis : MonoBehaviour {

	public void Invis(GameObject obj, int playerNum){
		int oldLayerValue = Convert.ToInt32(LayerMask.LayerToName (obj.layer),2);
		oldLayerValue |= (1 << (playerNum - 1));
		obj.layer = LayerMask.NameToLayer(Convert.ToString (oldLayerValue, 2));
	}
}
