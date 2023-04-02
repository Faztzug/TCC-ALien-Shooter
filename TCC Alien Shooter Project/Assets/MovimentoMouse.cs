using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoMouse : MonoBehaviour
{
    public float sensibilidadeMouse = 100f;
    [SerializeField] private Vector2 maxXRotation = new Vector2(-80f, 80f);

    public Transform playerBody;
    public Transform playerHead;

    private float camRotation;
    private Camera cam => Camera.main;
    [HideInInspector] public ReticulaFeedback reticula;
    public Vector3 raycastResult {get; private set;}

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        camRotation += mouseY;
        camRotation = Mathf.Clamp(camRotation, maxXRotation.x, maxXRotation.y);

        playerHead.localRotation = Quaternion.Euler(camRotation,0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);

        raycastResult = GetRayCastMiddle();
    }

    private int GetLayers()
    {
        var layer = 1 << 3;
        return ~layer;
    }

    
    public Vector3 GetRayCastMiddle()
    {
        var layer = GetLayers();
        
        RaycastHit rayHit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 500f, layer))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.forward * 500f, Color.blue);
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
            //Debug.DrawRay(cam.transform.position, cam.transform.forward * 500f, Color.red);
            if(reticula != null)
            {
                reticula.NeutralState();
            }
            return Vector3.zero;
        }
    }

    public Health GetTargetHealth()
    {
        var layer = GetLayers();
        RaycastHit rayHit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 500f, layer))
        {
            var curTransform = rayHit.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            while (healthObj != null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
            }
            return healthObj;
        }
        else
        {
            return null;
        }
    }
}
