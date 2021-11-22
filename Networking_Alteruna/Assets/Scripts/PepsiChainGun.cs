using UnityEngine;

[RequireComponent(typeof(SpawnerSynchronizable))]
public class PepsiChainGun : PhysicsWeapon
{
	public Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);

	private SpawnerSynchronizable _spawner;

	protected virtual void Awake()
	{
		base.Awake();
		_spawner = GetComponent<SpawnerSynchronizable>();
	}

	public override void Shoot()
	{
		base.Shoot();
		_spawner.Spawn(weaponOutput.position, Quaternion.LookRotation(weaponOutput.forward, weaponOutput.up), scale);
	}
}