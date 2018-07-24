﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

	public int playerNum;

	public KeyCode GetKeyCode(string controlName){
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
			if (playerNum > 0 && playerNum < 5) {
				return Input.GetAxis ("P" + playerNum + "_Horizontal");
			} else {
				print ("playerNum not a valid number fix it");
				return 0f;
			}
		} else if (axisName == "Vertical") { 
			if (playerNum > 0 && playerNum < 5) {
				return Input.GetAxis ("P" + playerNum + "_Vertical");
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
