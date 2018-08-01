﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A representation of a room in a dungeon
class Room {
	public List<Vector2Int> squares;
	public List<Room> neighbors;//list of all neighboring rooms
	public List<GameObject> doors;//list of doors at edge of this room
	public bool reached; //have I been reached in MST?
	public Room(){
		squares = new List<Vector2Int> ();
		neighbors = new List<Room> ();
		doors = new List<GameObject> ();
		reached = false;
	}

	//spawns item somewhere within the confines of the room
	//default rotation
	public GameObject SpawnItem (GameObject item, Transform parent = null){
		int randomSquareIndex = Random.Range (0, squares.Count);
		Vector2Int square = squares [randomSquareIndex];
		Vector3 pos = GameObject.Find ("GM").GetComponent<MakeWalls> ().GetCellPositionFor (square.x, square.y);
		float centerToWall = GameObject.Find ("GM").GetComponent<MakeWalls> ().Wall.transform.localScale.x / 2f;
		pos += new Vector3 (Random.Range (-centerToWall, centerToWall), Random.Range (-centerToWall, centerToWall), 0f);
		return GameObject.Instantiate (item, pos,Quaternion.identity,parent);
	}

	//spawns item somewhere within the confines of the room
	//specified rotation
	public GameObject SpawnItem (GameObject item, Quaternion rot, Transform parent = null){
		int randomSquareIndex = Random.Range (0, squares.Count);
		Vector2Int square = squares [randomSquareIndex];
		Vector3 pos = GameObject.Find ("GM").GetComponent<MakeWalls> ().GetCellPositionFor (square.x, square.y);
		float centerToWall = GameObject.Find ("GM").GetComponent<MakeWalls> ().Wall.transform.localScale.x / 2f;
		pos += new Vector3 (Random.Range (-centerToWall, centerToWall), Random.Range (-centerToWall, centerToWall), 0f);
		return GameObject.Instantiate (item, pos,rot,parent);
	}

	//spawns item at the center of the room (average of square positions)
	//will be used for rare/unique items for dramatic effect
	public GameObject SpawnItemAtCenter(GameObject item, Transform parent = null){
		Vector3 weightedAvg = new Vector3 (0,0,0);
		foreach (Vector2Int square in squares) {
			weightedAvg += GameObject.Find ("GM").GetComponent<MakeWalls> ().GetCellPositionFor (square.x, square.y);
		}
		weightedAvg *= (1f / squares.Count);
		return GameObject.Instantiate (item, weightedAvg,Quaternion.identity,parent);
	}
}
public class MakeWalls : MonoBehaviour {
	//back end - grid of rooms
	public bool coop;
	public int width;
	private int numSquares;
	private int numCheckedOff;
	private int roomNum;
	[HideInInspector]
	public int[,] rooms;
	Dictionary<int,Room> roomGraph;

	//front end - generated game objects
	private const float epsilon = 0.005f; //makes borders between walls/corners look better
	private const float smallWallOffset = 0.32f;
	private float cellSize;
	private Vector3 basePosition;
	private Quaternion vertical;
	public GameObject DoorContainer;
	public GameObject WallContainer;
	public GameObject TileContainer;
	public GameObject VoidContainer;//container for fog of war objects
	public GameObject[,] VoidArray;//grid of fog of war objects
	public GameObject Wall;
	public GameObject Corner;
	public GameObject Door;
	public GameObject WallSmall;
	public GameObject Tile;
	public GameObject Void;//fog of war objects

