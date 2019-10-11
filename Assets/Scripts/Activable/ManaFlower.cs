using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : Activable
{
    private int m_manaCount = 12;

    protected override void Activate()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerAbilities>().addMana(m_manaCount);

        Destroy(gameObject);

        Debug.Log("Player gained mana!!");
    }
}
