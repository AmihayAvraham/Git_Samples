using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow3DObject : MonoBehaviour
{
    public Camera cameraUI;
    public Transform toFollow;

    void FixedUpdate()
    {
        transform.position = cameraUI.WorldToScreenPoint(toFollow.transform.position);
    }
}
