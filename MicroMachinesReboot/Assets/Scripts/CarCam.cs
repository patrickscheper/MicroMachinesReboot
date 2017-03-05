using UnityEngine;

public class CarCam : MonoBehaviour
{
    [Header("Main Attributes")]
    [Space(2)]
    [Tooltip("If car speed is below this value, then the camera will default to looking forwards.")]
    public float rotationThreshold = 1f;

    [Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
    public float cameraStickiness = 10.0f;

    [Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    public float cameraRotationSpeed = 5.0f;

    private Transform rootPos;
    private Transform carCam;
    private Transform car;

    void Awake()
    {
        carCam = Camera.main.GetComponent<Transform>();
        rootPos = GetComponent<Transform>();
        car = rootPos.parent.GetComponent<Transform>();
    }

    void Start()
    {
        rootPos.parent = null;
    }

    void FixedUpdate()
    {
        Quaternion look;

        rootPos.position = Vector3.Lerp(rootPos.position, car.position, cameraStickiness * Time.fixedDeltaTime);
        look = Quaternion.LookRotation(-car.up + car.forward);
        look = Quaternion.Slerp(rootPos.rotation, look, cameraRotationSpeed * Time.fixedDeltaTime);

        rootPos.rotation = look;
    }
}