using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public WeaponBase[] weapons;

    [Header("Debug")]
    
    [SerializeField] private int _weaponIndex = 0;
    private float _timeOfLastShot = -100f;

    private void Awake()
    {
        if (weapons.Length == 0)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        float time = Time.time;
        _weaponIndex += Math.Sign(Input.mouseScrollDelta.y);
        if (_weaponIndex != 0)
        {
            if (_weaponIndex < 0)
                _weaponIndex = weapons.Length - 1;

            _weaponIndex %= weapons.Length;

            _timeOfLastShot = time;
        }

        if (Input.GetMouseButton(0))
        {
            WeaponBase weapon = weapons[_weaponIndex];
            if (time - _timeOfLastShot > weapon.weaponCooldown)
            {
                _timeOfLastShot = time;
                weapon.Shoot();
            }
        }
    }
}
