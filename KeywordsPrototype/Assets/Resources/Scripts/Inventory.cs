using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	const int inventorySize = 5; //how big is the inventory?
	public int inventorySlot;//which slot is currently active?
	const float pickupRadius = 0.2f; //how far away can the player pick up an object?
	public GameObject activeSquare;//the grid square the player's currently on
	GameObject[] items; //references to the gameobjects in inventory
	public Vector3 holdOffset; //what's the hold position of the inventory item?
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

		//Change which item is active
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
//		if (activeSquare != null) {
//			print (activeSquare.GetComponent<GridSquare> ().x + " " + activeSquare.GetComponent<GridSquare> ().y);
//		}
	}

	//C# mod is not too useful
	int correctmod(int a,int n){
		return ((a % n) + n) % n;
	}
		
	public void SetActive(GameObject newSquare){
		if (activeSquare != null) {
			activeSquare.GetComponent<SpriteRenderer> ().color = activeSquare.GetComponent<GridSquare> ().normalColor;
		}
		activeSquare = newSquare;
		activeSquare.GetComponent<SpriteRenderer> ().color = activeSquare.GetComponent<GridSquare> ().highlightedColor;
	}

	public void SetInactive(){
		activeSquare = null;
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

	//pseudocode of this:
	/*

	x = is player hovering over a grid square?
	y = is player currently holding something in inventory?
	z = is player holding a letter tile?
	w = is there a letter tile on the grid square?


	x y z w : swap inventory item with thing on the square
	x y z !w : place inventory item on the square 
	x y !z !w : Perform item’s action
	x !y !z w : take tile on square into inventory
	x !y !z !w : normal grab
	!x y !z !w : Perform item’s action
	!x !y !z !w : normal grab

	all other combinations are impossible or do nothing
	 */
	private void Interact(){
		print ("interacting");
		bool x = (activeSquare != null);
		bool y = (items [inventorySlot] != null); 
		bool z = y ? items [inventorySlot].CompareTag ("LetterTile") : false;
		bool w = x ? activeSquare.GetComponent<GridSquare> ().GetLetter () != activeSquare.transform.parent.gameObject.GetComponent<GridControl> ().placeholder : false;
		print (x + " " + y + " " + z + " " + w);

		if (!y && !z && !w) {
			NormalGrab ();
		} else if (y && !z && !w) {
			PerformItemAction ();
		} else if (x) {
			if (y && z) {
				if (w) {
					SwapWithSquare ();
				} else {
					PlaceOnSquare ();
				}
			} else if (!y && !z && w) {
				TakeFromSquare ();
			}
		}
	}

	private void PerformItemAction(){
		print ("performing super cool item action");
	}

	private void Drop(){
		print ("dropping");
		if (activeSquare == null) {
			if (items [inventorySlot] != null) {
				items [inventorySlot].transform.SetParent (null);
				if (items [inventorySlot].GetComponent<BoxCollider2D> () != null) {
					items [inventorySlot].GetComponent<BoxCollider2D> ().enabled = true;
				}
				if (items [inventorySlot].GetComponent<Rigidbody2D> () != null) {
					items [inventorySlot].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
					items [inventorySlot].GetComponent<Rigidbody2D> ().isKinematic = false;
					items [inventorySlot].GetComponent<Rigidbody2D> ().freezeRotation = false;
				}
				items [inventorySlot] = null;
			}
		}
	}

	private void PlaceOnSquare(){
		print ("placing tile on square");
		items [inventorySlot].transform.SetParent (null);
		items [inventorySlot].transform.position = activeSquare.transform.position;
		activeSquare.GetComponent<GridSquare> ().SetTile (items [inventorySlot]);
		items [inventorySlot] = null;
	}


	private void TakeFromSquare(){
		print ("taking from square");
		items [inventorySlot] = activeSquare.GetComponent<GridSquare> ().tile;
		items [inventorySlot].transform.SetParent (transform);
		items [inventorySlot].transform.localPosition = holdOffset;
		items [inventorySlot].transform.rotation = Quaternion.identity;
		activeSquare.GetComponent<GridSquare> ().SetTile (null);
	}
	private void SwapWithSquare(){
		print ("swapping tile with square");
		GameObject temp = activeSquare.GetComponent<GridSquare> ().tile;
		temp.transform.SetParent (transform);
		temp.transform.localPosition = holdOffset;
		temp.transform.rotation = Quaternion.identity;
		PlaceOnSquare ();
		items [inventorySlot] = temp;
	}


	private void NormalGrab(){
		print ("grabbing");
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
			//put item in inventory
			items [inventorySlot] = closestObject;
			closestObject.transform.SetParent (transform);
			closestObject.transform.localPosition = holdOffset;
			closestObject.transform.rotation = Quaternion.identity;
			if (closestObject.GetComponent<BoxCollider2D> () != null) {
				closestObject.GetComponent<BoxCollider2D> ().enabled = false;
			}
			if (closestObject.GetComponent<Rigidbody2D> () != null) {
				items [inventorySlot].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				closestObject.GetComponent<Rigidbody2D> ().isKinematic = true;
				closestObject.GetComponent<Rigidbody2D> ().freezeRotation = true;
			}
		}
	}
}
