using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.AddPlayerSpawner(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
