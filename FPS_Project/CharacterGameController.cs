using FIMSpace;
using MoreMountains.TopDownEngine;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGameController : MonoBehaviour
{
    [SerializeField]
    private CharacterModel currentModel;

    private Animator animator;
    private AimIK aimIk;
    private AimManager aimManager;
    private Health health;
    private LeaningAnimator leaningAnimator;

    public CharacterModel CurrentModel
    {
        get { return currentModel; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        aimIk = GetComponent<AimIK>();
        aimManager = GetComponent<AimManager>();
        health = GetComponent<Health>();
        leaningAnimator = GetComponent<LeaningAnimator>();

        if (currentModel.animatorController != null)
            animator.runtimeAnimatorController = currentModel.animatorController;
        
        animator.avatar = currentModel.animAvatar;

        aimManager.rootUpperBody = currentModel.rootUpperBody;
        aimManager.initialWeapon = currentModel.weapon;

        if (currentModel.useArmIK)
            aimManager.ArmIK = currentModel.armIK;

        health.CenterOfMass = currentModel.centerOfMass;

        aimIk.enabled = currentModel.useAimIK;
        if (currentModel.useAimIK)
        {
            EnableAimIK();
        }

        /*leaningAnimator.enabled = currentModel.useLeaningAnimator;
        if (currentModel.useLeaningAnimator)
            leaningAnimator.SetLeaningParams(currentModel.leanParams);*/
    }

    private void EnableAimIK()
    {
        aimIk.solver.transform = currentModel.aimTransform;
        aimIk.solver.bones = currentModel.bones;
    }
}
