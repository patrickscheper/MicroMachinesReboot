using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {

    public SimpleCarController[] players;

    public Vector3[] currentPosition;
    public int[] currentLap;
    public int[] currentCheckpoint;

	void Start ()
    {


		
	}

	void Update ()
    {

        foreach(SimpleCarController player in players)
        {
            currentPosition[player.m_PlayerNumber] = player.transform.position;
            currentLap[player.m_PlayerNumber] = player.currentLap;
            currentCheckpoint[player.m_PlayerNumber] = player.currentCheckpoint;
        }
		
	}
}
