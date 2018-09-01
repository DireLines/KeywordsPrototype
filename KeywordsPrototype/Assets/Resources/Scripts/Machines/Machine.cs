using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour {
	[HideInInspector]
	public GameObject slot;
	private GameObject progressIndicator;
	public float chargeTime;
	private float timer;
	public bool ticking;

	// Use this for initialization
	public virtual void Start () {
		ticking = false;
		slot = transform.GetChild (0).gameObject;
		progressIndicator = transform.GetChild (1).gameObject;
		timer = 0;
	}

	public virtual void Update(){
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
			PerformMachineAction();
		}
	}

	public virtual void PerformMachineAction(){
		print ("Im a machine doin a thing");
	}
	
}
