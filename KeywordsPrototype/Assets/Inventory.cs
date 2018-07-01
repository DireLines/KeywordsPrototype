using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public int inventorySlot;
	const int inventorySize = 5;
	// Use this for initialization
	void Start () {
		inventorySlot = 0;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Joystick1Button13)){
			inventorySlot = correctmod(inventorySlot-1,inventorySize);
		} else if (Input.GetKeyDown (KeyCode.Joystick1Button14)) {
			inventorySlot = correctmod(inventorySlot+1,inventorySize);
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			inventorySlot = 0;
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			inventorySlot = 1;
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			inventorySlot = 2;
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			inventorySlot = 3;
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			inventorySlot = 4;
		}
		print (inventorySlot);
	}

	//C# mod is not too useful
	int correctmod(int a,int n){
		return ((a % n) + n) % n;
	}
}
