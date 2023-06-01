using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoMouse : MonoBehaviour
{
    public float sensibilidadeMouse = 100f;
    private float senX => GameState.SettingsData.sensibilidadeX;
    private float senY => GameState.SettingsData.sensibilidadeY;
    [SerializeField] private Vector2 maxXRotation = new Vector2(-80f, 80f);

    public Transform playerBody;
    public Transform playerHead;

    private float camRotationY;
    private Camera cam => Camera.main;
    [SerializeField] private ReticulaFeedback reticula;
    public Vector3 raycastResult {get; private set;}
    public const int kHorizonPoint = 150;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * senX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * senY * Time.deltaTime;

        camRotationY += mouseY;
        camRotationY = Mathf.Clamp(camRotationY, maxXRotation.x, maxXRotation.y);

        playerHead.localRotation = Quaternion.Euler(camRotationY,0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);

        GetTargetHealth();
    }

    static public int GetLayers(bool isPlayerCast = true)
    {
        var layer = isPlayerCast ? LayerMask.GetMask("Player") : LayerMask.GetMask("Enemy");
        return ~layer;
    }

    
    // public Vector3 GetRayCastMiddle()
    // {
    //     var layer = GetLayers();
        
    //     RaycastHit rayHit;

    //     if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, kHorizonPoint, layer))
    //     {
    //         Debug.DrawRay(cam.transform.position, cam.transform.forward * kHorizonPoint, Color.blue);
    //         // if(rayHit.rigidbody != null && rayHit.rigidbody.gameObject.CompareTag("Enemy") && reticula != null)
    //         // {
    //         //     reticula.EnemyState();
    //         // }
    //         // else if(reticula != null)
    //         // {
    //         //     reticula.NeutralState();
    //         // }
    //         return rayHit.point;
    //     }
    //     else
    //     {
    //         //Debug.DrawRay(cam.transform.position, cam.transform.forward * kHorizonPoint, Color.green);
    //         // if(reticula != null)
    //         // {
    //         //     reticula.NeutralState();
    //         // }
    //         return cam.transform.position + cam.transform.forward * kHorizonPoint;
    //     }
    // }

    public Health GetTargetHealth()
    {
        var layer = GetLayers();
        RaycastHit rayHit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, kHorizonPoint, layer))
        {
            raycastResult = rayHit.point;
            var curTransform = rayHit.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            var itemObj = curTransform.GetComponentInChildren<Item>();
            while (healthObj == null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
                itemObj = curTransform.GetComponent<Item>();
            }
            //Debug.Log("Found Health? " + (healthObj != null));

            if(healthObj != null) reticula.SetReticulaState(ReticulaState.Enemy);
            else if(itemObj != null) reticula.SetReticulaState(ReticulaState.Interactable);
            else reticula.SetReticulaState(ReticulaState.Neutral);
            return healthObj;
        }
        else
        {
            reticula.SetReticulaState(ReticulaState.Neutral);
            raycastResult = cam.transform.position + cam.transform.forward * kHorizonPoint;
            return null;
        }
    }
}
