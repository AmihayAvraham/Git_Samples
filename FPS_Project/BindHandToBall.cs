using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindHandToBall : MonoBehaviour
{
    public ArmIK armIK;
    public BallController ballController;
    public BallHandler ballHandler;
    public float ballThrowDelay;

    private void Start()
    {
        ballHandler.onBallCaught.AddListener(OnBallCaught);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !ballController._caughtBall && Time.time - ballController._caughtBallTime >= ballThrowDelay)
        {
            SetBindArm(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            SetBindArm(false);
        }
    }

    private void OnBallCaught()
    {
        SetBindArm(false);
    }

    private void SetBindArm(bool value)
    {
        if (armIK != null)
            armIK.enabled = value;
    }
}
