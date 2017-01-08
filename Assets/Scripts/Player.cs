using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public enum Player_State { 
		RESTING, //This is when the keeper is idle
		RUNNING,
		RETREATING,
		PASSING,
		SHOT
	};

	public Player_State state;

	// Use this for initialization
	void Start () {
		state = Player_State.RESTING;
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		//float speed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
		//Debug.Log (string.Format ("speed:{0}",gameObject.GetComponent<Rigidbody> ().velocity));
		if (Input.GetButton ("Vertical")) {
			if (Input.GetAxis("Vertical")>0) {
				if (state != Player_State.RUNNING) {
//					GetComponent<Animation> ().Stop ();
					state = Player_State.RUNNING;
				}

			} 
			if(Input.GetAxis("Vertical")<0){
				if (state != Player_State.RETREATING) {
//					GetComponent<Animation> ().Stop ();
					state = Player_State.RETREATING;
				}

			}
		}
		else {
			float speed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
			if (speed < 2.0f) {
				state = Player_State.RESTING;
				if (state == Player_State.RUNNING || state == Player_State.RETREATING) {
//					GetComponent<Animation> ().Stop ();
				}
			}
		}
		switch (state) {
		case Player_State.RESTING:
			if (!GetComponent<Animation> ().IsPlaying ("defenderIdle")) {
				GetComponent<Animation> ().Play ("defenderIdle");
			}
			break;
		case Player_State.RUNNING:
			if (!GetComponent<Animation> ().IsPlaying ("run")) {
				//GetComponent<Animation>()["run"].speed = speed*0.3f;
				GetComponent<Animation> ().Play ("run");
			}
			break;
		case Player_State.RETREATING:
			if (!GetComponent<Animation> ().IsPlaying ("pasos_atras")) {
				//GetComponent<Animation>()["run"].speed = speed*0.3f;
				GetComponent<Animation> ().Play ("pasos_atras");
			}
			break;
		}
	}
}
