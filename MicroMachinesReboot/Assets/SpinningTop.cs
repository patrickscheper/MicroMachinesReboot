using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTop : MonoBehaviour
{

    public GameObject[] wayPoints;
    public GameObject currentWayPoint;


    void Awake()
    {

        InvokeRepeating("GotoWaypoint", 0, Random.Range(1, 4));

    }

    void GotoWaypoint()
    {
        currentWayPoint = wayPoints[Random.Range(0, wayPoints.Length)];
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, currentWayPoint.transform.position) > 0.1)
        {
            float t = 2 * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, currentWayPoint.transform.position, t);
        }

    }
}
