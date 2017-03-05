using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Main Attributes")]
    [Space(2)]

    [Tooltip("Assaign here all the menu items that should dissapear when the game runs.")]
    public GameObject[] menuObjects;
    [Tooltip("This is a seperate reference to the play button, because it gets deactivated once there are no controllers plugged in.")]
    public Button playButton;
    [Tooltip("This also is a seperate reference purely to notice the player that he or she should restart the game with plugged in controllers.")]
    public GameObject controllerWarning;
    [Space(10)]

    [Header("Menu Camera Attributes")]
    [Space(2)]

    [Tooltip("This is the camera speed.")]
    public float cameraMoveSpeed = 50;
    [Tooltip("Assaign here all the waypoints to where the menu camera has to move!")]
    public Transform[] cameraWayPoints;
    [Tooltip("Assaign here all the objects that the camera will look while moving. It's should be in order with the waypoints.")]
    public Transform[] cameraFocusObjects;
    [Tooltip("This should be a reference to the game camera object.")]
    public GameObject MenuCamera;

    [HideInInspector]
    public bool runGame;

    [HideInInspector]
    public GameManager gameManager;

    [HideInInspector]
    public bool isPaused;

    [HideInInspector]
    private int currentLookpoint = 0;

    [HideInInspector]
    public Transform currentWaypoint;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();

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

            Quaternion toRotation = Quaternion.LookRotation(cameraFocusObjects[currentLookpoint].position - MenuCamera.transform.position);
            MenuCamera.transform.rotation = Quaternion.Slerp(MenuCamera.transform.rotation, toRotation, 0.01f * Time.time);

            if (Vector3.Distance(MenuCamera.transform.position, currentWaypoint.transform.position) <= 0.2f)
            {
                currentLookpoint++;
                if (currentLookpoint == cameraWayPoints.Length)
                {
                    currentWaypoint = cameraWayPoints[0];
                    currentLookpoint = 0;
                }
                currentWaypoint = cameraWayPoints[currentLookpoint];
            }
        }
    }

    private void OnEnable()
    {
        foreach (GameObject item in menuObjects)
            item.SetActive(true);

        if (isPaused)
        {
            foreach (CarController car in gameManager.allCars)
            {
                car.enabled = false;
            }
        }

        if (!isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = true;
            currentWaypoint = cameraWayPoints[0];
        }
    }

    private void OnDisable()
    {
        foreach (GameObject item in menuObjects)
            item.SetActive(false);

        if (isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = false;
            StartCoroutine(gameManager.Countdown());
        }
        if (!isPaused)
        {
            MenuCamera.GetComponent<Camera>().enabled = false;

            foreach (CarController car in gameManager.allCars)
                car.enabled = false;
        }
    }

    public void OnPlay()
    {
        if (!isPaused)
        {
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
