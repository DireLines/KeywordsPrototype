using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public int inventorySlot;
	public const float pickupRadius = 0.3f;
	const int inventorySize = 5;
	GameObject[] items;
//	float[] oldLinearDrags;
	// Use this for initialization
	void Start () {
		items = new GameObject[inventorySize];
		inventorySlot = 0;
	}

	// Update is called once per frame
	void Update () {
		//Interact with world
		if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown (KeyCode.Joystick1Button16)) {
			Interact ();
		} else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown (KeyCode.Joystick1Button17)) {
			Drop ();
		}

		//Change which item is 
		if (Input.GetKeyDown (KeyCode.Joystick1Button13)){
			SwitchSlot(correctmod(inventorySlot-1,inventorySize));
		} else if (Input.GetKeyDown (KeyCode.Joystick1Button14)) {
			SwitchSlot(correctmod(inventorySlot+1,inventorySize));
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			SwitchSlot (0);
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			SwitchSlot (1);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			SwitchSlot (2);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			SwitchSlot (3);
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			SwitchSlot (4);
		}
//		print (inventorySlot);
	}

	//C# mod is not too useful
	int correctmod(int a,int n){
		return ((a % n) + n) % n;
	}

	private void SwitchSlot(int n){
		if (n < 0 || n >= inventorySize) {
			print ("tried to change inventory slot to a weird number");
			return;
		}
		if(items [inventorySlot] != null){
			items [inventorySlot].SetActive (false);
		}
		inventorySlot = n;
		if (items [inventorySlot] != null) {
			items [inventorySlot].SetActive (true);
		}

	}
	private void Interact(){
		print ("interacting");
		if (items [inventorySlot] == null) {
			//pick up nearest item within pickup radius
			Collider2D[] itemsWithinRadius = Physics2D.OverlapCircleAll(transform.position,pickupRadius,1<<LayerMask.NameToLayer("Pickup"));
			if (itemsWithinRadius.Length > 0) {
				float minDistance = (itemsWithinRadius [0].gameObject.transform.position - transform.position).magnitude;
				GameObject closestObject = itemsWithinRadius [0].gameObject;
				foreach (Collider2D item in itemsWithinRadius) {
					if ((item.gameObject.transform.position - transform.position).magnitude < minDistance) {
						minDistance = (item.gameObject.transform.position - transform.position).magnitude;
						closestObject = item.gameObject;
					}
				}
				items [inventorySlot] = closestObject;
				closestObject.transform.parent = transform;
				if (closestObject.GetComponent<BoxCollider2D> () != null) {
					closestObject.GetComponent<BoxCollider2D> ().enabled = false;
				}
			}
		}
	}

	private void Drop(){
		print ("dropping");
		if (items [inventorySlot] != null) {
			items [inventorySlot].transform.SetParent (null);
			items [inventorySlot].GetComponent<BoxCollider2D> ().enabled = true;
			items [inventorySlot] = null;
		}
	}

}
