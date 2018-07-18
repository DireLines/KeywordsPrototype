using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	private Rigidbody2D rb;
	private PlayerInfo me;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.freezeRotation = true;
		me = GetComponent<PlayerInfo> ();
	}

	// Update is called once per frame
	void Update () {
//		Vector2 original = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		rb.velocity = new Vector2 (me.GetAxis ("Horizontal"), me.GetAxis ("Vertical"));
	}
}
