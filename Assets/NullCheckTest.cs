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
    IEnumerator Start(){
        Debug.Log("Start begin---");
        yield return new WaitForSeconds(1);
        Debug.Log("Trying '?'' method:");
        target?.IsAlive();
        Debug.Log("Trying 'is not null' method:");
        if (target is not null) target.IsAlive();
        Debug.Log("Trying regular method:");
        if (target != null) target.IsAlive();
        yield return null;
    }
}
