using System;
using System.Collections.Generic;
using Alteruna.Trinity;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private List<PlayerMovementSync> players;

    private PlayerMovementSync myPlayer;
    private WeaponManager myWeapons;


    void Update()
    {
        if (myPlayer && myWeapons)
        {
            myPlayer.RecieveUpdate();
            myWeapons.RecieveUpdate();
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
    }
}
