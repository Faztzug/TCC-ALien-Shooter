using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoMouse : MonoBehaviour
{
    public float sensibilidadeMouse = 100f;
    [SerializeField] private Vector2 maxXRotation = new Vector2(-90f, 90f);

    public Transform playerBody;
    public Transform playerHead;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    int lastRot;
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;


        var xRot = Mathf.Clamp(mouseY, maxXRotation.x, maxXRotation.y);
        
        if(lastRot != (int)xRot) Debug.Log(xRot);
        lastRot = (int)xRot;

        playerHead.localRotation = Quaternion.Euler(xRot,0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
