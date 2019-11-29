using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlower : Activable
{
    [SerializeField]
    private int m_ManaAdd = 50;

    private GameObject m_player;

    protected override void Activate()
    {
        m_player = GameManager.Instance.Player;
        RegainMana();
    }

    private void RegainMana()
    {
        if (m_player.GetComponent<PlayerAbilities>().addMana(m_ManaAdd))
        {
            Destroy(gameObject);
            Debug.Log("Player gained mana!!");
        }

        
    }
}
