using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Transform weaponOutput;
    public float weaponCooldown;
    public int maxAmmo;
    public int ammoConsumption;
    [NonSerialized] public int currentAmmo;

    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public abstract void Shoot();
}
