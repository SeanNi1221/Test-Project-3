using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCheckTest : MonoBehaviour
{
    NullCheckTarget target;
    void Awake()
    {
        Debug.Log("Awake begin---");
        target = gameObject.AddComponent<NullCheckTarget>();
        Debug.Log("target added!");
        target?.IsAlive();
        Destroy(target);
        Debug.Log("target destroyed!");
    }
    void Start(){
        Debug.Log("Start begin---");
        Debug.Log("Trying '?'' method:");
        target?.IsAlive();
        Debug.Log("Trying 'is not null' method:");
        if (target is not null) target.IsAlive();
        Debug.Log("Trying regular method:");
        if (target != null) target.IsAlive();
    }
}
