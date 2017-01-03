using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour {
   public enum Side
    {
        Player,
        Computer
    }

    public Side side;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        var obj = col.gameObject;
        if (obj.tag == "Football")
        {
            if (side == Side.Computer)
            {
                GameManager.gm.PlayerScore++;
            }
            else if(side==Side.Player)
            {
                GameManager.gm.ComputerScore++;
            }
        }
    }
}
