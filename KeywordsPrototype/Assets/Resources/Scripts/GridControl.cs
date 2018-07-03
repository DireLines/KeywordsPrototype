using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControl : MonoBehaviour {
	public GameObject[,] grid;
	public char placeholder = ' ';
	public Words words;

	void Awake(){
		grid = new GameObject[GetComponent<MakeGrid> ().width, GetComponent<MakeGrid> ().width]; 
	}

	void Start(){
		foreach (string thing in GetWordsInGrid(2,4)) {
			print (thing);
		}
	}

	public void ValidateWords(int x,int y, GameObject player){
		foreach (string word in GetWordsInGrid(x,y)) {
			if (words.ValidateWord (word)) {
				player.GetComponent<DoorCollisionCheck> ().keys++;
			}
		}
	}

	//called on any space in the grid the player just interacted with to see if any new words have formed
	public List<string> GetWordsInGrid(int x,int y){
		List<string> words = new List<string> ();
		if (grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
			words.Add(GetHorizontalWord(x-1,y));
			words.Add(GetHorizontalWord(x+1,y));
			words.Add(GetVerticalWord(x,y-1));
			words.Add(GetVerticalWord(x,y+1));
		} else {
			words.Add (GetHorizontalWord (x, y));
			words.Add (GetVerticalWord (x, y));
		}
		return words;
	}

	//Gets the longest word including tile [x,y] which is horizontal
	public string GetHorizontalWord(int x, int y){
		int width = GetComponent<MakeGrid> ().width;
		if (x < 0 || x >= width || y < 0 || y >= width || grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
			print ("out of bounds - GetHorizontalWord returning empty string");
			return "";
		}
		string result = grid [x, y].GetComponent<GridSquare> ().GetLetter().ToString();
		int i = x-1;
		while (i != -1) {
//			print (i);
			if (i < 0 || grid [i, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result = grid [i, y].GetComponent<GridSquare> ().GetLetter() + result;
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
				i++;
			}
		}
		return result;
	}

	//Gets the longest word including tile [x,y] which is vertical
	public string GetVerticalWord(int x, int y){
		int width = GetComponent<MakeGrid> ().width;
		if (x < 0 || x >= width || y < 0 || y >= width || grid [x, y].GetComponent<GridSquare> ().GetLetter() == placeholder) {
			print ("out of bounds - GetVerticalWord returning empty string");
			return "";
		}
		string result = grid [x, y].GetComponent<GridSquare> ().GetLetter().ToString();
		int i = y-1;
		while (i != -1) {
			//			print (i);
			if (i < 0 || grid [x,i].GetComponent<GridSquare> ().GetLetter() == placeholder) {
				i = -1;
			} else {
				result = grid [x,i].GetComponent<GridSquare> ().GetLetter() + result;
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
				i++;
			}
		}
		return result;
	}
}
