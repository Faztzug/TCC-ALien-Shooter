using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Movimento : MonoBehaviour
{
    [Header("Character Values")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float runAccelaration = 3f;
    [SerializeField] private float inerciaDeccalaration = 5f;
    private float ungroudedTime;
    private float currentSpeed;
    private Vector3 movementInput;
    private Vector3 lastMovementInput;
    private float lastInputSpeed;
    [SerializeField] [Range(0.5f,1f)] private float backWardsMultiplier = 0.5f;
    [SerializeField] [Range(0.5f,1f)] private float strafeMultiplier = 0.9f;
    [SerializeField] private float jumpForce = 10f;
    public Vector3 LookAtRayHit{get; private set;}
    private CharacterController controller;
    private Camera cam;
    private Animator anim;

    [Header("Gravity Values")]
    [SerializeField] private float gravity = 1f;
    private float gravityAcceleration;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Sound passosSound;
    [HideInInspector] public ReticulaFeedback reticula;

    private bool isCrouching;
    [SerializeField] private float crouchingSpeed = 3f;
    private float upwardsHeight;
    private Vector3 upwardsCenter;
    private Vector3 upwardsCamLocalPos;
    [SerializeField] private float crouchingHeight;
    [SerializeField] private Vector3 crouchingCenter;
    [SerializeField] private Vector3 crouchingCamLocalPos;
    [SerializeField] private Vector3 deathCamLocalPos;
    private RigBuilder rigBuilder;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        anim = GetComponentInChildren<Animator>();
        rigBuilder = GetComponentInChildren<RigBuilder>();
        UpdateIK();
        upwardsHeight = controller.height;
        upwardsCenter = controller.center;
        upwardsCamLocalPos = cam.transform.localPosition;
        //StartCoroutine(UpdateRigBuilder());
    }

    private void UpdateIK()
    {

    }

    IEnumerator UpdateRigBuilder()
    {
        yield return new WaitForSeconds(1f);
        rigBuilder.Build();
        StartCoroutine(UpdateRigBuilder());
    }

    private void Update()
    {
        if (!GameState.isGamePaused)
        {
            Movement();
            Animations();
            UpdateIK();
        }
        else
        {
            if(passosSound.audioSource != null)
            {
                passosSound.audioSource.Pause();
            }
        }
    }

    private void Movement()
    {
        MoveRotation();

        MoveInput();
    }

    private void Animations()
    {
        isCrouching = Input.GetButton("Crouch");
        anim.SetBool("crouching", isCrouching);

        
        if(GameState.IsPlayerDead)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, deathCamLocalPos, 0.1f);
            return;
        }

        if(isCrouching)
        {
            controller.height = Mathf.Lerp(controller.height, crouchingHeight, 0.25f);
            controller.center = Vector3.Lerp(controller.center, crouchingCenter, 0.25f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, crouchingCamLocalPos, 0.1f);
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, upwardsHeight, 0.25f);
            controller.center = Vector3.Lerp(controller.center, upwardsCenter, 0.25f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, upwardsCamLocalPos, 0.1f);
        }
    }
    

    void OnAnimatorIK()
    {

    }

    private void MoveRotation()
    {
        var camRotation = cam.transform.rotation;
        var objRotation = transform.rotation;
        Vector3 setRotation = new Vector3(objRotation.eulerAngles.x, camRotation.eulerAngles.y, objRotation.eulerAngles.z);
        transform.eulerAngles = setRotation;
    }

    private void MoveInput()
    {
        if(controller.isGrounded)
        {
            ungroudedTime = 0;
            gravityAcceleration = -gravity;
        } 
        else
        {
            ungroudedTime += Time.deltaTime;
            gravityAcceleration -= gravity * Time.deltaTime;
        } 

        if(ungroudedTime < 0.1f && !GameState.IsPlayerDead)
        {
            anim.SetBool("isJumping", false);

            if (Input.GetButtonDown("Jump"))
            {
                gravityAcceleration = jumpForce;
                anim.SetBool("isJumping", true);
            }
        }

        Vector3 vertical = Input.GetAxis("Vertical") * transform.forward;
        if(Input.GetAxis("Vertical") < 0) vertical = Input.GetAxis("Vertical") * transform.forward * backWardsMultiplier;

        Vector3 rawHorizontal = Input.GetAxis("Horizontal") * cam.transform.right;
        Vector3 horizontal = rawHorizontal * strafeMultiplier;

        movementInput = (vertical + horizontal).normalized;
        if(GameState.IsPlayerDead) movementInput = Vector3.zero;

        var hasMovingInput = (Vector3.Distance(movementInput, Vector3.zero) > 0.01);
        var isRuning = Input.GetButton("Sprint");
        anim.SetBool("isRuning", isRuning && hasMovingInput && !isCrouching);

        if(hasMovingInput && (isRuning || (!isRuning && currentSpeed < walkSpeed))) currentSpeed += (isRuning ? runAccelaration : runAccelaration * 2) * Time.deltaTime;
        
        if((currentSpeed > crouchingSpeed && isCrouching)
        || !hasMovingInput)
        {
            currentSpeed -= (lastInputSpeed > crouchingSpeed ? inerciaDeccalaration : inerciaDeccalaration * 3) * Time.deltaTime;
            var flag = isCrouching && hasMovingInput && currentSpeed > crouchingSpeed - 0.1f && currentSpeed < crouchingSpeed + 0.1f;
            if(flag) currentSpeed = crouchingSpeed;
        }
        else if((currentSpeed > walkSpeed && !isRuning))
        {
            currentSpeed -= (lastInputSpeed > walkSpeed ? inerciaDeccalaration : inerciaDeccalaration * 3) * Time.deltaTime;
            var flag = hasMovingInput && currentSpeed > walkSpeed - 0.1f && currentSpeed < walkSpeed + 0.1f;
            if(flag) currentSpeed = walkSpeed;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, runSpeed);

        if(hasMovingInput)
        {
            lastInputSpeed = currentSpeed;
            lastMovementInput = movementInput;
        }

        var movement = (hasMovingInput ? movementInput : lastMovementInput) * currentSpeed;
        movement.y += gravityAcceleration;
        movement = movement * Time.deltaTime;
        
        controller.Move(movement);
        
        var velocitylAbs = Mathf.Abs(vertical.magnitude) + Mathf.Abs(horizontal.magnitude);
        anim.SetFloat("Movement", velocitylAbs);

        anim.SetFloat("Velocidade", Mathf.Abs((vertical.magnitude * currentSpeed) / runSpeed));

        if(Input.GetAxis("Horizontal") >= 0) anim.SetFloat("Strafe", (horizontal.magnitude * currentSpeed) / runSpeed);
        else if(Input.GetAxis("Horizontal") < 0) anim.SetFloat("Strafe", (-horizontal.magnitude * currentSpeed) / runSpeed);


        if((velocitylAbs > 0.1) && controller.isGrounded && !GameState.isGamePaused)
        {
            if(passosSound.isPlaying == false && passosSound.clip)
            {
                passosSound.PlayOn(audioSource);
            }
            passosSound.audioSource.pitch = passosSound.pitch * currentSpeed / runSpeed;
        }
        else if(passosSound.audioSource != null)
        {
            passosSound.audioSource.Pause();
        }
    }

    public void GoToCheckPoint(Vector3 checkpoint)
    {

    }
}