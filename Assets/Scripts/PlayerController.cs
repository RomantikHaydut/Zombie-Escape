using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Control Settings")]
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float gravityModifier = 0.95f;
    [SerializeField] float jumpPower = 0.25f;
    [SerializeField] InputAction newMovementInput;   // This action is for movement. For example jump action decleare another input.
    [Header("Mouse Control Options")]
    [SerializeField] float mouseSensivity = 1f;
    [SerializeField] float maxViewAngle = 60f;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [Header("Sound Settings")]
    [SerializeField] List<AudioClip> footStepSounds = new List<AudioClip>();
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    CharacterController characterController;

    Transform mainCamera;

    Animator anim;

    AudioSource audioSource;

    int lastIndex = -1;

    float currentSpeed = 8f;
    float horizontalInput;
    float verticalInput;

    Vector3 heightMovement;
    bool jump;
    bool landSoundPlayed = true;





    private void Awake()
    {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();


        if (Camera.main.gameObject.GetComponent<CameraController>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }

        mainCamera = GameObject.Find("CameraPoint").transform;
    }

    private void OnEnable()
    {
        newMovementInput.Enable();
    }

    private void OnDisable()
    {
        newMovementInput.Disable();
    }


    void Update()
    {
        KeyboardInput();

        AnimationChanger();




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
                Vector3 rotateVector = mainCamera.rotation.eulerAngles + new Vector3(-MouseInput().y, 0, 0);
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
            audioSource.PlayOneShot(jumpSound);
            landSoundPlayed = false;
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
            if (!landSoundPlayed)
            {
                audioSource.PlayOneShot(landSound);
                landSoundPlayed = true;
            }
        }

    }

    private void AnimationChanger()
    {
        if (newMovementInput.ReadValue<Vector2>().magnitude > 0f && !anim.GetBool("Run_b")) // Means if our input value greater than zero.
        {
            anim.SetBool("Walk_b", true);
        }
        else
        {
            anim.SetBool("Walk_b", false);
        }
    }

    private void KeyboardInput()
    {
        horizontalInput = newMovementInput.ReadValue<Vector2>().x;  // Here we get input value from newMomentInput.
        verticalInput = newMovementInput.ReadValue<Vector2>().y;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)
        {
            jump = true;
        }


        if (Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed = runSpeed;

            if (!anim.GetBool("Run_b") && newMovementInput.ReadValue<Vector2>().magnitude > 0f)
            {
                anim.SetBool("Run_b", true);
                anim.SetBool("Walk_b", false);
            }

        }
        else
        {
            currentSpeed = walkSpeed;

            anim.SetBool("Run_b", false);
        }
    }

    Vector2 MouseInput()
    {
        // Here we used ? operand and we changed mouse inputs according to invertX and invertY booleans.
        return new Vector2(invertX ? -Mouse.current.delta.x.ReadValue() : Mouse.current.delta.x.ReadValue(), invertY ? -Mouse.current.delta.y.ReadValue() : Mouse.current.delta.y.ReadValue()) * mouseSensivity;
        //return new Vector2(invertX ? -Input.GetAxisRaw("Mouse X") : Input.GetAxisRaw("Mouse X"), invertY ? -Input.GetAxisRaw("Mouse Y") : Input.GetAxisRaw("Mouse Y")) * mouseSensivity;
    }

    void PlayStepSound()
    {

        if (footStepSounds.Count > 0 && audioSource != null)
        {
            int index;
            do
            {
                index = UnityEngine.Random.Range(0, footStepSounds.Count);
                if (lastIndex != index)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(footStepSounds[index]);
                        lastIndex = index;
                        break;
                    }
                    
                }
            }
            while (index == lastIndex);
        }

    }



}
