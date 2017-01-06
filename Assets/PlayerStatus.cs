using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private GameManager.GameStatus status;
    private MonoBehaviour script;
	// Use this for initialization
	void Start ()
	{
	    status = GameManager.gm.status;
	    script = gameObject.GetComponent("Rigidbody First Person Controller") as MonoBehaviour;
	}
	
	// Update is called once per frame
	void Update () {
	    if (status != GameManager.gm.status)
	    {
	        status = GameManager.gm.status;
	        switch (status)
	        {
	            case GameManager.GameStatus.Running:
	                script.enabled = true;
	                break;
	            case GameManager.GameStatus.OffBorder:
                    script.enabled = false;
                    break;
	            case GameManager.GameStatus.Goal:
                    script.enabled = false;
                    break;
	            case GameManager.GameStatus.Over:
                    script.enabled = false;
                    break;
	            case GameManager.GameStatus.ToStart:
                    script.enabled = false;
                    break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }
	}
}
