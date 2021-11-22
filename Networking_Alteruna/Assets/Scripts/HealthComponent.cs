using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

	public GameObject KristerRagdoll;
	public MeshRenderer KristerRenderer;
	public Collider KristerCollider;

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
        //GetComponentInParent<PlayerMovementSync>().canMove = false;
		ActivateRagdoll();

    }

    public void OnRespawn()
    {
        currentHealth = maxHealth;
        GetComponentInParent<PlayerMovementSync>().canMove = true;
		DeactivateRagdoll();
    }

	private void ActivateRagdoll()
	{
		KristerRagdoll.transform.position = transform.position;
		KristerRagdoll.SetActive(true);
		KristerRenderer.enabled = false;
		KristerCollider.enabled = false;
	}

	private void DeactivateRagdoll()
	{
		KristerRagdoll.SetActive(false);
		KristerRenderer.enabled = true;
		KristerCollider.enabled = true;
	}
    private void OnMouseDown()
    {
        //TakeDamage(10);
    }
}
