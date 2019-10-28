using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    private const int MAX_MANA = 100;
    private const int MAX_DARKNESS = 100;
    private int m_Mana;
    private int m_Darkness;

    private float m_castingCooldown;

    public GameObject m_spikeSpell;
    private int m_spikeCost = 10;

    private Camera m_camera;
    private Text m_UiText;

    private void Start()
    {
        m_Mana = MAX_MANA;
        m_Darkness = MAX_DARKNESS;

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
                if (m_Mana >= m_spikeCost)
                {
                    Instantiate(m_spikeSpell, transform.position, Quaternion.LookRotation(m_camera.transform.forward));
                    m_Mana -= m_spikeCost;

                    m_castingCooldown = 1f;
                }
            }
        }

        updateUI();
    }

    private void updateUI()
    {
        m_UiText.text = "Mana : " + m_Mana;
    }

    public bool addMana(int a_mana)
    {   
        if (m_Mana < MAX_MANA)
        {
            int manaDiff = MAX_MANA - m_Mana;
            m_Mana += (manaDiff > a_mana) ? a_mana : manaDiff;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool increaseDarkness(int a_darkness)
    {
        if (m_Darkness < MAX_DARKNESS)
        {
            int darknessDiff = MAX_DARKNESS - m_Darkness;
            m_Darkness += (darknessDiff > a_darkness) ? a_darkness : darknessDiff;
            return true;
        }
        else
        {
            return false;
        }

    }
}
