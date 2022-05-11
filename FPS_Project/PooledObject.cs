using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField]
    private int duration;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("Disable", duration);
    }

    public void Disable()
    {
        if (this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
