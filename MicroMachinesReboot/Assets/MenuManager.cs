using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {


    public bool runGame;

    public bool isInGame;

    public int u = 0;
    public float speed;

    public Transform currentWaypoint;
    public Transform[] waypoints;
    public Transform[] thingsToLookAt;
    public GameObject MenuCamera;

    void Awake () {
		
	}
	
	void Update () {
		
	}

    private void OnEnable()
    {
        if(isInGame)
        {

        }
        else
        {

        }



    }

    private void OnDisable()
    {
        if (isInGame)
        {

        }
        else
        {

        }


    }
}
