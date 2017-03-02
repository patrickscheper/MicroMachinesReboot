using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public bool runGame = false;
    public bool runMenu = true;

    public List<Sprite> emblems;
    public Image[] positionImages;
    public GameObject[] mainCanvasItems;
    public Text text;

    public float timeTakenDuringLerp = 1f;
    public float _timeStartedLerping;


    public int u = 0;
    public float speed;

    public Transform currentWaypoint;
    public Transform[] waypoints;
    public Transform[] thingsToLookAt;
    public GameObject MenuCamera;


    public GameObject[] CarReference;
    public List<SimpleCarController> players;
    public List<Camera> cameras;

    public int amountPlayers = 4;

    public Transform[] startingPositions;

    public GameObject[] checkPoints;
    public int[] currentLap;
    public int[] currentCheckpoint;
    public float[] distanceNextWaypoint;

    public SimpleCarController player1;
    public SimpleCarController player2;
    public GameObject firstPlace;


    float t;
    public List<SimpleCarController> allCars;
    public SimpleCarController[] carOrder;

    private int i = 0;

    public void Awake()
    {
    }


    public void Start()
    {
        // set up the car objects


    }

    // this gets called every frame
    public void ManualUpdate()
    {
        foreach (SimpleCarController car in allCars)
        {
            carOrder[car.GetCarPosition(allCars.ToArray()) - 1] = car;
            positionImages[car.GetCarPosition(allCars.ToArray()) - 1].sprite = car.emblem;
        

        }
        

    }

    private void FixedUpdate()
    {
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
        currentWaypoint = waypoints[0];

    }

    public void OnGame()
    {

        mainCanvasItems[0].SetActive(false);


        Debug.Log(Input.GetJoystickNames().Length);
        if (runGame)
        {
            if (Input.GetJoystickNames().Length < amountPlayers)
            {
                print("Does this happen?");
                int i = 0;
                foreach (string gamepad in Input.GetJoystickNames())
                {
                    GameObject newCar = Instantiate(CarReference[i], startingPositions[i].position, startingPositions[i].rotation);
                    SimpleCarController carController = newCar.GetComponent<SimpleCarController>();
                    carController.m_PlayerNumber = i;
                    carController.lastWaypoint = checkPoints[0].transform;
                    emblems.Add(carController.emblem);
                    players.Add(carController); 
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
                        cameras[0].rect = new Rect(0, .5f, .5f, .5f);
                        cameras[1].rect = new Rect(.5f, .5f, .5f, .5f);
                        cameras[2].rect = new Rect(0, 0, 1, .5f);
                        break;
                    case 2:
                        print("Two players are connected, setting up cameras.");
                        positionImages[3].gameObject.SetActive(false);
                        positionImages[2].gameObject.SetActive(false);
                        cameras[0].rect = new Rect(0, 0, .5f, 1);
                        cameras[1].rect = new Rect(.5f, 0, .5f, 1);

                        break;
                    case 1:
                        positionImages[3].gameObject.SetActive(false);
                        positionImages[2].gameObject.SetActive(false);
                        positionImages[1].gameObject.SetActive(false);
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
            foreach (SimpleCarController car in players)
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

        foreach (SimpleCarController car in players)
            car.enabled = true;

        mainCanvasItems[1].SetActive(false);

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
