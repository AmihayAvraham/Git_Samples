using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMouse : MonoBehaviour
{
    public float xSpeed = 1;
    public float ySpeed = 1;

    public float rotateSpeed = 1;

    public float speed = 1;
    public bool toRotate;

    public Transform player;
    public Rigidbody rb;


    float xInput;
    float yInput;

    private void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Mouse X");
        yInput = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3.right * xInput * xSpeed) + (Vector3.up * yInput * ySpeed), speed);

        if (toRotate)
            player.Rotate(0, xInput * rotateSpeed, 0);

        //rb.MoveRotation(new Quaternion(0, xInput * rotateSpeed, 0, 0));
    }
}
