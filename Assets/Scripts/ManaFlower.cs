using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : Activable
{
    [SerializeField]
    private int m_ManaAdd = 12;

    private GameObject m_player;

    protected override void Activate()
    {
        // TODO Utiliser GameManager
        m_player = GameObject.FindGameObjectWithTag("Player");

        RegainMana();
    }

    private void RegainMana()
    {
        m_player.GetComponent<PlayerAbilities>().addMana(m_ManaAdd);
        Destroy(gameObject);

        Debug.Log("Player gained mana!!");
    }
}
