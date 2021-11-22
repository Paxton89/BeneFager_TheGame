using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void TakeDamage(float damage)
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
        Debug.Log(gameObject.name + " DIED" );
    }

    private void OnMouseDown()
    {
        TakeDamage(10);
    }
}
