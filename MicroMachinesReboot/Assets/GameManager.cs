using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool runMenu;
    public bool isGameRunning;
    public GameObject[] canvasObjects;

    public bool[] finishedCars;

    public GameObject[] CarReference;

    public int maxLap = 1;

    [HideInInspector]
    public List<CarController> allCars;
    [HideInInspector]
    public CarController[] carOrder;


    public Text[] playerLaps;

    public List<Sprite> emblems;

    public MenuManager menuManager;

    public List<Sprite> playerEmblems;
    public Image[] EmblemOrder;
    public Image[] positionImages;
    public Transform[] carCheckpoints;


    public Text BeginTimer;
    public bool showTime;

    public List<Camera> cameras;

    public int amountPlayers = 2;

    public Text TimePlaying;

    public float startTime;
    public float elapsedTime;

    public Text counterText;
    public bool goCounter = false;
    public float counter = 30;


    public Transform[] startingPositions;

    //   public GameObject[] checkPoints;

    void Awake()
    {


        amountPlayers = Input.GetJoystickNames().Length;
        finishedCars = new bool[amountPlayers];

    }

    private void OnEnable()
    {


        //set timer, lap setup enabled, so the player can see.
        foreach (GameObject item in canvasObjects)
        {
            item.SetActive(true);
        }

        if(isGameRunning == true)
        {
            print("Waarom werkte dit niet?");
            if (Input.GetJoystickNames().Length <= amountPlayers)
            {
                int i = 0;
                foreach (string gamepad in Input.GetJoystickNames())
                {
                    GameObject newCar = Instantiate(CarReference[i], startingPositions[i].position, startingPositions[i].rotation);
                    CarController carController = newCar.GetComponent<CarController>();
                    carController.checkpointChecks = new bool[carCheckpoints.Length];
                    carController.playerNumber = i;
                    carController.lastWaypoint = carCheckpoints[0];

                    //    carController.lastWaypoint = checkPoints[0].transform;
                    emblems.Add(carController.emblem);
                    //      players.Add(carController); 
                    allCars.Add(carController);
                    cameras.Add(newCar.GetComponentInChildren<Camera>());
                    i++;
                }

                //changes the camera
                switch (cameras.Count)
                {
                    case 4:
                        print("Four players are connected, setting up cameras.");
                        cameras[0].rect = new Rect(0, .5f, .5f, .5f);
                        cameras[1].rect = new Rect(.5f, .5f, .5f, .5f);
                        cameras[2].rect = new Rect(0, 0, .5f, .5f);
                        cameras[3].rect = new Rect(.5f, 0, .5f, .5f);
                        break;
                    case 3:
                        print("Three players are connected, setting up cameras.");

                        positionImages[3].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        cameras[0].rect = new Rect(0, .5f, .5f, .5f);
                        cameras[1].rect = new Rect(.5f, .5f, .5f, .5f);
                        cameras[2].rect = new Rect(0, 0, 1, .5f);
                        break;
                    case 2:
                        print("Two players are connected, setting up cameras.");
                        positionImages[3].gameObject.SetActive(false);
                        positionImages[2].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        playerLaps[2].gameObject.SetActive(false);
                        cameras[0].rect = new Rect(0, 0, .5f, 1);
                        cameras[1].rect = new Rect(.5f, 0, .5f, 1);

                        break;
                    case 1:
                        positionImages[3].gameObject.SetActive(false);
                        positionImages[2].gameObject.SetActive(false);
                        positionImages[1].gameObject.SetActive(false);
                        playerLaps[3].gameObject.SetActive(false);
                        playerLaps[2].gameObject.SetActive(false);
                        playerLaps[1].gameObject.SetActive(false);
                        print("One player are connected, setting up cameras.");

                        cameras[0].rect = new Rect(0, 0, 1, 1);

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

        //If match is done, reset and disable canvas properties.
            foreach(GameObject item in canvasObjects)
            {
                item.SetActive(false);
            }

        showTime = false;


    }



  //  public List<CarController> players;




  //  public int[] currentLap;
 //   public int[] currentCheckpoint;
 //   public float[] distanceNextWaypoint;

 //   public CarController player1;
 //  public CarController player2;
//    public GameObject firstPlace;


   // float t;


 //   private int i = 0;


    //This is a nice way of not updating every damn frame but rather 1,5 seconds.
    public void ManualUpdate()
    {
        for (int i = 0; i < allCars.Count; i++)
        {
            playerLaps[i].text = "LAP: " + allCars[i].currentLap.ToString() + " / " + maxLap.ToString();
            if(allCars[i].currentLap == maxLap)
            {
                finishedCars[i] = true;
                IsFinished();
                allCars[i].enabled = false;
                goCounter = true;
            }
            carOrder[allCars[i].GetCarPosition(allCars.ToArray()) - 1] = allCars[i];
            positionImages[allCars[i].GetCarPosition(allCars.ToArray()) - 1].sprite = allCars[i].emblem;




        }
    }


    public void IsFinished()
    {
        for (int i = 0; i < finishedCars.Length; ++i)
        {
            if (finishedCars[i] == false)
            {
                return;
            }
            

        }
        OnWin();
    }


    private void FixedUpdate()
    {

        if(showTime)
        {
            elapsedTime = Time.time - startTime;
            TimePlaying.text = (elapsedTime / 60).ToString("00") + ":" + (elapsedTime % 60).ToString("00");
        }

       if(goCounter)
        {
            counter = counter - Time.deltaTime;
            counterText.text = "Remainging time: " + (counter / 60).ToString("00") + ":" + (counter % 60).ToString("00");

            if (counter <= 0)
            {
                
               // show menu, new game can be played OnMenu();
                goCounter = false;
            }

        }
       



    }

    public void OnPause()
    {
        menuManager.enabled = true;
        menuManager.isPaused = true;
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

/*
    public GameObject GrabNewWaypoint(int i)
    {

        if (i == checkPoints.Length)
            return checkPoints[0];

        else
            return checkPoints[i + 1];

    }
    */
}
