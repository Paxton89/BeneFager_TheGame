using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Transform weaponOutput;
    public float weaponCooldown;

    public abstract void Shoot();
}
