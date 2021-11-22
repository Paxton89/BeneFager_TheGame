using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

	public GameObject Ragdoll;

    private Quaternion deadRotation;
    private void Start()
    {
        deadRotation = new Quaternion(-90,0,0,0);
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
		//Ragdoll here
		var spawnedRagdoll = Instantiate(Ragdoll, transform.position, transform.rotation);
        Debug.Log(gameObject.name + " DIED" );


    }

    private void OnMouseDown()
    {
        TakeDamage(10);
    }
}
