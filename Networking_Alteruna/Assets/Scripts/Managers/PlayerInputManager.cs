using System;
using System.Collections.Generic;
using Alteruna.Trinity;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    
    [SerializeField] private List<PlayerMovementSync> players;

    public PlayerMovementSync MyPlayer { get; private set; }
    public WeaponManager MyWeapons { get; private set; }
    public UInt16 MyPlayerId { get; private set; }
    
    public Action onPlayerJoined;
    public Action onUpdate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
        if (id > players.Count)
        {
            Debug.LogError("Too many players joined");
            return;
        }

        MyPlayerId = id;
        MyPlayer = players[id];
        MyWeapons = MyPlayer.gameObject.GetComponentInChildren<WeaponManager>();
        MyPlayer.OnJoin();
        onPlayerJoined?.Invoke();
    }
}
