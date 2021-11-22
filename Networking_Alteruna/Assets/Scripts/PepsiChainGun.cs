using UnityEngine;

public class PepsiChainGun : PhysicsWeapon
{
	public GameObject pepsi;

	public override void Shoot()
	{
		base.Shoot();
		GameObject currentBullet = Instantiate(pepsi, weaponOutput.position, Quaternion.LookRotation(weaponOutput.forward, weaponOutput.up));
	}
}