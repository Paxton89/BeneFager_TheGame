using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;
    public GameObject hpValue;

	public GameObject KristerRagdoll;
	public MeshRenderer KristerRenderer;
	public Collider KristerCollider;
	
	private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        int newHealth = (int)(currentHealth -= damage);
        Debug.Log("oof! " + gameObject.name + " took " + damage + " points of dmg!" );

        if(hpValue)
			hpValue.GetComponent<Text>().text = newHealth.ToString();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
	    Debug.Log(gameObject.name + " DIED" );
        GetComponentInParent<PlayerMovementSync>().canMove = false;
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
