using UnityEngine;

public class PepsiJetpack : MonoBehaviour
{
	public ParticleSystem[] pepsiThrusters;
	public float juiceInSeconds;
	public float jetpackAcceleration;

	private bool _thrusting = false;
	private Transform _tf;
	private PlayerMovementSync _player;

	private void Awake()
	{
		_tf = transform;
		_player = GetComponent<PlayerMovementSync>();
		PlayerInputManager.Instance.onUpdate += RecieveUpdate;
	}

	private void RecieveUpdate()
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
				_player.playerVelocity += _tf.up * (jetpackAcceleration * Time.deltaTime);
				
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
}