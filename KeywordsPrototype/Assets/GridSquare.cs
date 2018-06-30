using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour {

	public char letter;
	public Color highlightedColor;
	[HideInInspector]
	public Color normalColor;
	private SpriteRenderer sr;
	void Start(){
		letter =  transform.parent.gameObject.GetComponent<GridControl> ().placeholder;
		sr = GetComponent<SpriteRenderer> ();
		normalColor = sr.color;
	}
	//TODO: improve this system for selecting the current grid square
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.name.Contains ("Player") && other.isTrigger) {
			transform.parent.gameObject.GetComponent<GridControl> ().SetActive (gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.name.Contains ("Player") && other.isTrigger) {
			sr.color = normalColor;
		}
	}
}
