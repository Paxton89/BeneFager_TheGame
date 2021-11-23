using UnityEngine;

[RequireComponent(typeof(SpawnerSynchronizable))]
public class PepsiChainGun : PhysicsWeapon
{
	public Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);

	private SpawnerSynchronizable _spawner;
	private ObjectPool _pool;

	protected virtual void Awake()
	{
		base.Awake();
		_spawner = GetComponent<SpawnerSynchronizable>();
		_pool = GetComponent<ObjectPool>();
	}

	public override void Shoot()
	{
		base.Shoot();
		GameObject shot = _pool.GetObject();
		shot.transform.position = weaponOutput.position;
		shot.transform.rotation = Quaternion.LookRotation(weaponOutput.forward, weaponOutput.up);
		shot.transform.localScale = scale;
		shot.SetActive(true);
	}
}