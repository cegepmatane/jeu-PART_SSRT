using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlower : Activable
{
    [SerializeField]
    private int m_DarknessDecrease = 20;

    private GameObject m_player;

    protected override void Activate()
    {
        // TODO Utiliser GameManager
        m_player = GameObject.FindGameObjectWithTag("Player");

        RegainMana();
    }

    private void RegainMana()
    {
        if (m_player.GetComponent<PlayerAbilities>().decreaseDarkness(m_DarknessDecrease))
        {
            Destroy(gameObject);

        }


    }
}

