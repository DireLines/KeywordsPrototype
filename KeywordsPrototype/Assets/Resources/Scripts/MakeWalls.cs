using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GraphNode {
	public int value;
	public List<GraphNode> neighbors;
}
public class MakeWalls : MonoBehaviour {
//	public MakeWalls(int w, int h){
//		width = w;
//		height = h;
//	}
	//back end - grid of rooms
	public int width;
	public int height;
	private int numSquares;
	private int numCheckedOff;
	private int roomNum;
	[HideInInspector]
	public int[,] rooms;

	//translation - variables for turning grid into gameobjects
	List<Vector2Int> rightDoorSquares;
	List<Vector2Int> bottomDoorSquares;
	List<GraphNode> borderGraph;

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
		basePosition = new Vector3 (-(width / 2) * cellSize, (height / 2) * cellSize,0f);
		vertical = Quaternion.Euler (0, 0, 90);

		FillRooms ();
		GenerateWalls ();
		PlaceFogOfWar ();
		MakeLoot ();
	}

	public void MakeFloor(){
		//clear out old stuff
		foreach (Transform child in DoorContainer.transform) {
			Destroy (child.gameObject);
		}
		foreach (Transform child in WallContainer.transform) {
			Destroy (child.gameObject);
		}
		foreach (Transform child in TileContainer.transform) {
			Destroy (child.gameObject);
		}

		//make new stuff
		FillRooms ();
		//PrintRooms();
		GenerateWalls ();	
		PlaceFogOfWar ();
		MakeLoot ();
	}

	//BACK END
	void FillRooms(){
		rooms = new int[width, height];
		roomNum = 1;
		numCheckedOff = 0;
		numSquares = width * height;
		while (numCheckedOff < numSquares) {
			MakeRoom (Random.Range(0,width),Random.Range(0,width),Random.Range(4,7),Random.Range(4,7));
		}
		//Starting Room
		MakeRoom(width/2,height/2,3,3);

	}

	//is this coordinate in bounds?
	bool InBounds(int x, int y){
		return (x >= 0 && x < width && y >= 0 && y < height);
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
		print ("makin dungeon walls");
		VoidArray = new GameObject[width, height];
		for (int x = -1; x < width; x++) {
			for (int y = -1; y < height; y++) {
				if (ThereShouldBeARightWallAt(x,y)) {
					if (!InBounds(x,y) || !InBounds (x + 1, y)) {
						PlaceRightWallAt (x, y, 0f);
					} else {
						PlaceRightWallAt (x, y, 0.2f);
					}
				}
				if (ThereShouldBeABottomWallAt(x,y)) {
					if (!InBounds(x,y) || !InBounds (x, y+1)) {
						PlaceBottomWallAt (x, y, 0f);
					} else {
						PlaceBottomWallAt (x, y, 0.2f);
					}
				}
				if (ThereShouldBeABottomRightCornerAt (x,y)) {
					PlaceBottomRightCornerAt(x,y);
				}
			}
		}
	}

	void PlaceFogOfWar(){
		print ("placing fog of war objects");
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
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

	Vector3 GetCellPositionFor(int x, int y){
		return new Vector3 (basePosition.x + x * cellSize, basePosition.y - y * cellSize,basePosition.z);
	}
	void PlaceRightWallAt(int x, int y, float doorChance){
		float randy = Random.value;
		if (randy < doorChance) {
			GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
			GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f,0f, 0f), vertical, DoorContainer.transform);
			GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, -cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
			newDoor.GetComponent<Door> ().keyNum = (int)((Vector2)newDoor.transform.position).magnitude;
			return;
		}
		GameObject.Instantiate(Wall,GetCellPositionFor(x,y) + new Vector3(cellSize*0.5f,0f,0f),vertical, WallContainer.transform);
	}
	void PlaceBottomWallAt(int x, int y, float doorChance){
		float randy = Random.value;
		if (randy < doorChance) {
			GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
			GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (0f,-cellSize * 0.5f, 0f), Quaternion.identity, DoorContainer.transform);
			GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (-cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
			newDoor.GetComponent<Door> ().keyNum = (int)(((Vector2)newDoor.transform.position).magnitude);
			return;
		}
		GameObject.Instantiate(Wall,GetCellPositionFor(x,y) + new Vector3(0f,-cellSize*0.5f,0f),Quaternion.identity,WallContainer.transform);
	}
//	void PlaceRightDoorAt(int x, int y){
//		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
//		GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f,0f, 0f), vertical, DoorContainer.transform);
//		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * 0.5f, -cellSize * smallWallOffset, 0f), vertical, WallContainer.transform);
//		newDoor.GetComponent<Door> ().keyNum = (int)((Vector2)newDoor.transform.position).magnitude;
//	}
//	void PlaceBottomDoorAt(int x, int y){
//		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
//		GameObject newDoor = GameObject.Instantiate (Door, GetCellPositionFor (x, y) + new Vector3 (0f,-cellSize * 0.5f, 0f), Quaternion.identity, DoorContainer.transform);
//		GameObject.Instantiate (WallSmall, GetCellPositionFor (x, y) + new Vector3 (-cellSize * smallWallOffset, -cellSize * 0.5f, 0f), Quaternion.identity, WallContainer.transform);
//		newDoor.GetComponent<Door> ().keyNum = (int)((Vector2)newDoor.transform.position).magnitude;
//	}
//	void PlaceRightWallAt(int x, int y){
//		GameObject.Instantiate(VerticalWall,GetCellPositionFor(x,y) + new Vector3(cellSize*0.5f,0f,0f),vertical, WallContainer.transform);
//	}
//	void PlaceBottomWallAt(int x, int y){
//		GameObject.Instantiate(HorizontalWall,GetCellPositionFor(x,y) + new Vector3(0f,-cellSize*0.5f,0f),Quaternion.identity,WallContainer.transform);
//	}
	void PlaceBottomRightCornerAt(int x, int y){
		GameObject.Instantiate(Corner,GetCellPositionFor(x,y) + new Vector3(cellSize*0.5f,-cellSize*0.5f,0f),Quaternion.identity,WallContainer.transform);
	}

	void PlaceFogOfWarAt(int x, int y){
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
		for (int i = 0; i < (width*height)/16; i++) {
			GameObject.Instantiate (Tile, Random.insideUnitCircle * cellSize*width/2, Quaternion.Euler (0, 0, Random.Range (-30f, 30f)), TileContainer.transform);
		}
	}
}
