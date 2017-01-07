using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class EnemyManager : MonoBehaviour
{
    public GameObject AiPrefab;
    public GameObject Football;
	// Use this for initialization
    private GameObject[] _aiList;
    private Vector3[] _defaultPositions;
    private float[] _distanceToBall;
    private void Start ()
    {
        _aiList=new GameObject[transform.childCount];
        var t = 0;
        foreach (Transform child in transform)
        {

            _aiList[t] = child.gameObject;
            t++;
        }
        _distanceToBall=new float[_aiList.Length];
        _defaultPositions =new Vector3[_aiList.Length];
        
        for (var i = 0; i < _aiList.Length; ++i)
        {
            _defaultPositions[i] = _aiList[i].transform.position;
            _distanceToBall[i] = (_aiList[i].transform.position - Football.transform.position).magnitude;
        }
    }
	
	// Update is called once per frame
    private void Update () {
        for (var i = 0; i < _aiList.Length; ++i)
        {
            _distanceToBall[i] = (_aiList[i].transform.position - Football.transform.position).magnitude;
        }
        float min = _distanceToBall.Min();
        var index = _distanceToBall.ToList().IndexOf(min);

        for (var i = 0; i < _aiList.Length; ++i)
        {
            var s = _aiList[i].GetComponent("AI") as AI;
            if (i == index)
            {
                Debug.Assert(s != null, "s != null");
                s.isActive = true;
            }
            else
            {
                Debug.Assert(s != null, "s != null");
                s.isActive = false;
            }

        }
    }
}
