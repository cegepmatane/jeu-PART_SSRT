using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : Activable
{
    protected override void Activate()
    {
        Destroy(gameObject);

        //TODO
        Debug.Log("Player gained mana!!");
    }
}
