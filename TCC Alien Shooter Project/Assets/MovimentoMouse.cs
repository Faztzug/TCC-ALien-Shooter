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
    }
}
