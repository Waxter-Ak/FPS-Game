using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots = 0.1f;
    public int maxAmmoPerGun = 25;
    public float reloadTime = 2f;
    public int currAmmo;

    private void Start()
    {
        currAmmo = maxAmmoPerGun;
    }
}
