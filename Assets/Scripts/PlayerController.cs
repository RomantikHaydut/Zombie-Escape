using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Control Settings")]
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float runSpeed = 12f;

    CharacterController characterController;

    float currentSpeed = 8f;
    float horizontalInput;
    float verticalInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        KeyboardInput();
    }


    private void FixedUpdate()
    {
        Vector3 localVerticalVector = transform.forward * verticalInput;
        Vector3 localHoriztonalVector = transform.right * horizontalInput;

        Vector3 movementVector = (localHoriztonalVector + localVerticalVector).normalized;
        characterController.Move(movementVector * Time.deltaTime * currentSpeed);
    }

    private void KeyboardInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }



}
