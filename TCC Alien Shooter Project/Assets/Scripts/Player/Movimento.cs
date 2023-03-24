using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private AudioClip passosClip;
    [HideInInspector] public ReticulaFeedback reticula;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        anim = GetComponentInChildren<Animator>();
        UpdateIK();
        //Application.targetFrameRate = 124;
    }

    private void UpdateIK()
    {

    }

    private void Update()
    {
        if (!GameState.isGamePaused)
        {
            Movement();
            Animations();
            LookAtRayHit = GetRayCastMiddle();
            UpdateIK();
        }
    }

    private void Movement()
    {
        MoveRotation();

        MoveInput();
    }

    private void Animations()
    {
        
    }
    private Vector3 GetRayCastMiddle()
    {
        var layer = 1 << 3;
        layer = ~layer;
        
        RaycastHit rayHit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 500f, layer))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward * 500f, Color.red);
            if(rayHit.rigidbody != null && rayHit.rigidbody.gameObject.CompareTag("Enemy") && reticula != null)
            {
                reticula.EnemyState();
            }
            else if(reticula != null)
            {
                reticula.NeutralState();
            }
            return rayHit.point;
        }
        else
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward * 500f, Color.gray);
            if(reticula != null)
            {
                reticula.NeutralState();
            }
            return Vector3.zero;
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

        if(ungroudedTime < 0.1f)
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

        var hasMovingInput = (Vector3.Distance(movementInput, Vector3.zero) > 0.01);
        var isRuning = Input.GetButton("Sprint");

        if(hasMovingInput && (isRuning || (!isRuning && currentSpeed < walkSpeed))) currentSpeed += (isRuning ? runAccelaration : runAccelaration * 2) * Time.deltaTime;
        
        if((currentSpeed > walkSpeed & !isRuning)
        || !hasMovingInput)
        {
            currentSpeed -= (lastInputSpeed > walkSpeed ? inerciaDeccalaration : inerciaDeccalaration * 3) * Time.deltaTime;
            if(!isRuning && hasMovingInput && currentSpeed > walkSpeed - 0.1f && currentSpeed < walkSpeed + 0.1f) currentSpeed = walkSpeed;
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


        if((velocitylAbs > 0.1) && controller.isGrounded)
        {
            if(audioSource.isPlaying == false && passosClip)
            {
                audioSource.PlayOneShot(passosClip);
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void GoToCheckPoint(Vector3 checkpoint)
    {

    }
}