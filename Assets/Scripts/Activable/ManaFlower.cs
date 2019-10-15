using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : Activable
{
    private int m_manaAdd = 12;

    private GameObject m_player;

    protected override void Activate()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");

        RegainMana();
    }

    private void RegainMana()
    {
        m_player.GetComponent<PlayerAbilities>().addMana(m_manaAdd);
        Destroy(gameObject);

        Debug.Log("Player gained mana!!");
    }
}
