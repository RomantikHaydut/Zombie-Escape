using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;

    Transform mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("CameraPoint").transform;
        SpawnNewWeapon();
    }

    void SpawnNewWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

        currentWeapon.SpawnNewWeapon(mainCamera.GetChild(0).GetChild(0));

    }
}
