using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesktopController : MonoBehaviour
{
    public float speed = 2f;
    public float mouseSensitivity = 100f;
    public Transform camera;
    public GameObject exitObject;
    public UnityEvent onExitHoverStart;
    public UnityEvent onExitHoverEnd;
    public UnityEvent onExitClick;
    public bool isTeleporting = false;
    public bool isHovering = false;
    private float xRotation = 0f;
    private CharacterController controller;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        // For some reason, if controller.Move is called while teleporting, it does not always work,
        // so cancel the current update if we're teleporting.
        // Why are you like this Unity?
        if(isTeleporting) {
            isTeleporting = false;
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        xRotation = Mathf.Clamp(xRotation - mouseY, -90, 90);
        
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        handleInteractions();
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
    }

    void handleInteractions()
    {
        Vector3 origin = camera.position;
        Vector3 direction = camera.forward;
        RaycastHit hitInfo;
        float maxDistance = 1.5f;

        bool hit = Physics.Raycast(origin, direction, out hitInfo, maxDistance, LayerMask.GetMask("Interactable"));

        if(hit && hitInfo.transform.gameObject == exitObject) {
            if(!isHovering) {
                onExitHoverStart.Invoke();
                isHovering = true;
            }

            if(Input.GetButton("Fire1")) {
                onExitClick.Invoke();
            }
        } else {
            if(isHovering) {
                onExitHoverEnd.Invoke();
                isHovering = false;
            }
        }
    }
}
