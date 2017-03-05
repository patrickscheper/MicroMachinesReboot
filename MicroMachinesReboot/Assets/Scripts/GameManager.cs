using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Main Attributes")]
    [Space(2)]
    [Tooltip("Assaign here the maximum laps that a player should play in order to win.")]
    public int maxLap = 1;
    [Tooltip("Assaign here the menu manager on this game object.")]
    public MenuManager menuManager;
    [Tooltip("Assaign here all the references to the player (car) prefabs. Idea: Use a wheel of fortune for players to randomly get a car, maybe with different abilities? (Variation!)")]
    public GameObject[] CarReference;
    [Tooltip("Assaign here all the starting positions of the players.")]
    public Transform[] startingPositions;
    [Space(10)]

    [Header("Canvas Attributes")]
    [Space(2)]
    [Tooltip("Assaign here all the canvas objects that should be hidden once the menu is enabled.")]
    public GameObject[] canvasObjects;
    [Tooltip("Assaign here all the player emblems, these should be in order.")]
    public Image[] emblemOrder;
    [Tooltip("Assaign here all the player lap texts, these should be in order.")]
    public Text[] playerLaps;
    [Space(10)]

    [Header("Time Attributes")]
    [Space(2)]
    [Tooltip("Assaign here the timer text.")]
    public Text BeginTimer;
    [Tooltip("Assaign here the remaining time text.")]
    public Text counterText;
    [Tooltip("Assaign here how long the remaining time should be!")]
    public float counter = 30;

    [HideInInspector]
    public int amountPlayers = 2;

    [HideInInspector]
    public bool runMenu;

    [HideInInspector]
    public List<Camera> gameCameras;

    [HideInInspector]
    public bool isGameRunning;

    [HideInInspector]
    public bool[] finishedCars;

    [HideInInspector]
    public List<CarController> allCars;

    [HideInInspector]
    public CarController[] carOrder;

    [HideInInspector]
    public List<Sprite> playerEmblems;

    [HideInInspector]
    public Transform[] carCheckpoints;

    [HideInInspector]
    public bool showTime;

    [HideInInspector]
    public Text TimePlaying;

    private float startTime;
    private float elapsedTime;
    private bool goCounter = false;

    void Awake()
    {
        menuManager = GetComponent<MenuManager>();
        amountPlayers = Input.GetJoystickNames().Length;
        finishedCars = new bool[amountPlayers];
    }

    private void OnEnable()
    {
        foreach (GameObject item in canvasObjects)
            item.SetActive(true);

        if (isGameRunning != true)
        {
            if (Input.GetJoystickNames().Length <= amountPlayers)
            {
                int i = 0;
                foreach (string gamepad in Input.GetJoystickNames())
                {
                    GameObject newCar = Instantiate(CarReference[i], startingPositions[i].position, startingPositions[i].rotation);
                    CarController carController = newCar.GetComponent<CarController>();
                    carController.checkpointChecks = new bool[carCheckpoints.Length];
                    carController.playerNumber = i;
                    carController.lastCheckpoint = carCheckpoints[0];

                    playerEmblems.Add(carController.emblem);
                    allCars.Add(carController);
                    gameCameras.Add(newCar.GetComponentInChildren<Camera>());
                    i++;
                }

                switch (gameCameras.Count)
                {
                    case 4:
                        print("Four players are connected, setting up cameras.");
                        gameCameras[0].rect = new Rect(0, .5f, .5f, .5f);
                        gameCameras[1].rect = new Rect(.5f, .5f, .5f, .5f);
                        gameCameras[2].rect = new Rect(0, 0, .5f, .5f);
                        gameCameras[3].rect = new Rect(.5f, 0, .5f, .5f);
                        break;
                    case 3:
                        print("Three players are connected, setting up cameras.");

                        emblemOrder[3].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        gameCameras[0].rect = new Rect(0, .5f, .5f, .5f);
                        gameCameras[1].rect = new Rect(.5f, .5f, .5f, .5f);
                        gameCameras[2].rect = new Rect(0, 0, 1, .5f);
                        break;
                    case 2:
                        print("Two players are connected, setting up cameras.");
                        emblemOrder[3].gameObject.SetActive(false);
                        emblemOrder[2].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        playerLaps[2].gameObject.SetActive(false);
                        gameCameras[0].rect = new Rect(0, 0, .5f, 1);
                        gameCameras[1].rect = new Rect(.5f, 0, .5f, 1);

                        break;
                    case 1:
                        emblemOrder[3].gameObject.SetActive(false);
                        emblemOrder[2].gameObject.SetActive(false);
                        emblemOrder[1].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        playerLaps[2].gameObject.SetActive(false);
                        playerLaps[1].gameObject.SetActive(false);
                        print("One player are connected, setting up cameras.");

                        gameCameras[0].rect = new Rect(0, 0, 1, 1);

                        break;
                    default:
                        print("There are no controllers added, so no cars will be instantiated nor will the camera setup process start.");
                        break;
                }
            }
            else
                Debug.Log("No controllers found, game will not function like it should.");

            carOrder = new CarController[allCars.Count];
            foreach (CarController car in allCars)
                car.enabled = false;

            InvokeRepeating("ManualUpdate", 0.5f, 0.5f);
            StartCoroutine("Countdown");
        }
    }

    private void OnDisable()
    {
        foreach (GameObject item in canvasObjects)
            item.SetActive(false);

        showTime = false;
    }

    private void FixedUpdate()
    {
        if (showTime)
        {
            elapsedTime = Time.time - startTime;
            TimePlaying.text = (elapsedTime / 60).ToString("00") + ":" + (elapsedTime % 60).ToString("00");
        }

        if (goCounter)
        {
            counter = counter - Time.deltaTime;
            counterText.text = "Remainging time: " + (counter / 60).ToString("00") + ":" + (counter % 60).ToString("00");

            if (counter <= 0)
                goCounter = false;
        }
    }

    public void ManualUpdate()
    {
        //This is a nice way of not updating every damn frame but rather 1,5 seconds.
        for (int i = 0; i < allCars.Count; i++)
        {
            playerLaps[i].text = "LAP: " + allCars[i].currentLap.ToString() + " / " + maxLap.ToString();

            if (allCars[i].currentLap == maxLap)
            {
                finishedCars[i] = true;
                IsFinished();
                allCars[i].enabled = false;
                goCounter = true;
            }
            carOrder[allCars[i].GetCarPosition(allCars.ToArray()) - 1] = allCars[i];
            emblemOrder[allCars[i].GetCarPosition(allCars.ToArray()) - 1].sprite = allCars[i].emblem;
        }
    }

    public void IsFinished()
    {
        for (int i = 0; i < finishedCars.Length; ++i)
        {
            if (finishedCars[i] == false)
                return;
        }

        OnWin();
    }

    public void OnPause()
    {
        menuManager.enabled = true;
        menuManager.isPaused = true;
        isGameRunning = true;
        enabled = false;
    }

    public void OnWin()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public IEnumerator Countdown()
    {
        BeginTimer.text = "3";
        yield return new WaitForSeconds(1.5f);

        BeginTimer.text = "2";
        yield return new WaitForSeconds(1.5f);


        BeginTimer.text = "1";
        yield return new WaitForSeconds(1.5f);

        BeginTimer.text = "GO";
        yield return new WaitForSeconds(1.5f);

        BeginTimer.text = "";

        foreach (CarController car in allCars)
            car.enabled = true;

        showTime = true;

        startTime = Time.time;

        yield return null;
    }
}
