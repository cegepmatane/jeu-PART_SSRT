using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    private const int  MAX_MANA = 100;
    private int m_mana;

    private float m_castingCooldown;

    public GameObject m_spikeSpell;
    private int m_spikeCost = 10;

    private Camera m_camera;
    private Text m_UiText;

    private void Start()
    {
        m_mana = MAX_MANA;

        m_camera = GetComponentInChildren<Camera>();
        m_UiText = transform.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        //Laisse un délai entre chaque sort
        m_castingCooldown -= Time.deltaTime;
        if (m_castingCooldown < 0)
        {
            //Clic gauche de souris
            if (Input.GetButton("Fire1"))
            {
                if (m_mana >= m_spikeCost)
                {
                    Instantiate(m_spikeSpell, transform.position, Quaternion.LookRotation(m_camera.transform.forward));
                    m_mana -= m_spikeCost;

                    m_castingCooldown = 1f;
                }
            }
        }

        updateUI();
    }

    private void updateUI()
    {
        m_UiText.text = "Mana : " + m_mana;
    }

    public bool addMana(int a_mana)
    {   if(m_mana == MAX_MANA)
        {
            return false;
        }
        if (m_mana < MAX_MANA)
        {
            int manaDiff = MAX_MANA - m_mana;
            m_mana += (manaDiff > a_mana) ? a_mana : manaDiff;
        }
        return true;
    }
}
