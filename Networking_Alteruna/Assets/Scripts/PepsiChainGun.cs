using UnityEngine;

[RequireComponent(typeof(SpawnerSynchronizable))]
public class PepsiChainGun : PhysicsWeapon
{
	public GameObject pepsi;

	private SpawnerSynchronizable _spawner;

	private void Awake()
	{
		_spawner = GetComponent<SpawnerSynchronizable>();
		_spawner.SpawnableObject = pepsi;
	}

	public override void Shoot()
	{
		base.Shoot();
		_spawner.Spawn(weaponOutput.position, Quaternion.LookRotation(weaponOutput.forward, weaponOutput.up), Vector3.one);
	}
}