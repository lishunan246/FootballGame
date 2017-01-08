using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class EnemyManager : MonoBehaviour
{
    // Use this for initialization
    private GameObject[] _aiList;
    private Vector3[] _defaultPositions;
    private float[] _distanceToBall;
    public GameObject AiPrefab;
    public GameObject Football;

    public float period = 2.0f;
    private float remain = 2.0f;
    private void Start()
    {
        _aiList = new GameObject[transform.childCount];
        var t = 0;
        foreach (Transform child in transform)
        {
            _aiList[t] = child.gameObject;
            t++;
        }
        _distanceToBall = new float[_aiList.Length];
        _defaultPositions = new Vector3[_aiList.Length];

        for (var i = 0; i < _aiList.Length; ++i)
        {
            _defaultPositions[i] = _aiList[i].transform.position;
            _distanceToBall[i] = (_aiList[i].transform.position - Football.transform.position).magnitude;
        }
    }

    public void ResetPosition()
    {
        for (var i = 0; i < _aiList.Length; ++i)
            _aiList[i].transform.position = _defaultPositions[i];
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.gm.status == GameManager.GameStatus.OffBorder ||
            GameManager.gm.status == GameManager.GameStatus.Goal)
        {
            return;
        }
        if (remain < 0)
        {
            UpdateAIPosition();
            remain = period;
        }
        else
        {
            remain -= Time.deltaTime;
        }

    }

    private void UpdateAIPosition()
    {
        for (var i = 0; i < _aiList.Length; ++i)
            _distanceToBall[i] = (_aiList[i].transform.position - Football.transform.position).magnitude;

        var ranked = _distanceToBall.Select((d, i) =>
                new {index = i, distance = d})
            .OrderBy(pair => pair.distance)
            .ToList();

        for (var i = 0; i < _aiList.Length; ++i)
        {
            var s = _aiList[ranked[i].index].GetComponent("AI") as AI;
            if (i == 0)
            {
                Debug.Assert(s != null, "s != null");
                GameManager.gm.AI_Active = s.gameObject;
                s.status = AI.Status.Attack;
            }
            else if (i == 1)
            {
                Debug.Assert(s != null, "s != null");
                s.status = AI.Status.Assist;
            }
            else
            {
                Debug.Assert(s != null, "s != null");
                s.status = AI.Status.Idle;
            }
        }
    }
}