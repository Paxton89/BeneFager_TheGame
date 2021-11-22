using System;
using System.Collections.Generic;
using Alteruna.Trinity;
using UnityEngine;

public class MoveOnInput : MonoBehaviour
{
    // This is now a manager, congratulations

    [SerializeField] private List<Transform> Players;

    private Transform myPlayer = null;
    
    
    
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myPlayer.transform.position += Vector3.up * 5;
        }
    }

    public void PlayerJoined(AlterunaTrinity instance, Session session, IDevice device, UInt16 id)
    {
        if (id > Players.Count)
        {
            Debug.LogError("Too many players joined");
            return;
        }
        myPlayer = Players[id];
    }
}
