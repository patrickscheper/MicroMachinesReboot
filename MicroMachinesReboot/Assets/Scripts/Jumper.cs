using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [Tooltip("This is the force that the car gets launched")]
    public float force = 250;

    private void OnTriggerEnter(Collider other)
    {
        print("Jump!");
        other.GetComponent<Rigidbody>().AddForce(Vector3.up * force);
    }
}
