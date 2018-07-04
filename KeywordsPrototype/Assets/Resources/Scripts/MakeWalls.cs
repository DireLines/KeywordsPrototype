using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWalls : MonoBehaviour {
	//back end - grid of rooms
	public int width;
	public int height;
	private int numSquares;
	private int numCheckedOff;
	private int roomNum;
	[HideInInspector]
	public int[,] rooms;

	//translation - variables for turning grid into gameobjects

	//front end - generated game objects
	public float wallSpacing;
	public GameObject DoorContainer;
	public GameObject WallContainer;
	public GameObject VerticalWall;
	public GameObject HorizontalWall;
	public GameObject Door;
	public GameObject VerticalWallSmall;
	public GameObject HorizontalWallSmall;


	// Use this for initialization
	void Awake(){
		FillRooms ();
//		PrintRooms ();
		GenerateWalls ();
	}


	//BACK END
	void FillRooms(){
		rooms = new int[width, height];
		roomNum = 1;
		numCheckedOff = 0;
		numSquares = width * height;
		while (numCheckedOff < numSquares) {
			MakeRoom (Random.Range(0,width),Random.Range(0,width),Random.Range(5,10),Random.Range(5,10));
		}
	}

	//is this coordinate in bounds?
	bool InBounds(int x, int y){
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	//makes a room of width w and height h centered at [x,y]
	void MakeRoom(int x, int y, int w, int h){
		for (int i = -w / 2; i <= w / 2; i++) {
			for (int j = -h / 2; j <= h / 2; j++) {
				int a = i + x;
				int b = j + y;
//				print(a+" "+b);
				if (InBounds (a, b)) {
					if (rooms [a, b] == 0) {
						numCheckedOff++;
					}
					rooms [a, b] = roomNum;
				}
			}
		}
		roomNum++;
	}

	//C# mod is not too useful
	int correctmod(int a,int n){
		return ((a % n) + n) % n;
	}

	//test if back end is giving the right output by mapping room nums to ascii chars and filling a string with that
	//makes it absurdly slow for large sizes because Unity's print buffer is expecting a whole bunch of small messages rather than a single large message
	void PrintRooms(){
		string result = "";
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				result += (char)(correctmod (rooms [j, i], 94) + 32) + " ";
			}
			result += "\n";
		}
		print (result);
	}



	void GenerateWalls(){
		print ("yo");
	}
}
