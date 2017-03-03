using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public int currentCheckpoint;
    public int currentLap;
    public Transform lastWaypoint;
    public bool[] checkpoints;
    public int nbWaypoint; //Set the amount of Waypoints

    private static int WAYPOINT_VALUE = 100;
    private static int LAP_VALUE = 10000;
    private int cpt_waypoint = 0;

    public Sprite emblem;

    public int m_PlayerNumber;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;

    public float DeathZone = -0.1f;
    public float AirZone;

    public GameObject cameraParent;

    public bool isSpawning = false;
    public float bumpForce = 20;
    public ParticleSystem explosionParticle;
    public GameObject waypoint;
    private Quaternion oldRotation;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private float m_MovementInputValue;
    private float m_TurnInputValue;

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        currentCheckpoint = 0;
        currentLap = 0;
        cpt_waypoint = 0;
        checkpoints = new bool[nbWaypoint];
        m_Rigidbody = GetComponent<Rigidbody>();

    }

    public float GetDistance()
    {
        return (transform.position - lastWaypoint.position).magnitude + currentCheckpoint * WAYPOINT_VALUE + currentLap * LAP_VALUE;
    }

    public int GetCarPosition(SimpleCarController[] allCars)
    {
        float distance = GetDistance();
        int position = 1;
        foreach (SimpleCarController car in allCars)
        {
            if (car.GetDistance() > distance)
                position++;
        }
        return position;
    }

    


private void OnEnable()
    {
        waypoint.transform.parent = null;
        m_Rigidbody.isKinematic = false;

        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Killer")
            CancelInvoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Killer")
            InvokeRepeating("CreateNewSpawnPoint", 0, 5);
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;

    }

    private void Start()
    {
        AirZone = transform.position.y + 0.01f;
        m_MovementAxisName = "Vertical" + (1 + m_PlayerNumber);
        m_TurnAxisName = "Horizontal" + (1 + m_PlayerNumber);

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
        for (int i = 0; i < checkpoints.Length; ++i)
        {
            if (checkpoints[i] == false)
            {
                return;
            }
        }

        currentLap += 1;
        ResetLapCounter();
    }

    public void ResetLapCounter()
    {
        for (int i = 0; i < checkpoints.Length; ++i)
        {
            checkpoints[i] = false;
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


        print(m_MovementInputValue);
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);



        if (transform.position.y < DeathZone && isSpawning == false)
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
        if (m_MovementInputValue < 0)
        {
            Vector3 movement = transform.forward * m_MovementInputValue * (m_Speed / 2) * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        }

        if (m_MovementInputValue > 0)
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        }

    }


    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Vector3 dir = collision.gameObject.transform.position - transform.position;
            dir = -dir.normalized + Vector3.up;
            print("Does this ever fucking happen?");
            m_Rigidbody.AddForce(dir * bumpForce);

        }
    }
}
