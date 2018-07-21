﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControl : MonoBehaviour {
	public GameObject[,] grid;
	private List<GameObject> reachedTiles;
	private List<GameObject> validWordTiles;
	public char placeholder = ' ';
	private Words words;
	private AudioSource getKeySource;
	public int ownerNum;
	private GameObject owner;

	void Awake(){
		grid = new GameObject[GetComponent<MakeGrid> ().width, GetComponent<MakeGrid> ().width]; 
		reachedTiles = new List<GameObject> ();
		validWordTiles = new List<GameObject> ();
	}

	void Start(){
		words = GameObject.Find ("GM").GetComponent<Words> ();
		getKeySource = GameObject.Find ("GetKeySFX").GetComponent<AudioSource> ();
		if (ownerNum != 0) {
			owner = GameObject.Find ("Player" + ownerNum);
			//recolor grid squares
			foreach (Transform child in transform) {
				Color ownerColor = owner.GetComponent<SpriteRenderer> ().color;
				float d = 0.7f;
				Color darkerColor = new Color (ownerColor.r * d, ownerColor.g * d, ownerColor.b * d, 1f);
				child.gameObject.GetComponent<SpriteRenderer> ().color = darkerColor;
				child.gameObject.GetComponent<GridSquare> ().normalColor = darkerColor;
				child.gameObject.GetComponent<GridSquare>().highlightedColor = ownerColor;
			}
		}
	}

	//called on any space in the grid the player just interacted with to see if any new words have formed
	public void ValidateWords(int x,int y, GameObject player){
		if (ownerNum != 0 && owner != player) {
			return;
		}
		validWordTiles.Clear ();
		if (grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
			if(words.ValidateWord(GetHorizontalWord(x-1,y),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					validWordTiles.Add(tile);
				}
			}
			if(words.ValidateWord(GetHorizontalWord(x+1,y),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					validWordTiles.Add(tile);
				}
			}
			if(words.ValidateWord(GetVerticalWord(x,y-1),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					validWordTiles.Add(tile);
				}
			}
			if(words.ValidateWord(GetVerticalWord(x,y+1),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					validWordTiles.Add(tile);
				}
			}
		} else {
			if(words.ValidateWord(GetHorizontalWord(x,y),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					validWordTiles.Add(tile);
				}
			}
			if(words.ValidateWord(GetVerticalWord(x,y),ownerNum)){
				player.GetComponent<DoorCollisionCheck>().keys++;
				foreach(GameObject tile in reachedTiles){
					if (!validWordTiles.Contains (tile)) {
						validWordTiles.Add (tile);
					}
				}
			}
		}
		foreach (GameObject tile in validWordTiles) {
			tile.GetComponent<LetterTile> ().Dec ();
		}
		validWordTiles.Clear ();
	}

	//Gets the longest word including tile [x,y] which is horizontal
	public string GetHorizontalWord(int x, int y){
		reachedTiles.Clear ();
		int width = GetComponent<MakeGrid> ().width;
		if (x < 0 || x >= width || y < 0 || y >= width || grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
//			print ("out of bounds - GetHorizontalWord returning empty string");
			return "";
		}
		string result = grid [x, y].GetComponent<GridSquare> ().GetLetter().ToString();
		reachedTiles.Add (grid [x, y].GetComponent<GridSquare> ().tile);
		int i = x-1;
		while (i != -1) {
//			print (i);
			if (i < 0 || grid [i, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result = grid [i, y].GetComponent<GridSquare> ().GetLetter() + result;
				reachedTiles.Add (grid [i, y].GetComponent<GridSquare> ().tile);
				i--;
			}
		}
		i = x+1;
		while (i != -1) {
//			print (i);
			if (i >= width || grid [i, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result += grid [i, y].GetComponent<GridSquare> ().GetLetter();
				reachedTiles.Add (grid [i, y].GetComponent<GridSquare> ().tile);
				i++;
			}
		}
		return result;
	}

	//Gets the longest word including tile [x,y] which is vertical
	public string GetVerticalWord(int x, int y){
		reachedTiles.Clear ();
		int width = GetComponent<MakeGrid> ().width;
		if (x < 0 || x >= width || y < 0 || y >= width || grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
//			print ("out of bounds - GetVerticalWord returning empty string");
			return "";
		}
		string result = grid [x, y].GetComponent<GridSquare> ().GetLetter().ToString();
		reachedTiles.Add (grid [x, y].GetComponent<GridSquare> ().tile);
		int i = y-1;
		while (i != -1) {
			//			print (i);
			if (i < 0 || grid [x,i].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result = grid [x,i].GetComponent<GridSquare> ().GetLetter() + result;
				reachedTiles.Add (grid [x,i].GetComponent<GridSquare> ().tile);
				i--;
			}
		}
		i = y+1;
		while (i != -1) {
			//			print (i);
			if (i >= width || grid [x,i].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result += grid [x,i].GetComponent<GridSquare> ().GetLetter();
				reachedTiles.Add (grid [x,i].GetComponent<GridSquare> ().tile);
				i++;
			}
		}
		return result;
	}
}
