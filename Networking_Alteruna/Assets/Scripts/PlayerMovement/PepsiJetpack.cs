using UnityEngine;

public class PepsiJetpack : MonoBehaviour
{
	public ParticleSystem[] pepsiThrusters;
	public float juiceInSeconds;
	public float jetpackForce;
	public ForceMode jetpackForceMode;

	private bool _thrusting = false;
	private Rigidbody _rb;
	private Transform _tf;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_tf = transform;
	}

	public void Update()
	{
		if (juiceInSeconds <= 0f)
		{
			_thrusting = false;
			return;
		}
		
		if (Input.GetKey(KeyCode.Space))
		{
			if (_thrusting)
			{
				juiceInSeconds -= Time.deltaTime;
				return;
			}

			_thrusting = true;
			foreach (ParticleSystem system in pepsiThrusters)
			{
				system.Play();
			}
		}
		else if(_thrusting)
		{
			_thrusting = false;
			foreach (ParticleSystem system in pepsiThrusters)
			{
				system.Stop();
			}
		}
	}

	private void FixedUpdate()
	{
		if (_thrusting && _rb)
		{
			_rb.AddForce(_tf.up * jetpackForce, jetpackForceMode);
		}
	}
}