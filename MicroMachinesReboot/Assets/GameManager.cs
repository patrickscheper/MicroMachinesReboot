using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public bool runGame = false;
    public bool runMenu = true;

    public List<Sprite> emblems;
    public Image[] positionImages;

    public Text[] playerLaps;

    public GameObject[] mainCanvasItems;
    public Text text;

 //   public float timeTakenDuringLerp = 1f;
 //   public float _timeStartedLerping;

    public Button playButton;
    public GameObject controllerWarning;

    public int maxLap = 1;


    public int u = 0;
    public float speed;

    public bool showTime;

    public Transform currentWaypoint;
    public Transform[] waypoints;
    public Transform[] thingsToLookAt;
    public GameObject MenuCamera;


    public GameObject[] CarReference;
  //  public List<SimpleCarController> players;
    public List<Camera> cameras;

    public int amountPlayers = 2;

    public Text TimePlaying;

    public float startTime;
    public float elapsedTime;

    public Text counterText;
    public bool goCounter = false;
    public float counter = 30;

    public Transform[] startingPositions;

    public GameObject[] checkPoints;
  //  public int[] currentLap;
 //   public int[] currentCheckpoint;
 //   public float[] distanceNextWaypoint;

 //   public SimpleCarController player1;
 //  public SimpleCarController player2;
//    public GameObject firstPlace;


    float t;
    public List<SimpleCarController> allCars;
    public SimpleCarController[] carOrder;

    private int i = 0;

    public void Awake()
    {

        if (Input.GetJoystickNames().Length <= 0)
        {
            controllerWarning.SetActive(true);
            playButton.interactable = false;

        }
        else if (Input.GetJoystickNames().Length > 0)
        {
            controllerWarning.SetActive(false);
            playButton.interactable = true;
        }
    }

    public void ManualUpdate()
    {
        

        for (int i = 0; i < allCars.Count; i++)
        {
           playerLaps[i].text = "LAP: " + allCars[i].currentLap.ToString() + " / " + maxLap.ToString();
        }

        foreach (SimpleCarController car in allCars)
        {
            if (car.currentLap == maxLap)
            {
                car.enabled = false;
                goCounter = true;
            }

            carOrder[car.GetCarPosition(allCars.ToArray()) - 1] = car;
            positionImages[car.GetCarPosition(allCars.ToArray()) - 1].sprite = car.emblem;
            

        }
        

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
                
                OnMenu();
                goCounter = false;
            }

        }
       

        if (runMenu)
        {

            float t = speed * Time.deltaTime;

            MenuCamera.transform.position = Vector3.MoveTowards(MenuCamera.transform.position, currentWaypoint.transform.position, t);

            //       Vector3 direction = thingsToLookAt[u].position - MenuCamera.transform.position;
            Quaternion toRotation = Quaternion.LookRotation(thingsToLookAt[u].position - MenuCamera.transform.position);
            MenuCamera.transform.rotation = Quaternion.Slerp(MenuCamera.transform.rotation, toRotation, 0.01f * Time.time);



            if (Vector3.Distance(MenuCamera.transform.position, currentWaypoint.transform.position) <= 0.2f)
            {
                u++;
                if (u == waypoints.Length)
                {
                    currentWaypoint = waypoints[0];
                    u = 0;
                }
                currentWaypoint = waypoints[u];



            }
        }

    }

    public void OnPlay()
    {
        runGame = true;
        runMenu = false;
        OnGame();

    }

    public void OnMenu()
    {
        showTime = false;
        runGame = false;
        runMenu = true;
        currentWaypoint = waypoints[0];

        foreach(SimpleCarController car in carOrder)
        {
            Destroy(car.cameraParent);
            Destroy(car.gameObject);
        }
        MenuCamera.GetComponent<Camera>().enabled = true;

        CancelInvoke();

        mainCanvasItems[0].SetActive(true);
        mainCanvasItems[2].SetActive(false);




    }

    public void OnGame()
    {
        mainCanvasItems[0].SetActive(false);


        if (runGame)
        {
            if (Input.GetJoystickNames().Length < amountPlayers)
            {
                int i = 0;
                foreach (string gamepad in Input.GetJoystickNames())
                {
                    GameObject newCar = Instantiate(CarReference[i], startingPositions[i].position, startingPositions[i].rotation);
                    SimpleCarController carController = newCar.GetComponent<SimpleCarController>();
                    carController.m_PlayerNumber = i;
                    carController.lastWaypoint = checkPoints[0].transform;
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

                MenuCamera.GetComponent<Camera>().enabled = false;


            }
            else
                Debug.Log("No controllers found, game will not function like it should.");


            carOrder = new SimpleCarController[allCars.Count];
            foreach (SimpleCarController car in allCars)
                car.enabled = false;

            mainCanvasItems[2].SetActive(true);

            InvokeRepeating("ManualUpdate", 0.5f, 0.5f);
            StartCoroutine("Countdown");


        }
    }


    IEnumerator Countdown()
    {

        text.text = "3";
        yield return new WaitForSeconds(1.5f);

        text.text = "2";
        yield return new WaitForSeconds(1.5f);


        text.text = "1";
        yield return new WaitForSeconds(1.5f);

        text.text = "GO";
        yield return new WaitForSeconds(1.5f);

        text.text = "";
        print("Test");

        foreach (SimpleCarController car in allCars)
            car.enabled = true;

        mainCanvasItems[1].SetActive(false);
        showTime = true;
        startTime = Time.time;


        yield return null;

    }

    public void OnQuit()
    {
        Application.Quit();

    }

    public GameObject GrabNewWaypoint(int i)
    {

        if (i == checkPoints.Length)
            return checkPoints[0];

        else
            return checkPoints[i + 1];

    }
}
