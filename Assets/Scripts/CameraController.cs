using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;

    Vector3 offset;
    void Start()
    {
        FindPlayer();
        offset = player.transform.position - transform.position;
    }

    
    void LateUpdate()
    {
        transform.position = player.transform.position - offset;
    }

    void FindPlayer()
    {
        player = GameObject.Find("Player");
    }
}
