using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBorder : MonoBehaviour
{
    public float MaxX;
    public float MaxY;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var t = gameObject.transform.position;
	    if (Math.Abs(t[0]) > MaxX || Math.Abs(t[1]) > MaxY)
	    {
	        GameManager.gm.OffBorder = true;
	    }

	}
}
