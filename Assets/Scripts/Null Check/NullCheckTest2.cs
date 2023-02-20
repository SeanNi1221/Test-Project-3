using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCheckTest2 : MonoBehaviour
{
    GameObject nonSerialized;
    public GameObject Serialized;
    void Awake() {
        Debug.Log(nonSerialized is null);
        //Output:"True", As expected.

        Debug.Log(nonSerialized?.transform);
        //Output:"Null", As expected.

        Debug.Log(Serialized is null);
        //In editor output:"False"
        //In build output:"True"
        
        Debug.Log(Serialized?.transform);
        //In editor output: UnassignedReferenceException
        //In build output: "Null"
    }
    void Start(){
        // AssertionTest();
    }
    GameObject _cameraObj;
    Camera _camera;
    string _cameraName;

    void SyntaxExample(){
        _cameraObj = GameObject.Find(_cameraName);
        _camera = _cameraObj ? _cameraObj.GetComponent<Camera>() : _camera;
        if(_cameraObj) _camera = _cameraObj.GetComponent<Camera>();
        Debug.Assert(_camera);
    }

    void AssertionTest(){
        Debug.Log("Starting AssertionTest...");
        Debug.Assert(nonSerialized != null);
        Debug.Log("AssertionTest ended");
    }
}
