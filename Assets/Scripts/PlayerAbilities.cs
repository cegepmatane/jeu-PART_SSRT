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
    //private Text m_UiText;
    private GameObject m_UiManaBar;
    private GameObject m_UiDarknessBar;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip m_FireballAppear;
    [SerializeField] private AudioClip m_FireballReleased;

    private void Start()
    {
        m_Mana = MAX_MANA;
        m_Darkness = 0;

        m_camera = GetComponentInChildren<Camera>();
        //m_UiText = transform.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>();
        m_UiManaBar = transform.Find("Canvas").transform.Find("ManaBar").gameObject;
        m_UiDarknessBar = transform.Find("Canvas").transform.Find("DarknessBar").gameObject;
        m_Audio = GetComponentInChildren<AudioSource>();
    }

    private void FixedUpdate()
    {
        //Laisse un délai entre chaque sort
        m_castingCooldown -= Time.deltaTime;
        if (m_castingCooldown < 0)
        {
            //Clic gauche de souris
            if (Input.GetButtonDown("Fire1"))
            {
                //m_Audio.clip = m_FireballAppear;
                //m_Audio.Play();
            }

            if (Input.GetButtonUp("Fire1") && m_Mana >= m_spikeCost)
            {
                GetComponentInChildren<Animator>().SetTrigger("FireballCast");
                m_Audio.clip = m_FireballReleased;
                m_Audio.Play();
                Instantiate(m_spikeSpell, transform.position, Quaternion.LookRotation(m_camera.transform.forward));
                m_Mana -= m_spikeCost;

                m_castingCooldown = 1f;
            }
        }

        updateUI();
    }

    private void updateUI()
    {
        //m_UiText.text = "Mana : " + m_Mana;
        
        m_UiManaBar.GetComponent<RectTransform>().localScale = new Vector3((float)m_Mana / MAX_MANA, 1, 1);
        m_UiDarknessBar.GetComponent<RectTransform>().localScale = new Vector3(1, (float)m_Darkness / MAX_DARKNESS, 1);
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
            if(m_Darkness == MAX_DARKNESS)
            {
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }

    }
}
