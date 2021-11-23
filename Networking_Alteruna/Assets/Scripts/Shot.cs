using System.Collections;
using UnityEngine;

public class Shot : MonoBehaviour
{
	public float initialForce;
	public ForceMode forceMode;
	public float damageToApply;
	public GameObject particleToSpawn;
	public float explosionForce;
	public ForceMode explosionForceMode;
	public float explosionDelay = 0.5f;

	[HideInInspector] public ObjectPool _pool;
	
	private Transform _tf;
	private Rigidbody _rb;
	
	private void Awake()
	{
		_tf = transform;
		_rb = GetComponent<Rigidbody>();
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		_rb.AddForce(initialForce * _tf.forward, forceMode);
		StartCoroutine(ExplodeRoutine());
	}

	private IEnumerator ExplodeRoutine()
	{
		yield return new WaitForSeconds(explosionDelay);
		Explode();
	}

	private void OnCollisionEnter(Collision other)
	{
		HealthComponent health = other.collider.GetComponent<HealthComponent>();
		Rigidbody rb = other.collider.GetComponent<Rigidbody>();

		if (!health)
			return;

		health.TakeDamage(damageToApply);

		if (rb && explosionForce > 0f)
		{
			rb.AddForce((other.contacts[0].point - _tf.position).normalized * explosionForce, explosionForceMode);
		}

		Explode();
	}

	public void Explode()
	{
		if(particleToSpawn)
			Instantiate(particleToSpawn, _tf.position, _tf.rotation);
		
		StopAllCoroutines();
		_rb.velocity = Vector3.zero;
		_pool.ReturnObject(gameObject);
		gameObject.SetActive(false);
	}
}