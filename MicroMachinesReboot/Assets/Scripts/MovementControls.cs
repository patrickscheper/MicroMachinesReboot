using UnityEngine;
using System.Collections;

public class MovementControls : MonoBehaviour
{

    Rigidbody rBody;
    public Vector3 COM = new Vector3(0, 0, 0);
    public WheelCollider[] wc;

    public int wc_Torque_Length;


    public bool BrakeAllowed;

    public float m_Torque = 2500f;
    public float m_Steer = 25f;
    public float m_Brake = 10000f;
    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.centerOfMass = COM;
    }

    void Update()
    {
        HandBrake();

    }
    // Update is called once per frame
    void FixedUpdate()
    {

        for (int i = 0; i < wc_Torque_Length; i++)
        {
            wc[i].motorTorque = Input.GetAxis("Vertical") * m_Torque;
        }

        wc[0].steerAngle = Input.GetAxis("Horizontal") * m_Steer;
        wc[1].steerAngle = Input.GetAxis("Horizontal") * m_Steer;

    }

    private void HandBrake()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            BrakeAllowed = true;
        }
        else
        {
            BrakeAllowed = false;
        }

        if (BrakeAllowed)
        {
            for (int i = 0; i < wc_Torque_Length; i++)
            {
                wc[i].brakeTorque = m_Brake;
                wc[i].motorTorque = 0f;
            }
        }
        else if (!BrakeAllowed && Input.GetButton("Vertical") == true)
        {
            for (int i = 0; i < wc_Torque_Length; i++)
            {
                wc[i].brakeTorque = 0;
            }
        }

    }
}