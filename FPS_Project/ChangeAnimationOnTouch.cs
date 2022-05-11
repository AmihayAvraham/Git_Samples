using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimationOnTouch : MonoBehaviour
{
    public string targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == targetTag)
        {
            other.gameObject.GetComponent<PresetAnimation>().DeathButton = true;
        }
    }
}
