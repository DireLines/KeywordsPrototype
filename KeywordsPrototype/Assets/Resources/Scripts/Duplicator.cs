using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour {
	private GameObject slot;
	private GameObject progressIndicator;
	private Vector3 placePosition;
	public GameObject Tile;
	private Transform TileContainer;
	public float chargeTime;
	private float timer;
	public bool ticking;

	// Use this for initialization
	void Start () {
		ticking = false;
		placePosition = new Vector3 (0.5f, 0, 0);
		slot = transform.GetChild (0).gameObject;
		progressIndicator = transform.GetChild (1).gameObject;
		TileContainer = GameObject.Find ("Tiles").transform;
		timer = 0;
	}

	void Update(){
		if (slot.GetComponent<GridSquare> ().tile != null) {
			ticking = true;
		} else {
			ticking = false;
			timer = 0f;
			progressIndicator.GetComponent<SpriteRenderer> ().color = Color.black;
		}
		if (ticking) {
			timer += Time.deltaTime;
			float frac = 0.7f*(timer / chargeTime)+0.3f;
			progressIndicator.GetComponent<SpriteRenderer> ().color = new Color (frac,frac,frac, 1f);
		}
		if (timer >= chargeTime) {
			timer = 0f;
			Duplicate ();
		}
	}
	void Duplicate(){
		GameObject tile = slot.GetComponent<GridSquare>().tile;
		Vector3 pos = transform.position + placePosition;
		GameObject newTile = GameObject.Instantiate (Tile, pos, Quaternion.identity, TileContainer);
		newTile.GetComponent<LetterTile> ().SetLetter (tile.GetComponent<LetterTile> ().letter);
		newTile.GetComponent<LetterTile> ().SetMatches (tile.GetComponent<LetterTile> ().matches);
	}
}
