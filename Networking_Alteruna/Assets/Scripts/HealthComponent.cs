using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

    private Quaternion deadRotation;
    private void Start()
    {
        deadRotation = new Quaternion(-90,0,0,0);
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("oof! " + gameObject.name + " took " + damage + " points of dmg!" );
        if (currentHealth <= 0)
        {
            Die();
        }
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
    private void OnMouseDown()
    {
        //TakeDamage(10);
    }
}
