using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;

    public float DeathZone = -0.1f;
    public float AirZone;

    public bool isSpawning = false;
    public float bumpForce = 20;
    public ParticleSystem explosionParticle;
    public GameObject waypoint;
    private Quaternion oldRotation;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;

    public GameObject[] slipParticles;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

    }


    private void OnEnable()
    {
        waypoint.transform.parent = null;
        m_Rigidbody.isKinematic = false;

        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;

    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;

    }

    private void Start()
    {
        AirZone = transform.position.y + 0.01f;
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

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


    private void Update()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);


        if (transform.position.y > AirZone)
            foreach (GameObject item in slipParticles)
            {
                item.SetActive(false);
            }

        if (transform.position.y > AirZone)
            foreach (GameObject item in slipParticles)
            {
                item.SetActive(true);
            }



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
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
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
