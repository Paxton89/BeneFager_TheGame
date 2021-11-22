using UnityEngine;

public class Shot : MonoBehaviour
{
	public float initialForce;
	public ForceMode forceMode;
	public float damageToApply;
	public GameObject particleToSpawn;
	public float explosionForce;
	public ForceMode explosionForceMode;
	
	private Transform _tf;
	private Rigidbody _rb;
	
	private void Awake()
	{
		_tf = transform;
		_rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		_rb.AddForce(initialForce * _tf.forward, forceMode);
	}

	private void OnTriggerEnter(Collider other)
	{
		HealthComponent health = other.GetComponent<HealthComponent>();
		Rigidbody rb = other.GetComponent<Rigidbody>();

		if (!health)
			return;
		
		health.TakeDamage(damageToApply);

		if (rb && explosionForce > 0f)
		{
			rb.AddForce((other.transform.position - _tf.position).normalized * explosionForce, explosionForceMode);
		}
		
		if(particleToSpawn)
			Instantiate(particleToSpawn, _tf.position, _tf.rotation);
		
		Destroy(gameObject);
	}
}