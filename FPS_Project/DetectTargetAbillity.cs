using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DetectTargetType
{
    FirstOnList,
    Closest
}

public class DetectTargetAbillity : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Keep Null to load it from UnitCharacter")]
    private UnitCharacterData characterData;
    [SerializeField]
    private LayerMask enemiesLayers;
    [SerializeField]
    private bool isAlwaysDetect = false;
    [SerializeField]
    private float timeBetweenTargetScan = 1f;
    [SerializeField]
    private DetectTargetType detectType = DetectTargetType.Closest;
    [SerializeField]
    private int maxTargetsToScan = 5;
    [SerializeField]
    private Transform overrideTransform;
    [Tooltip("My team")]
    [SerializeField]
    private UnitTeam team;
    [SerializeField]
    private bool showGizmo;

    public UnityEvent<UnitHealth> OnTargetDetected = new UnityEvent<UnitHealth>();

    public UnitHealth CurrentTarget { get; private set; }

    public bool IsEnabled 
    { 
        get
        {
            if (isAlwaysDetect)
                return true;

            return isEnabled;
        }
    }

    private float lastScan;
    private Collider[] hits;
    private UnitHealth targetInRange;
    private Transform myTransform;
    private UnitCharacter unitCharacter;
    private bool isEnabled = false;
    private UnitHealth targetTopProirity;

    private bool isInitialized = false;

    private void OnEnable()
    {
        if (!isInitialized)
            InitOnce();

        Init();
    }

    private void InitOnce()
    {
        if (overrideTransform != null)
            myTransform = overrideTransform;
        else
            myTransform = transform;

        unitCharacter = GetComponent<UnitCharacter>();

        if (unitCharacter != null)
        {
            characterData = unitCharacter.CharacterData;
        }

        hits = new Collider[10];

        isInitialized = true;
    }

    public void Init()
    {
        CurrentTarget = null;
        targetTopProirity = null;
        isEnabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInitialized)
            return;

        if (IsEnabled)
        {
            if (Time.time - lastScan >= timeBetweenTargetScan)
                ScanForTarget();
        }
    }

    public void SetDetectState(bool enabled)
    {
        lastScan = -1;
        isEnabled = enabled;
    }

    public void SetTopPriorityTarget(UnitHealth targetHealth)
    {
        //targetTopProirity = targetHealth;
    }

    private void ScanForTarget()
    {
        lastScan = Time.time;
        UnitHealth targetUnit = null;

        if (targetTopProirity != null &&
           Vector3.Distance(myTransform.position, targetTopProirity.GetPosition()) <= characterData.DetectRange)
        {
            targetUnit = targetTopProirity;
        }
        else
        {
            float minDistance = 100;
            int targetsScaned = 0;

            /*hits = Physics.OverlapSphere(myTransform.position, characterData.DetectRange, enemiesLayers);

            if (hits.Length <= 0)
                return;*/

            int numOfCollisions = Physics.OverlapSphereNonAlloc(myTransform.position, characterData.DetectRange, hits, enemiesLayers);
            if (numOfCollisions <= 0)
                return;

            for (int i = 0; i < numOfCollisions; i++)
            {
                if (WaveManager.Instance.UnitsByColliders.TryGetValue(hits[i], out targetInRange) &&           
            /*foreach (Collider hit in hits)
            {
                if (WaveManager.Instance.UnitsByColliders.TryGetValue(hit, out targetInRange) &&*/
                    targetInRange != null &&
                    targetInRange.Team != this.team && 
                    targetInRange.IsAlive())
                {
                    if (detectType == DetectTargetType.FirstOnList)
                    {
                        targetUnit = targetInRange;

                        break;
                    }
                    else if (detectType == DetectTargetType.Closest)
                    {
                        float curDistance = Vector3.Distance(myTransform.position, targetInRange.GetPosition());
                        if (curDistance < minDistance)
                        {
                            minDistance = curDistance;
                            targetUnit = targetInRange;
                        }

                        targetsScaned++;

                        if (targetsScaned >= maxTargetsToScan)
                            break;
                    }
                }
            }
        }

        if (targetUnit != null)
        {
            CurrentTarget = targetUnit;
            OnTargetDetected.Invoke(CurrentTarget);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (characterData != null && showGizmo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, characterData.DetectRange);
        }
    }
}
