using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opponent : MonoBehaviour {

//	public enum Player_State { 
//		RESTING, //This is when the keeper is idle
//		RUNNING,
//		RETREATING,
//		PASSING,
//		SHOT
//	};

	public AI.Status state;

	private AI ai;

	// Use this for initialization
	void Start () {
//		state = Player_State.RESTING;
		ai = gameObject.GetComponent("AI") as AI;
		state = ai.status;
	}

	// Update is called once per frame
	void FixedUpdate () {


		//float speed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
		//Debug.Log (string.Format ("speed:{0}",gameObject.GetComponent<Rigidbody> ().velocity));
//		if (Input.GetButton ("Vertical")) {
//			if (Input.GetAxis("Vertical")>0) {
//				if (state != Player_State.RUNNING) {
//					GetComponent<Animation> ().Stop ();
//					state = Player_State.RUNNING;
//				}
//
//			} 
//			if(Input.GetAxis("Vertical")<0){
//				if (state != Player_State.RETREATING) {
//					GetComponent<Animation> ().Stop ();
//					state = Player_State.RETREATING;
//				}
//
//			}
//		}
//		else {
//			float speed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
//			if (speed < 2.0f) {
//				state = Player_State.RESTING;
//				if (state == Player_State.RUNNING || state == Player_State.RETREATING) {
//					GetComponent<Animation> ().Stop ();
//				}
//			}
//		}

		state = ai.status;
      

		float speed = gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
//	    if (speed < 0.01)
//	    {
//            if (!GetComponent<Animation>().IsPlaying("Idle"))
//                GetComponent<Animation>().Play("Idle");
//	        return;
//	    }
		if (!GetComponent<Animation> ().IsPlaying ("tiro") && !GetComponent<Animation> ().IsPlaying ("pass")) {
			switch (state) {
			case AI.Status.Idle:
				if (!GetComponent<Animation> ().IsPlaying ("defenderIdle")) {
					GetComponent<Animation> ().Play ("defenderIdle");
				}
				break;
			case AI.Status.Attack:
				if (!GetComponent<Animation> ().IsPlaying ("run")) {
					//GetComponent<Animation>()["run"].speed = speed*0.3f;
					GetComponent<Animation> ().Play ("run");
				}
				break;
			case AI.Status.Assist:
				if (!GetComponent<Animation> ().IsPlaying ("run")) {
					//GetComponent<Animation>()["run"].speed = speed*0.3f;
					GetComponent<Animation> ().Play ("run");
				}
				break;
			case AI.Status.Return:
				if (!GetComponent<Animation>().IsPlaying("run"))
				{
					//GetComponent<Animation>()["run"].speed = speed*0.3f;
					GetComponent<Animation>().Play("run");
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

	}
}
