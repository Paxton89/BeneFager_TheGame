using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;
    public Text hpValue;

    private Quaternion deadRotation;
    private void Start()
    {
        deadRotation = new Quaternion(-90,0,0,0);
        currentHealth = maxHealth;
        hpValue = GameObject.Find("HP-Value").GetComponent<Text>();
    }
    
    public void TakeDamage(float damage)
    {
        int newHealth = (int)(currentHealth -= damage);
        Debug.Log("oof! " + gameObject.name + " took " + damage + " points of dmg!" );
        hpValue.text = newHealth.ToString();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        TakeDamage(1);
    }

    private void Die()
    {
        //Ragdoll here
        Debug.Log(gameObject.name + " DIED" );
        GetComponentInParent<PlayerMovementSync>().canMove = false;
    }

    public void OnRespawn()
    {
        currentHealth = maxHealth;
        GetComponentInParent<PlayerMovementSync>().canMove = true;
    }
}