	// Use this for initialization
	void Awake(){
		Destroy (GetComponent<SpriteRenderer> ());
		cellSize = Wall.transform.localScale.x + Corner.transform.localScale.x - epsilon;
		print ("cellSize: " + cellSize);
		basePosition = new Vector3 (-(width / 2) * cellSize, (width / 2) * cellSize,0f);
		vertical = Quaternion.Euler (0, 0, 90);
		VoidArray = new GameObject[width, width];

		FillRooms ();
		FillRoomGraph ();
		GenerateWalls ();
		PlaceFogOfWar ();
		MakeLoot ();
		print ("level Score: " + GetComponent<Words> ().levelScore);
	}
	//BACK END
	void FillRooms(){
		rooms = new int[width, width];
		roomNum = 1;
		numCheckedOff = 0;
		numSquares = width * width;
		while (numCheckedOff < numSquares) {
			MakeRoom (Random.Range(0,width),Random.Range(0,width),Random.Range(4,7),Random.Range(4,7));
		}
		//Starting Room
		int halfX = width/2;
		if (coop) {
			MakeRoom (halfX, halfX, 3, 3,-1);
			return;
		}
		int num = 6;//how far are player starting rooms from the center?
		MakeRoom(halfX,halfX,7,7,-5);//Boss Chamber
		MakeRoom(halfX - num, halfX - num,3,3,-1);//P1 start
		MakeRoom(halfX + num, halfX - num,3,3,-2);//P2 start
		MakeRoom(halfX - num, halfX + num,3,3,-3);//P3 start
		MakeRoom(halfX + num, halfX + num,3,3,-4);//P4 start
	}

	//is this coordinate in bounds?
	bool InBounds(int x, int y){
		return (x >= 0 && x < width && y >= 0 && y < width);
	}

