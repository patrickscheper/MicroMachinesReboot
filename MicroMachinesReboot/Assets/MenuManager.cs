using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{


    public bool runGame;

    public GameManager gameManager;

    public GameObject[] menuObjects;

    public bool isPaused;

    private int currentLookpoint = 0;
    public float cameraMoveSpeed = 50;



    public Button playButton;
    public GameObject controllerWarning;

    public Transform currentWaypoint;
    public Transform[] waypoints;
    public Transform[] thingsToLookAt;
    public GameObject MenuCamera;

    void Awake()
    {

        if (Input.GetJoystickNames().Length <= 0)
        {
            controllerWarning.SetActive(true);
            playButton.interactable = false;

        }
        if (Input.GetJoystickNames().Length > 0)
        {
            controllerWarning.SetActive(false);
            playButton.interactable = true;
        }

    }

    void FixedUpdate()
    {

        if (!isPaused)
        {
            float t = cameraMoveSpeed * Time.deltaTime;

            MenuCamera.transform.position = Vector3.MoveTowards(MenuCamera.transform.position, currentWaypoint.transform.position, t);

            //       Vector3 direction = thingsToLookAt[u].position - MenuCamera.transform.position;
            Quaternion toRotation = Quaternion.LookRotation(thingsToLookAt[currentLookpoint].position - MenuCamera.transform.position);
            MenuCamera.transform.rotation = Quaternion.Slerp(MenuCamera.transform.rotation, toRotation, 0.01f * Time.time);



            if (Vector3.Distance(MenuCamera.transform.position, currentWaypoint.transform.position) <= 0.2f)
            {
                currentLookpoint++;
                if (currentLookpoint == waypoints.Length)
                {
                    currentWaypoint = waypoints[0];
                    currentLookpoint = 0;
                }
                currentWaypoint = waypoints[currentLookpoint];



            }
        }


    }

    private void OnEnable()
    {

        foreach (GameObject item in menuObjects)
        {
            item.SetActive(true);
        }


        if (isPaused)
        {
            // disable all car components.
            foreach (CarController car in gameManager.allCars)
            {
                car.enabled = false;
            }
        }
        //This means it's the first time booting, or after a match.
        if (!isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = true;

            //destroy all car components.


            currentWaypoint = waypoints[0];



        }



    }

    private void OnDisable()
    {
        
        foreach (GameObject item in menuObjects)
        {
            item.SetActive(false);
        }

        if (isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = false;

            //set counter on, that one will change the enablity to true.
            StartCoroutine(gameManager.Countdown());

        }
        if (!isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = false;

            foreach (CarController car in gameManager.allCars)
            {
                car.enabled = false;
            }

        }


    }

    public void OnPlay()
    {
        if(!isPaused)
        {
            gameManager.isGameRunning = true;

            gameManager.enabled = true;
            enabled = false;
        }

        if (isPaused)
        {
                gameManager.enabled = true;
            gameManager.StopAllCoroutines();
            gameManager.StartCoroutine("Countdown");

            
                enabled = false;
            }
    }



    public void OnQuit()
    {
        Application.Quit();

    }
}
