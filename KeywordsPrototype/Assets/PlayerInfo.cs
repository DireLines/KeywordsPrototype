using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

	public int playerNum;

	public KeyCode GetControl(string controlName){
		KeyCode[] controlSet = new KeyCode[4];
		if (controlName == "A") {
			controlSet = new KeyCode[4] {
				KeyCode.Joystick1Button16,
				KeyCode.Joystick2Button16,
				KeyCode.Joystick3Button16,
				KeyCode.Joystick4Button16
			};
		} else if (controlName == "B") {
			controlSet = new KeyCode[4] {
				KeyCode.Joystick1Button17,
				KeyCode.Joystick2Button17,
				KeyCode.Joystick3Button17,
				KeyCode.Joystick4Button17
			};
		} else if (controlName == "LeftBumper") {
			controlSet = new KeyCode[4] {
				KeyCode.Joystick1Button13,
				KeyCode.Joystick2Button13,
				KeyCode.Joystick3Button13,
				KeyCode.Joystick4Button13
			};
		} else if (controlName == "RightBumper") {
			controlSet = new KeyCode[4] {
				KeyCode.Joystick1Button14,
				KeyCode.Joystick2Button14,
				KeyCode.Joystick3Button14,
				KeyCode.Joystick4Button14
			};
		} else {
			print ("control name not recognized");
		}
		return controlSet [playerNum-1];
	}

	public float GetAxis(string axisName){
		if (axisName == "Horizontal") {
			return Input.GetAxis ("Horizontal");
			if (playerNum == 1) {
				return Input.GetAxis ("P1_Horizontal");
			} else if (playerNum == 2) {
				return Input.GetAxis ("P2_Horizontal");
			} else if (playerNum == 3) {
				return Input.GetAxis ("P3_Horizontal");
			} else if (playerNum == 4) {
				return Input.GetAxis ("P4_Horizontal");
			} else {
				print ("playerNum not a valid number fix it");
				return 0f;
			}
		} else if (axisName == "Vertical") {
			return Input.GetAxis ("Vertical");
			if (playerNum == 1) {
				return Input.GetAxis ("P1_Vertical");
			} else if (playerNum == 2) {
				return Input.GetAxis ("P2_Vertical");
			} else if (playerNum == 3) {
				return Input.GetAxis ("P3_Vertical");
			} else if (playerNum == 4) {
				return Input.GetAxis ("P4_Vertical");
			} else {
				print ("playerNum not a valid number fix it");
				return 0f;
			}
		} else {
			print ("axis name not recognized");
			return 0f;
		}
	}
}
