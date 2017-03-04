using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public int currentWaypoint;
    public GameManager gameManager;

	void Start () {

        if(gameManager == null)
        {
            Debug.LogWarning("Can't find Gamemanager in Checkpoint, trying to add.");
            gameManager = GameObject.FindGameObjectWithTag("Gamemanager").GetComponent<GameManager>();
            if (!gameManager)
            {
                Debug.LogError("Couldn't find Gamemanager, game won't function properly.");
            }
        }
		
	}

    private void OnTriggerEnter(Collider other)
    {


        CarController SCC = other.GetComponent<CarController>();

        if (currentWaypoint == 1)
            SCC.ResetLapCounter();

        SCC.currentCheckpoint = currentWaypoint;

        SCC.checkpointChecks[currentWaypoint] = true;

        SCC.IsNewLap();

    }
}
