using System;
using System.Collections.Generic;
using Alteruna.Trinity;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    
    [SerializeField] private List<PlayerMovementSync> players;

    private PlayerMovementSync myPlayer;
    private WeaponManager myWeapons;
    private Action onPlayerJoined;
    private Action onUpdate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (myPlayer && myWeapons)
        {
            myPlayer.RecieveUpdate();
            myWeapons.RecieveUpdate();
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
        myPlayer = players[id];
        myWeapons = players[id].gameObject.GetComponentInChildren<WeaponManager>();
        myPlayer.OnJoin();
        onPlayerJoined?.Invoke();
    }
}
