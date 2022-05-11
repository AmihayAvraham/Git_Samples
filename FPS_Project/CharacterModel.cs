using FIMSpace;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using static RootMotion.FinalIK.IKSolver;

public class CharacterModel : MonoBehaviour
{
    public Avatar animAvatar;
    public Transform rootUpperBody;
    public Transform centerOfMass;
    public Weapon weapon;
    public GameObject grenade;
    public Renderer[] renderers;
    public AnimatorController animatorController;

    public GameObject[] blades;

    [Header("AimIK")]
    public bool useAimIK;
    public Transform aimTransform;
    public Bone[] bones;

    [Header("LeaningAnimator")]
    public bool useLeaningAnimator;
    public LeaningProcessor leanParams;

    [Header("ArmIK")]
    public bool useArmIK;
    public ArmIK armIK;
}
