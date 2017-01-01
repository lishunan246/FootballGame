using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    Debug.Log("start");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "Football")
        {
            GameManager.gm.OffBorder = true;
        }
    }
}
