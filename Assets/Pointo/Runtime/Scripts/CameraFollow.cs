using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float movementTime = 5f;

    [Header("Movement")] [SerializeField] private float moveSpeed;

    [Header("Rotation")] [SerializeField] private float rotationSpeed;

    [Header("Zoom")] [SerializeField] private Transform cameraTransform;

    [SerializeField] private float minZoom = 4f;
    [SerializeField] private float maxZoom = 40;
    [SerializeField] private Vector3 zoomSpeed;
    private Vector3 currentPos;
    private Quaternion currentRotation;
    private Vector2 mouseLook;
    private Vector3 newZoom;

    private Vector3 startPos;
    private Quaternion startRotation;

    private void Start()
    {
        startPos = transform.position;
        startRotation = transform.rotation;

        currentPos = startPos;
        currentRotation = startRotation;

        newZoom = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * movementTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * movementTime);
        }

        HandleMove();

        HandleRot();

        HandleZoom();
    }

    private void HandleMove()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        currentPos += (transform.right * x + transform.forward * z) * moveSpeed;

        transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * movementTime);
    }

    private void HandleRot()
    {
        var horizontal = 0f;

        if (Input.GetMouseButton(2)) horizontal = Input.GetAxis("Mouse X");

        if (Input.GetKey(KeyCode.Q)) horizontal = -1f;

        if (Input.GetKey(KeyCode.E)) horizontal = 1f;

        currentRotation *= Quaternion.Euler(Vector3.up * horizontal * rotationSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, currentRotation, Time.deltaTime * movementTime);
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomSpeed;
            newZoom.x = 0;
            newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
            newZoom.z = Mathf.Clamp(newZoom.z, minZoom, maxZoom);

            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        }
    }
}