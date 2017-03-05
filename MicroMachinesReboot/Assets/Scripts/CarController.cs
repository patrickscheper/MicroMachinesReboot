using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Main Attributes")]
    [Space(2)]
    [Tooltip("This is the speed of the car.")]
    public float speed;
    [Tooltip("This is the speed which the car turns.")]
    public float turnSpeed;
    [Tooltip("This is the speed in which a new spawn point is set.")]
    public int spawnTime;
    [Tooltip("This is the height (Y axis) on which the car gets destroyed, AKA:'Falls of table'")]
    public float deathZone = -0.1f;
    [Tooltip("This is the bump force that gets applied to another car if they collide.")]
    public float bumpForce = 50;
    [Space(10)]

    [Header("Misc.")]
    [Space(2)]
    [Tooltip("This should be the emblem that is shown as the race position.")]
    public Sprite emblem;
    [Tooltip("This should be the particles that play for a new lap, or respawn. Note: Do not loop these particles!")]
    public ParticleSystem victoryParticles;
    [Tooltip("This should be the particles that play if the car gets destroyed. Note: Do not loop these particles!")]
    public ParticleSystem explosionParticle;
    [Tooltip("This should be the camera that is a child of the car.")]
    public GameObject cameraParent;

    [HideInInspector]
    public int playerNumber;

    [HideInInspector]
    public int currentCheckpoint;

    [HideInInspector]
    public int currentLap;

    [HideInInspector]
    public Transform lastCheckpoint;

    [HideInInspector]
    public bool[] checkpointChecks;


    private static int checkPointValue = 100;
    private static int lapValue = 10000;

    public GameObject spawnPoint;
    private bool isSpawning = false;
    private Quaternion oldRotation;
    private string movementAxisName;
    private string turnAxisName;
    private float movementInputValue;
    private float turnInputValue;
    private Rigidbody rb;

    private void OnEnable()
    {
        spawnPoint = transform.FindChild("spawnPoint").gameObject;
        movementInputValue = 0f;
        turnInputValue = 0f;
    }

    private void Start()
    {
        spawnPoint.transform.parent = null;

        currentCheckpoint = 0;
        currentLap = 0;
        rb = GetComponent<Rigidbody>();

        movementAxisName = "Vertical" + (1 + playerNumber);
        turnAxisName = "Horizontal" + (1 + playerNumber);

        InvokeRepeating("CreateNewSpawnPoint", 0, spawnTime);
    }

    private void Update()
    {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(turnAxisName);
        Turn();

        if (transform.position.y < deathZone && isSpawning == false)
            Destroyed();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void CreateNewSpawnPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            print("Setting new spawn point!");
            oldRotation = transform.rotation;
            spawnPoint.transform.position = hit.point;
        }

        else
            return;
    }

    public void IsNewLap()
    {
        for (int i = 0; i < checkpointChecks.Length; ++i)
        {
            if (checkpointChecks[i] == false)
                return;
        }

        victoryParticles.Play();
        currentLap += 1;
        ResetLapCounter();
    }

    public void ResetLapCounter()
    {
        for (int i = 0; i < checkpointChecks.Length; ++i)
            checkpointChecks[i] = false;
    }

    public float GetDistance()
    {
        return (transform.position - lastCheckpoint.position).magnitude + currentCheckpoint * checkPointValue + currentLap * lapValue;
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

    public void CalculatePosition(Vector3 currentPosition, Vector3 lastCheckpoint, Vector3 nextCheckpoint)
    {
        Vector3 distanceFromCurCheckpoint = currentPosition - lastCheckpoint;
        Vector3 distanceBetweenCheckpoints = nextCheckpoint - lastCheckpoint;
        float fraction = Vector3.Dot(distanceFromCurCheckpoint, distanceBetweenCheckpoints) / distanceBetweenCheckpoints.sqrMagnitude;
    }

    public void Destroyed()
    {
        isSpawning = true;
        explosionParticle.Play();

        enabled = false;

        explosionParticle.transform.parent = null;
        gameObject.SetActive(false);

        Invoke("SpawningNewCar", 2);
    }

    private void SpawningNewCar()
    {
        explosionParticle.transform.parent = transform;
        enabled = true;

        explosionParticle.transform.position = transform.position;
        transform.position = spawnPoint.transform.position;
        transform.rotation = oldRotation;
        gameObject.SetActive(true);

        isSpawning = false;
    }

    private void Movement()
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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Vector3 dir = collision.gameObject.transform.position - transform.position;
            dir = -dir.normalized + Vector3.up;

            rb.AddForce(dir * bumpForce);
        }
    }
}
