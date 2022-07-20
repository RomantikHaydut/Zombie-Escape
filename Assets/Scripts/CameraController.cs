using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;

    void Start()
    {
        FindTarget();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void LateUpdate()
    {
        if (target!=null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

    void FindTarget()
    {
        target = GameObject.Find("CameraPoint").transform;
    }
}
