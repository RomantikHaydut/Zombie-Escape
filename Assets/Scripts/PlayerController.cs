using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Control Settings")]
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float gravityModifier = 0.95f;
    [SerializeField] float jumpPower = 0.25f;
    [Header("Mouse Control Options")]
    [SerializeField] float mouseSensivity = 1f;
    [SerializeField] float maxViewAngle = 60f;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    CharacterController characterController;

    float currentSpeed = 8f;
    float horizontalInput;
    float verticalInput;

    Vector3 heightMovement;
    bool jump;


    Transform mainCamera;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = GameObject.Find("CameraPoint").transform;

        if (Camera.main.gameObject.GetComponent<CameraController>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }
    }

    void Update()
    {
        KeyboardInput();

    }


    private void FixedUpdate()
    {
        Move();

        Rotate();

    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + MouseInput().x, transform.eulerAngles.z);

        // Here we clamp camera view.
        if (mainCamera != null)
        {

            if (mainCamera.eulerAngles.x > maxViewAngle && mainCamera.eulerAngles.x < 180f)
            {
                mainCamera.rotation = Quaternion.Euler(maxViewAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);
            }
            else if (mainCamera.eulerAngles.x > 180f && mainCamera.eulerAngles.x < 360f - maxViewAngle)
            {
                mainCamera.rotation = Quaternion.Euler(360f - maxViewAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);
            }
            else
            {
                mainCamera.rotation = Quaternion.Euler(mainCamera.rotation.eulerAngles + new Vector3(-MouseInput().y, 0, 0));
            }
        }



    }

    private void Move()
    {
        // Here we set heightMovement according to jump ability.
        if (jump)
        {
            heightMovement.y = jumpPower;
            jump = false;
        }

        heightMovement.y -= gravityModifier * Time.deltaTime; // For gravity effect , heightMovement vector is getting less everytime.

        Vector3 localVerticalVector = transform.forward * verticalInput;
        Vector3 localHoriztonalVector = transform.right * horizontalInput;

        Vector3 movementVector = (localHoriztonalVector + localVerticalVector).normalized;

        characterController.Move((movementVector * Time.deltaTime * currentSpeed) + heightMovement);

        //Here we set heightMovement to zero when we on ground.
        if (characterController.isGrounded)
        {
            heightMovement.y = 0f;
        }

    }

    private void KeyboardInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            jump = true;
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    Vector2 MouseInput()
    {
        // Here we used ? operand and we changed mouse inputs according to invertX and invertY booleans.
        return new Vector2(invertX ? -Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse X"), invertY ? -Input.GetAxisRaw("Mouse Y")  : Input.GetAxisRaw("Mouse Y")) * mouseSensivity;
    }



}