	//makes a room of width w and height h centered at [x,y]
	void MakeRoom(int x, int y, int w, int h){
		for (int i = -(w / 2); i <= w / 2; i++) {
			for (int j = -(h / 2); j <= h / 2; j++) {
				int a = i + x;
				int b = j + y;
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

	//makes a room of width w and height h centered at [x,y] with room designation num
	//for special rooms
	void MakeRoom(int x, int y, int w, int h, int num){
		for (int i = -(w / 2); i <= w / 2; i++) {
			for (int j = -(h / 2); j <= h / 2; j++) {
				int a = i + x;
				int b = j + y;
				if (InBounds (a, b)) {
					if (rooms [a, b] == 0) {
						numCheckedOff++;
					}
					rooms [a, b] = num;
				}
			}
		}
	}

	//C# mod is not too useful
	int correctmod(int a,int n){
		return ((a % n) + n) % n;
	}

	//test if back end is giving the right output by mapping room nums to ascii chars and filling a string with that
	//makes it absurdly slow for large sizes because Unity's print buffer is expecting a whole bunch of small messages rather than a single large message
	void PrintRooms(){
		string result = "";
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < width; j++) {
				result += (char)(correctmod (rooms [j, i], 94) + 32) + " ";
			}
			result += "\n";
		}
		print (result);
	}

	//analyze room grid and convert it into an adjacency list of room objects
	void FillRoomGraph(){
		roomGraph = new Dictionary<int, Room> ();
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
				int roomnum = rooms [x, y];
				if (!roomGraph.ContainsKey(roomnum)) {
					roomGraph.Add (roomnum, new Room ());
				}
				roomGraph [roomnum].squares.Add (new Vector2Int (x, y));
			}
		}
		foreach (Room room in roomGraph.Values) {
			foreach (Vector2Int square in room.squares) {
				int numDissimilar = 0;
				foreach (Vector2Int neighbor in GetNeighbors(square)) {
					if (rooms [neighbor.x,neighbor.y] != rooms [square.x,square.y]) {
						numDissimilar++;
						Room otherGuy = roomGraph [rooms [neighbor.x,neighbor.y]];
						if (!room.neighbors.Contains(otherGuy)) {
							otherGuy.neighbors.Add (room);
							room.neighbors.Add (otherGuy);
						}
					}
				}
			}
		}
	}

	//FRONT END
	//runs BFS starting from player starting rooms.
	void GenerateWalls(){
		print ("makin dungeon walls");
		Queue<Room> q1 = new Queue<Room> ();//all rooms at the current layer of depth
		Queue<Room> q2 = new Queue<Room> ();//all rooms at the next layer of depth

		//start with player starting rooms
		q1.Enqueue (roomGraph [-1]);
		if (!coop) {
			q1.Enqueue (roomGraph [-2]);
			q1.Enqueue (roomGraph [-3]);
			q1.Enqueue (roomGraph [-4]);
		}

		//Make the rest of the stuff
		int depth = 1;
		for (int i = 0; i < 10; i++) {
			while (q1.Count > 0) {
				Room a = q1.Dequeue ();
				a.reached = true;
				foreach (Room neighbor in a.neighbors) {
					int w = DoorWeightFor (depth);
					if (neighbor.squares.Count < 3) {
						MakeBorderBetween (a, neighbor, false);
					} else {
						if (!neighbor.reached) {
							q2.Enqueue (neighbor);
							neighbor.reached = true;
							neighbor.neighbors.Remove (a);
							MakeBorderBetween (a, neighbor, true, w);
						} else {
							MakeBorderBetween (a, neighbor, false,w,doorChance:0.05f);
						}
					}
				}
			}
			q1 = new Queue<Room> (q2);
			q2.Clear ();
			depth++;
		}

		//make corners
		for (int x = -1; x < width; x++) {
			for (int y = -1; y < width; y++) {
				if (ThereShouldBeABottomRightCornerAt (x,y)) {
					PlaceBottomRightCornerAt(x,y);
				}
			}
		}

		//make walls at edge of floor
		for (int i = 0; i < width; i++) {
			PlaceRightWallAt (-1, i);
			PlaceRightWallAt (width-1, i);
		}
		for (int i = 0; i < width; i++) {
			PlaceBottomWallAt (i,-1);
			PlaceBottomWallAt (i,width-1);
		}
	}

	int DoorBaseWeightFor(int depth){
		Words w = GetComponent<Words> ();
		int averageMaxDepth = 7;
		if (depth == 1) {
			return 2;
		}
		int deepestAmount = (int)(w.levelScore * w.humanKnowledgeFactor);//scale according to how good the level is
		//		float howDeepAmI = (float)(depth * depth) / (averageMaxDepth * averageMaxDepth);//scale quadratically with depth
		float howDeepAmI = (float)(depth) / (averageMaxDepth);//scale linearly with depth
		return (int)(deepestAmount*howDeepAmI);
	}

	int DoorWeightFor (int depth){
		int variance = Random.Range (-(depth - 1), depth + 1); //add a pinch of salt
//		print ("depth: " + depth +" fancy number: " + baseWeight);
		return variance + DoorBaseWeightFor(depth);
	}
		
	void MakeBorderBetween(Room a, Room b, bool door, int weight = 1, float doorChance = 0f){
		//Find border
		List<Vector2Int> rightBorderSquares = new List<Vector2Int> ();//squares immediately to the left of the border (so, if you placed a wall to the right it would be the border)
		List<Vector2Int> bottomBorderSquares = new List<Vector2Int> ();//squares immediately above the border (so, if you placed a wall on the bottom it would be the border)
		foreach (Vector2Int square in a.squares) {
			Vector2Int toTheRight = new Vector2Int (square.x + 1, square.y);
			if (b.squares.Contains (toTheRight)) {
				rightBorderSquares.Add (square);
			}
			Vector2Int toTheBottom = new Vector2Int (square.x, square.y + 1);
			if (b.squares.Contains (toTheBottom)) {
				bottomBorderSquares.Add (square);
			}
		}
		foreach (Vector2Int square in b.squares) {
			Vector2Int toTheRight = new Vector2Int (square.x + 1, square.y);
			if (a.squares.Contains (toTheRight)) {
				rightBorderSquares.Add (square);
			}
			Vector2Int toTheBottom = new Vector2Int (square.x, square.y + 1);
			if (a.squares.Contains (toTheBottom)) {
				bottomBorderSquares.Add (square);
			}
		}

		if (!door) {
			//Put walls everywhere on the border
			foreach (Vector2Int square in rightBorderSquares) {
				PlaceRightWallAndMaybeDoorAt (square.x, square.y, doorChance, weight);
			}
			foreach (Vector2Int square in bottomBorderSquares) {
				PlaceBottomWallAndMaybeDoorAt (square.x, square.y, doorChance, weight);
			}
			return;
		}
		//Pick a random square on the border, make a door there
		int randomIndex = Random.Range (0, rightBorderSquares.Count + bottomBorderSquares.Count);
		Vector2Int selectedBorder = new Vector2Int ();
		if (randomIndex >= rightBorderSquares.Count) {
			randomIndex -= rightBorderSquares.Count;
			selectedBorder = bottomBorderSquares [randomIndex];
			PlaceBottomDoorAt (selectedBorder.x, selectedBorder.y,weight);
			bottomBorderSquares.RemoveAt (randomIndex);
		} else {
			selectedBorder = rightBorderSquares [randomIndex];
			PlaceRightDoorAt (selectedBorder.x, selectedBorder.y,weight);
			rightBorderSquares.RemoveAt (randomIndex);
		}
		//Put walls everywhere else on the border
		foreach (Vector2Int square in rightBorderSquares) {
			PlaceRightWallAndMaybeDoorAt (square.x, square.y, doorChance, weight);
		}
		foreach (Vector2Int square in bottomBorderSquares) {
			PlaceBottomWallAndMaybeDoorAt (square.x, square.y, doorChance, weight);
		}
	}

	List<Vector2Int> GetNeighbors(Vector2Int xy){
		int x = xy.x;
		int y = xy.y;
		List<Vector2Int> result = new List<Vector2Int> ();
		if (InBounds (x + 1, y)) {
			result.Add (new Vector2Int (x + 1, y));
		}
		if (InBounds (x, y + 1)) {
			result.Add (new Vector2Int (x, y + 1));
		}
		return result;
	}

	void PlaceFogOfWar(){
		print ("placing fog of war objects");
		for (int x = -3; x < width+3; x++) {
			for (int y = -3; y < width+3; y++) {
				PlaceFogOfWarAt (x, y);
			}
		}
	}

	bool ThereShouldBeARightWallAt(int x, int y){
		if (!InBounds (x, y) && !InBounds (x + 1, y)) {
			return false;
		}
		if (!InBounds (x, y)) {
			return true;
		}
		if (!InBounds(x+1,y)){
			return true;
		}
		if (rooms [x, y] != rooms [x + 1, y]) {
			return true;
		}
		return false;
	}
	bool ThereShouldBeABottomWallAt(int x, int y){
		if (!InBounds (x, y) && !InBounds (x, y+1)) {
			return false;
		}
		if (!InBounds (x, y)) {
			return true;
		}
		if (!InBounds(x,y+1)) {
			return true;
		}
		if (rooms [x, y] != rooms [x, y + 1]) {
			return true;
		}
		return false;
	}
	bool ThereShouldBeABottomRightCornerAt(int x, int y){
		if (!InBounds (x, y)) {
			return true;
		}
		if(!InBounds(x+1,y+1)){
			return true;
		}
		if (rooms [x, y] != rooms [x + 1, y] || rooms [x, y] != rooms [x, y + 1] || rooms [x, y] != rooms [x + 1, y + 1]) {
			return true;
		}
		return false;
	}

	public Vector3 GetCellPositionFor(int x, int y){
		return new Vector3 (basePosition.x + x * cellSize, basePosition.y - y * cellSize,basePosition.z);
	}
	GameObject PlaceRightDoorAt(int x, int y, int keyNum){
		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
		GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f,0f, 0f), vertical, DoorContainer.transform);
		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, -cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
		newDoor.GetComponent<Door> ().keyNum = keyNum;
		return newDoor;
	}
	GameObject PlaceBottomDoorAt(int x, int y, int keyNum){
		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
		GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (0f,-cellSize * 0.5f, 0f), Quaternion.identity, DoorContainer.transform);
		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (-cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
		newDoor.GetComponent<Door> ().keyNum = keyNum;
		return newDoor;
	}
	GameObject PlaceRightWallAt(int x, int y){
		return GameObject.Instantiate(Wall,GetCellPositionFor(x,y) + new Vector3(cellSize*0.5f,0f,0f),vertical, WallContainer.transform);
	}
	GameObject PlaceBottomWallAt(int x, int y){
		return GameObject.Instantiate(Wall,GetCellPositionFor(x,y) + new Vector3(0f,-cellSize*0.5f,0f),Quaternion.identity,WallContainer.transform);
	}
	GameObject PlaceRightWallAndMaybeDoorAt(int x, int y,float doorChance, int weight){
		float randy = Random.value;
		if (randy <= doorChance) {
			return PlaceRightDoorAt (x, y, weight);
		}
		return PlaceRightWallAt (x, y);
	}
	GameObject PlaceBottomWallAndMaybeDoorAt(int x, int y,float doorChance, int weight){
		float randy = Random.value;
		if (randy <= doorChance) {
			return PlaceBottomDoorAt (x, y, weight);
		}
		return PlaceBottomWallAt (x, y);
	}
	void PlaceBottomRightCornerAt(int x, int y){
		GameObject.Instantiate(Corner,GetCellPositionFor(x,y) + new Vector3(cellSize*0.5f,-cellSize*0.5f,0f),Quaternion.identity,WallContainer.transform);
	}

	void PlaceFogOfWarAt(int x, int y){
		if (!InBounds (x, y)) {
			GameObject.Instantiate(Void,GetCellPositionFor(x,y),Quaternion.identity,VoidContainer.transform);
			return;
		}
		GameObject newFog = GameObject.Instantiate(Void,GetCellPositionFor(x,y),Quaternion.identity,VoidContainer.transform);
		VoidArray [x, y] = newFog;
		if (!ThereShouldBeARightWallAt (x - 1, y)) {
			newFog.GetComponent<FogOfWar> ().neighbors.Add (VoidArray [x - 1, y]);
			VoidArray [x-1, y].GetComponent<FogOfWar> ().neighbors.Add (newFog);
		}
		if (!ThereShouldBeABottomWallAt (x, y-1)) {
			newFog.GetComponent<FogOfWar> ().neighbors.Add (VoidArray [x, y-1]);
			VoidArray [x, y-1].GetComponent<FogOfWar> ().neighbors.Add (newFog);
		}
	}

	void MakeLoot(){
		print ("making some sweet loot");
		Words w = GetComponent<Words> ();
		for (int i = 0; i < (width*width)/16; i++) {
			GameObject newTile = GameObject.Instantiate (Tile, Random.insideUnitCircle * cellSize*width/2, Quaternion.Euler (0, 0, Random.Range (-30f, 30f)), TileContainer.transform);
			newTile.GetComponent<LetterTile> ().SetLetter (w.GetRandomSourceChar());
			newTile.GetComponent<LetterTile> ().SetMatches (Random.Range (3, 9));
		}
		PlaceGoodTilesInRoom (-1);
		if (!coop) {
			PlaceGoodTilesInRoom (-2);
			PlaceGoodTilesInRoom (-3);
			PlaceGoodTilesInRoom (-4);
			PlaceGoodTilesInRoom (-5);
		}
	}

	void PlaceGoodTilesInRoom(int roomNum){
		if (!roomGraph.ContainsKey (roomNum)) {
			return;
		}
		Words w = GetComponent<Words> ();
		Room r = roomGraph [roomNum];
		string startingTileSet = "";
		do{
			startingTileSet= "";
			for(int i = 0; i < 8; i++){
				startingTileSet += w.GetRandomSourceChar();
			}
		} while (w.GetScoreExact(startingTileSet) < DoorBaseWeightFor(2));
		char[] startingTiles = startingTileSet.ToCharArray ();
		for (int i = 0; i < 8; i++) {
			GameObject newTile = r.SpawnItem (Tile, Quaternion.Euler (0, 0, Random.Range (-30f, 30f)), TileContainer.transform);
			newTile.GetComponent<LetterTile> ().SetLetter (startingTiles [i]);
			newTile.GetComponent<LetterTile> ().SetMatches (Random.Range (3, 9));
		}
	}
}
