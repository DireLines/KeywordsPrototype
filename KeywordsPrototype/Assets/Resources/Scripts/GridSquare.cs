﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour {
	public GameObject tile;
//	public char letter;
	public Color highlightedColor;
	[HideInInspector]
	public Color normalColor;
	private SpriteRenderer sr;
	void Awake(){
//		letter = transform.parent.gameObject.GetComponent<GridControl> ().placeholder;
//		letter = (char)Random.Range (97, 123);
		tile = null;
		sr = GetComponent<SpriteRenderer> ();
		normalColor = sr.color;
	}
	//TODO: improve this system for selecting the current grid square
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.name.Contains ("Player") && other.isTrigger) {
			other.transform.gameObject.GetComponent<Inventory> ().SetActive (gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.name.Contains ("Player") && other.isTrigger) {
			sr.color = normalColor;
			other.transform.gameObject.GetComponent<Inventory> ().SetInactive ();
		}
	}

	public char GetLetter(){
		if (tile == null) {
			return transform.parent.gameObject.GetComponent<GridControl>().placeholder;
		}
		return tile.GetComponent<LetterTile>().letter;
	}

	public void SetTile(GameObject newTile){
		if (!newTile.CompareTag ("LetterTile")) {
			print ("tried to set tile to something not tagged as LetterTile");
		}
		tile = newTile;
	}
}
