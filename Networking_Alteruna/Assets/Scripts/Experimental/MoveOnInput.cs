using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnInput : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position += Vector3.up * 5;
        }
    }
}
