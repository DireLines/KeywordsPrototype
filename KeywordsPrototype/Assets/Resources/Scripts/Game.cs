using UnityEngine;
using System.Collections;
using System;

public static class Game
{
	public static void SetInvis(GameObject obj, int playerNum){
		if (obj.layer < 16) {//if it's not one of the visibility affected layers
			return;
		}
		int oldLayerValue = Convert.ToInt32(LayerMask.LayerToName (obj.layer),2);
		oldLayerValue |= (1 << (playerNum - 1));
		obj.layer = LayerMask.NameToLayer(Convert.ToString (oldLayerValue, 2).PadLeft(4,'0'));
	}
}

