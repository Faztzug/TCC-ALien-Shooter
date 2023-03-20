using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    [Header("Character Values")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float runAccelaration = 3f;
    private float currentSpeed;
    [SerializeField] [Range(0.5f,1f)] private float backWardsMultiplier = 0.5f;
    [SerializeField] [Range(0.5f,1f)] private float strafeMultiplier = 0.9f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] [Range(0,1)] private float weightIKhand;
    private Transform lookAtObj;
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
    private PlayerIK ik;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
        anim = GetComponentInChildren<Animator>();
        lookAtObj = cam.transform.GetChild(0);
        ik = GetComponentInChildren<PlayerIK>();
        UpdateIK();
    }

    private void UpdateIK()
    {
        if(ik == null) return;
        ik.lookAtObj = lookAtObj;
        ik.weightIKhand = weightIKhand;
        ik.LookAtRayHit = LookAtRayHit;
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
        //limitando a rotacao da cabeca
        Vector3 frente = transform.forward;
        Vector3 direcaoAlvo = lookAtObj.transform.position - transform.position;
        float angulo = Vector3.Angle(frente, direcaoAlvo);

        if(LookAtRayHit != Vector3.zero)
        {
            anim.SetLookAtPosition(LookAtRayHit);
            anim.SetIKPosition(AvatarIKGoal.RightHand, LookAtRayHit);
        }
        else
        {
            anim.SetLookAtPosition(lookAtObj.position);
            anim.SetIKPosition(AvatarIKGoal.RightHand, lookAtObj.position);
        }
        

        if (angulo < 70 )
        {
            anim.SetLookAtWeight(1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIKhand);
        }
        else
        {
            anim.SetLookAtWeight(1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIKhand);
        }
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
        Vector3 vertical = Input.GetAxis("Vertical") * transform.forward;
        if(Input.GetAxis("Vertical") < 0) vertical = Input.GetAxis("Vertical") * transform.forward * backWardsMultiplier;
        Vector3 rawHorizontal = Input.GetAxis("Horizontal") * cam.transform.right;
        Vector3 horizontal = rawHorizontal * strafeMultiplier;

        if(controller.isGrounded)
        {
            gravityAcceleration = 0f;
            anim.SetBool("isJumping", false);

            if (Input.GetButtonDown("Jump"))
            {
                gravityAcceleration = jumpForce;
                anim.SetBool("isJumping", true);
            }
            else gravityAcceleration = -gravity * 10f * Time.deltaTime;

        }
        else
        {
            gravityAcceleration -= gravity * Time.deltaTime;
        }

        Vector3 movement = (vertical + horizontal) * Time.deltaTime;
        if(Input.GetButton("Sprint")) currentSpeed += runAccelaration * Time.deltaTime;
        else currentSpeed -= runAccelaration * Time.deltaTime;

        if(currentSpeed < speed) currentSpeed = speed;
        else if(currentSpeed > runSpeed) currentSpeed = runSpeed;

        movement = movement * currentSpeed;

        movement.y = gravityAcceleration * Time.deltaTime * speed;
        
        controller.Move(movement);
        
        var velocitylAbs = Mathf.Abs(vertical.magnitude) + Mathf.Abs(horizontal.magnitude);
        anim.SetFloat("Movement", velocitylAbs);

        anim.SetFloat("Velocidade", Mathf.Abs((vertical.magnitude * currentSpeed) / runSpeed));

        if(Input.GetAxis("Horizontal") >= 0) anim.SetFloat("Strafe", (horizontal.magnitude * currentSpeed) / runSpeed);
        else if(Input.GetAxis("Horizontal") < 0) anim.SetFloat("Strafe", (-horizontal.magnitude * currentSpeed) / runSpeed);


        if((velocitylAbs > 0.1) && controller.isGrounded)
        {
            if(audioSource.isPlaying == false)
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