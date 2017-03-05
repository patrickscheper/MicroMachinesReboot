using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("This should be the checkpoint number.")]
    public int currentWaypoint;
    [Tooltip("Assaign here a reference to the game manager.")]
    public GameManager gameManager;

    void Awake()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("Can't find Gamemanager in Checkpoint, trying to add.");
            gameManager = GameObject.FindGameObjectWithTag("Gamemanager").GetComponent<GameManager>();

            if (!gameManager)
                Debug.LogError("Couldn't find Gamemanager, game won't function properly.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();

        if (currentWaypoint == 1)
            carController.ResetLapCounter();

        carController.currentCheckpoint = currentWaypoint;
        carController.checkpointChecks[currentWaypoint] = true;
        carController.IsNewLap();
    }
}
