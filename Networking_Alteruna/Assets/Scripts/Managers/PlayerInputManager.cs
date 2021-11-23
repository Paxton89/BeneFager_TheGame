using System;
using Alteruna.Trinity;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    public Vector3 spawnPos;

    public PlayerMovementSync MyPlayer { get; private set; }
    public WeaponManager MyWeapons { get; private set; }
    public UInt16 MyPlayerId { get; private set; }
    
    public Action onPlayerJoined;
    public Action onUpdate;

    private SpawnerSynchronizable _playerSpawner;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _playerSpawner = GetComponent<SpawnerSynchronizable>();
    }

    void Update()
    {
        if (MyPlayer && MyWeapons)
        {
            MyPlayer.RecieveUpdate();
            MyWeapons.RecieveUpdate();
            onUpdate?.Invoke();
        } 
    }

    public void PlayerJoined(AlterunaTrinity instance, Session session, IDevice device, UInt16 id)
    {
        MyPlayerId = id;
        _playerSpawner.Spawn(spawnPos, Quaternion.identity, Vector3.one);
        MyPlayer = _playerSpawner.GetObjects()[id].GetComponent<PlayerMovementSync>();
        MyWeapons = MyPlayer.gameObject.GetComponentInChildren<WeaponManager>();
        MyPlayer.OnJoin();
        onPlayerJoined?.Invoke();
    }
}
