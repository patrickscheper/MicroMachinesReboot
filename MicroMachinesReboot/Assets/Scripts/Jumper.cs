using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{

    public float force = 250;

    private void OnTriggerEnter(Collider other)
    {
        print("Jump!");
        other.GetComponent<Rigidbody>().AddForce(Vector3.up * force);
    }
}
