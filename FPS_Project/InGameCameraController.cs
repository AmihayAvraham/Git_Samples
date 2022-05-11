using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCameraController : MonoBehaviour
{
    [SerializeField]
    private float minDistanceToTarget;
    [SerializeField]
    private float cameraSpeed;
    [SerializeField]
    private Transform targetPos;
    [SerializeField]
    private Transform posGrid;

    [SerializeField]
    private Vector3 rightDir;
    [SerializeField]
    private Vector3 leftDir;

    private void Start()
    {
        targetPos.position = transform.position;
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, targetPos.position);

        if (distance >= minDistanceToTarget)
            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, Mathf.Min(cameraSpeed, distance));
    }

    public void MoveLeft()
    {
        targetPos.localPosition += rightDir;
    }

    public void MoveRight()
    {
        targetPos.localPosition += leftDir;
    }

    public void MoveToGrid()
    {
        targetPos.position = posGrid.position;
    }
}
