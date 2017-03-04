using UnityEngine;

public class CarController : MonoBehaviour
{

    public int currentCheckpoint;
    public int currentLap;
    public Transform lastWaypoint;
    public bool[] checkpointChecks;

    public ParticleSystem victoryParticles;

    private static int checkPointValue = 100;
    private static int lapValue = 10000;

    public Sprite emblem;

    public int playerNumber;
    public float speed = 12f;
    public float turnSpeed = 180f;

    public float deathZone = -0.1f;
    public float AirZone;

    public GameObject cameraParent;

    public bool isSpawning = false;
    public float bumpForce = 20;
    public ParticleSystem explosionParticle;
    public GameObject waypoint;
    private Quaternion oldRotation;

    private string movementAxisName;
    private string turnAxisName;
    private float movementInputValue;
    private float turnInputValue;

    private Rigidbody rb;

    private void Awake()
    {
        currentCheckpoint = 0;
        currentLap = 0;
        rb = GetComponent<Rigidbody>();

    }

    public float GetDistance()
    {
        return (transform.position - lastWaypoint.position).magnitude + currentCheckpoint * checkPointValue + currentLap * lapValue;
    }

    public int GetCarPosition(CarController[] allCars)
    {
        float distance = GetDistance();
        int position = 1;
        foreach (CarController car in allCars)
        {
            if (car.GetDistance() > distance)
                position++;
        }
        return position;
    }

    


private void OnEnable()
    {
        waypoint.transform.parent = null;
        rb.isKinematic = false;

        movementInputValue = 0f;
        turnInputValue = 0f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            print("Will not place a waypoint here, too close to trap");
            CancelInvoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Trap")
        {
            InvokeRepeating("CreateNewSpawnPoint", 0, 5);
        }
    }


    private void OnDisable()
    {
        rb.isKinematic = true;

    }

    private void Start()
    {
        AirZone = transform.position.y + 0.01f;
        movementAxisName = "Vertical" + (1 + playerNumber);
        turnAxisName = "Horizontal" + (1 + playerNumber);

        InvokeRepeating("CreateNewSpawnPoint", 0, 5);

    }

    private void CreateNewSpawnPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            print("Setting new waypoint");
            oldRotation = transform.rotation;
            waypoint.transform.position = hit.point;


        }
        else
        {

            return;
        }

    }

    public void IsNewLap()
    {
        for (int i = 0; i < checkpointChecks.Length; ++i)
        {
            if (checkpointChecks[i] == false)
            {
                return;
            }
        }

        victoryParticles.Play();
        currentLap += 1;
        ResetLapCounter();
    }

    public void ResetLapCounter()
    {
        for (int i = 0; i < checkpointChecks.Length; ++i)
        {
            checkpointChecks[i] = false;
        }
    }


    public void CalculatePosition(Vector3 currentPosition, Vector3 lastCheckpoint, Vector3 nextCheckpoint)
    {
        Vector3 displacementFromCurrentNode = currentPosition - lastCheckpoint;
        Vector3 currentSegmentVector = nextCheckpoint - lastCheckpoint;
        float fraction = Vector3.Dot(displacementFromCurrentNode, currentSegmentVector) /
            currentSegmentVector.sqrMagnitude;
        Debug.Log(fraction);
    }

    private void Update()
    {


        print(movementInputValue);
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(turnAxisName);



        if (transform.position.y < deathZone && isSpawning == false)
        {
            Destroyed();



        }
    }

    public void Destroyed()
    {
        isSpawning = true;
        Debug.Log("You died, trying to respawn.");
        explosionParticle.Play();
        this.enabled = false;
        explosionParticle.transform.parent = null;
        gameObject.SetActive(false);
        Invoke("SpawningNewCar", 2);
    }

    private void SpawningNewCar()
    {
        explosionParticle.transform.parent = transform;
        this.enabled = true;
        explosionParticle.transform.position = transform.position;
        transform.position = waypoint.transform.position;
        transform.rotation = oldRotation;
        gameObject.SetActive(true);
        isSpawning = false;
    }


    private void FixedUpdate()
    {
        Move();
        Turn();
    }


    private void Move()
    {
        if (movementInputValue < 0)
        {
            Vector3 movement = transform.forward * movementInputValue * (speed / 2) * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

        }

        if (movementInputValue > 0)
        {
            Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

        }

    }


    private void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Vector3 dir = collision.gameObject.transform.position - transform.position;
            dir = -dir.normalized + Vector3.up;
            print("Does this ever fucking happen?");
            rb.AddForce(dir * bumpForce);

        }
    }
}
