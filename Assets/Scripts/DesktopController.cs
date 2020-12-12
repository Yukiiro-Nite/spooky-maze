using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopController : MonoBehaviour
{
    public float speed = 2f;
    public float mouseSensitivity = 100f;
    public Transform camera;
    private float xRotation = 0f;
    private CharacterController controller;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        xRotation = Mathf.Clamp(xRotation - mouseY, -90, 90);
        
        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
