using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	private Rigidbody2D rb;
	private PlayerInfo me;
	private KeyCode[] allAButtons;
	// Use this for initialization
	void Start () {
		allAButtons = new KeyCode[8] {
			KeyCode.Joystick1Button16,
			KeyCode.Joystick2Button16,
			KeyCode.Joystick3Button16,
			KeyCode.Joystick4Button16,
			KeyCode.Joystick5Button16,
			KeyCode.Joystick6Button16,
			KeyCode.Joystick7Button16,
			KeyCode.Joystick8Button16
		};
		rb = GetComponent<Rigidbody2D> ();
		rb.freezeRotation = true;
		me = GetComponent<PlayerInfo> ();
//		foreach (string stick in Input.GetJoystickNames()) {
//			print (stick);
//		}
	}
	
	// Update is called once per frame
	void Update () {
		rb.velocity = new Vector2 (me.GetAxis ("Horizontal"), me.GetAxis ("Vertical"));
//		for (int i = 0; i < 8; i++) {
//			if (Input.GetKeyDown (allAButtons [i])) {
//				print (i + 1);
//			}
//		}
	}
}
