using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTargeting : MonoBehaviour
{
    [SerializeField]
    private Transform[] targets;

    public Transform GetRandomTarget()
    {
        return targets[Random.Range(0, targets.Length)];
    }
}
