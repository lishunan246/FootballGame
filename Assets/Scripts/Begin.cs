using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Return))
	    {
	        Application.LoadLevel("Level1");
	    }
	}
}
