using System;
using System.Collections.Generic;
using Alteruna.Trinity;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private List<PlayerMovementSync> players;

    private PlayerMovementSync myPlayer;
    

    void Update()
    {
        if (myPlayer)
        {
            myPlayer.RecieveUpdate();
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
        myPlayer.OnJoin();
    }
}
