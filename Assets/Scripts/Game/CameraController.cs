using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float normalSpeed;
    public float megaSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;

    public Vector3 zoomAmount;
    public Vector3 newZoom;
    public Vector3 newPosition;
    public Quaternion newRotation;

    public float mouseScrollSensitivity = 10f; // Adjust sensitivity for mouse wheel zoom

    public KeyCode speedUpKey = KeyCode.Space; // Key to speed up time
    public float normalTimeScale = 1f;
    public float fastTimeScale = 4f; // Time scale when speed-up is active

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleTimeScale();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = megaSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        // Movement Controls
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }

        // Rotation Controls
        if ((Input.GetKey(KeyCode.Q)))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        // Zoom Controls with Keys
        if (Input.GetKey(KeyCode.Z))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.X))
        {
            newZoom -= zoomAmount;
        }

        // Zoom Controls with Mouse Wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            newZoom += zoomAmount * scrollInput * mouseScrollSensitivity;
        }

        // Smooth Transitions
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleTimeScale()
    {
        // Check for the speed-up key
        if (Input.GetKeyDown(speedUpKey))
        {
            Time.timeScale = fastTimeScale; // Speed up time
        }
        else if (Input.GetKeyUp(speedUpKey))
        {
            Time.timeScale = normalTimeScale; // Reset time to normal
        }
    }
}
