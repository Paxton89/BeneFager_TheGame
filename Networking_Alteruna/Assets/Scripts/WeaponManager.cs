using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public WeaponBase[] weapons;

    [Header("Debug")]
    
    [SerializeField] private int _weaponIndex = 0;
    public GameObject UI_AmmoValue;
    private float _timeOfLastShot = -100f;

    private void Awake()
    {
        if (weapons.Length == 0)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (!UI_AmmoValue)
            return;
        
        if (_weaponIndex == 0)
        {
            var unlimited = "!";
            UI_AmmoValue.GetComponent<Text>().text = unlimited;
        }
        else
        {
            UI_AmmoValue.GetComponent<Text>().text = weapons[_weaponIndex].currentAmmo.ToString();   
        }
    }

    public void RecieveUpdate()
    {
        float time = Time.time;
        _weaponIndex += Math.Sign(Input.mouseScrollDelta.y);
        if (_weaponIndex != 0)
        {
            if (_weaponIndex < 0)
                _weaponIndex = weapons.Length - 1;

            _weaponIndex %= weapons.Length;

            if(UI_AmmoValue)
                if (_weaponIndex == 0)
                {
                    var unlimited = "!";
                    UI_AmmoValue.GetComponent<Text>().text = unlimited.ToString();
                }
                else
                {
                    UI_AmmoValue.GetComponent<Text>().text = weapons[_weaponIndex].currentAmmo.ToString();   
                }
            
            _timeOfLastShot = time;
        }

        if (Input.GetMouseButton(0))
        {
            WeaponBase weapon = weapons[_weaponIndex];
            if ((time - _timeOfLastShot > weapon.weaponCooldown && weapon.currentAmmo >= weapon.ammoConsumption) || _weaponIndex == 0 && time - _timeOfLastShot > weapon.weaponCooldown)
            {
                _timeOfLastShot = time;
                weapon.Shoot();
                if (_weaponIndex != 0)
                {
                    weapon.currentAmmo -= weapon.ammoConsumption;
                    if(UI_AmmoValue)
                        UI_AmmoValue.GetComponent<Text>().text = weapon.currentAmmo.ToString(); 
                }
            }
        }
    }
}
