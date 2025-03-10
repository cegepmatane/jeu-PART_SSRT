﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public class Spell
    {
        private GameObject m_Prefab;
        private int m_Cost;
        private Vector3 m_BasePosition;
        private float m_Duration;
        

        public Spell(GameObject a_Prefab, int a_Cost, float a_Duration)
        {
            m_Prefab = a_Prefab;
            m_Cost = a_Cost;
            m_Duration = a_Duration;
        }

        public GameObject Prefab
        {
            get { return m_Prefab; }
        }

        public int Cost
        {
            get { return m_Cost; }
        }

        public Vector3 BasePosition
        {
            get { return m_BasePosition; }
            set { m_BasePosition = value; }
        }

        public float Duration
        {
            get { return m_Duration; }
        }
    }

    private const int MAX_MANA = 100;
    private const int MAX_DARKNESS = 100;
    private int m_Mana;
    private float m_Darkness;
    public int MaxNumberOfLives = 3;
    private int m_NumberOfLives;
    private float m_CastingCooldown;
    private int m_SelectedCost;

    //SPELLS
    //private bool m_IsMousePressed;
    private List<Spell> m_Spells;
    private Spell m_SelectedSpell;

    //Spells gameobjects
    public GameObject Fireball;
    public GameObject LightningStrike;
    public GameObject Illuminate;
    public GameObject Shockwave;

    private Camera m_camera;
    //private Text m_UiText;
    private GameObject m_UiManaBar;
    private GameObject m_UiDarknessBar;
    private GameObject m_UiHurtScreen;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip m_FireballSound;
    [SerializeField] private AudioClip m_LightningStrikeSound;
    [SerializeField] private AudioClip m_IlluminateSound;

    private bool m_Damageable = true;

    private void Start()
    {
        m_Mana = MAX_MANA;
        m_Darkness = 0;
        m_NumberOfLives = MaxNumberOfLives;
        m_Spells = new List<Spell>();
        //Construction de la liste de sorts
        m_Spells.Add(new Spell(Fireball, 10, 1));
        m_Spells.Add(new Spell(LightningStrike, 20, 2));
        m_Spells.Add(new Spell(Illuminate, 10, 2));
        m_Spells.Add(new Spell(Shockwave,30, 2));

        //Le premier sort est sélectionné par défaut
        m_SelectedSpell = m_Spells[0];
        m_SelectedCost = m_SelectedSpell.Cost;

        m_camera = GetComponentInChildren<Camera>();
        //m_UiText = transform.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>();
        m_UiManaBar = transform.Find("UIGame").transform.Find("ManaBar").gameObject;
        m_UiDarknessBar = transform.Find("UIGame").transform.Find("DarknessBar").gameObject;
        m_UiHurtScreen = transform.Find("UIGame").transform.Find("HurtScreen").gameObject;

        m_Audio = GetComponentInChildren<AudioSource>();

    }

    private void Update()
    {
        //Laisse un délai entre chaque sort
        m_CastingCooldown -= Time.deltaTime;
        if (m_CastingCooldown < 0)
        {
            //bool mouseWasPressed = m_IsMousePressed;
            //if (Input.GetButtonDown("Fire1") && mouseWasPressed)
            //m_IsMousePressed = true;

            //Lancer le sort sélectionné
            if (/*m_IsMousePressed &&*/ Input.GetButtonUp("Fire1") && m_Mana >= m_SelectedCost)
            {

                switch (m_SelectedSpell.Prefab.name)
                {
                    case "Fireball":
                        GetComponentInChildren<Animator>().SetTrigger("FireballCast");
                        m_Audio.clip = m_FireballSound;
                        break;
                    case "LightningStrike":
                        GetComponentInChildren<Animator>().SetTrigger("LightningStrikeCast");
                        m_Audio.clip = m_LightningStrikeSound;
                        break;
                    case "Illuminate":
                        GetComponentInChildren<Animator>().SetTrigger("IlluminateCast");
                        m_Audio.clip = m_IlluminateSound;
                        break;
                    case "Shockwave":
                        GetComponentInChildren<Animator>().SetTrigger("ShockwaveCast");
                        break;
                }
                Debug.LogWarning(m_SelectedCost);
                m_Audio.PlayOneShot(m_Audio.clip);

                //
                //A un certain point de l'animation du sort, un Animation Event appelle la fonction InstantiateSpell
                //
                //m_IsMousePressed = false;
                m_CastingCooldown = m_SelectedSpell.Duration;
            }

            SpellChoice();
        }
        updateUI();
    }

    private void SpellChoice()
    {
        //A AMELIORER
        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_SelectedSpell = m_Spells[0];
            Debug.Log("BOULE DE FEU");
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            m_SelectedSpell = m_Spells[1];
            Debug.Log("ECLAIR");
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            m_SelectedSpell = m_Spells[2];
            Debug.Log("LUMIERE");
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            m_SelectedSpell = m_Spells[3];
            Debug.Log("ONDE DE CHOC");
        }

        m_SelectedCost = m_SelectedSpell.Cost;

    }

    public void InstantiateSpell(Vector3 a_SpellPos)
    {
        m_SelectedSpell.BasePosition = a_SpellPos;
        //Instantiation
        Instantiate(m_SelectedSpell.Prefab, m_SelectedSpell.BasePosition, m_SelectedSpell.Prefab.name == "Shockwave" ? Quaternion.Euler(-90, 0, 0) : Quaternion.LookRotation(m_camera.transform.forward));
        //Retrait du mana
        m_Mana -= m_SelectedCost;
    }

    private void updateUI()
    {
        m_UiManaBar.GetComponent<RectTransform>().localScale = new Vector3((float)m_Mana / MAX_MANA, 1, 1);
        m_UiDarknessBar.GetComponent<RectTransform>().localScale = new Vector3(1, (float)m_Darkness / MAX_DARKNESS, 1);
       
        float t_OpacityLevel = 255 - (m_NumberOfLives * 85);
        if(t_OpacityLevel != 0)
        {
            t_OpacityLevel = 1 / (255 / t_OpacityLevel);
        }
        Color t_color = m_UiHurtScreen.GetComponent<RawImage>().color;
        
        m_UiHurtScreen.GetComponent<RawImage>().color = new Color(t_color.r,t_color.g,t_color.b, t_OpacityLevel);
        //Debug.Log(m_UiHurtScreen.GetComponent<RawImage>().color);
    }
    //Si ceci retourne FALSE, cela veut dire que la mana est au maximum!
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
    //Si ceci retourne FALSE, cela veut dire que la darkness est au maximum!
    public bool increaseDarkness(float a_darkness)
    {
        if (m_Darkness < MAX_DARKNESS)
        {
            float darknessDiff = MAX_DARKNESS - m_Darkness;
            m_Darkness += (darknessDiff > a_darkness) ? a_darkness : darknessDiff;
            if (m_Darkness == MAX_DARKNESS)
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
    //Si ceci retourne FALSE, cela veut dire qu'il n'y a plus de darkness restante!
    public bool decreaseDarkness(float a_darkness)
    {

        if (m_Darkness > 0)
        {

            m_Darkness -= (m_Darkness > a_darkness) ? a_darkness : m_Darkness;
            if (m_Darkness == 0)
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

    private void OnTriggerEnter(Collider collider)
    {
        if(GameManager.Instance.ShadowEntity != null)
        {
            if (collider == GameManager.Instance.ShadowEntity.GetComponent<BoxCollider>() && m_Damageable)
            {
                Debug.Log("CONTACT!");
                GameManager.Instance.ShadowEntity.GetComponent<ShadowEnemyController>().TriggerAttack();
                m_Damageable = false;
                int t_NewLivesNumber = NumberOfLives - 1;
                if (t_NewLivesNumber < 0)
                {
                    GameManager.Instance.Defeat();
                }
                else
                {
                    NumberOfLives = t_NewLivesNumber;
                }

                GameManager.Instance.EndShadowCycle();
            }
        }
        
    }

    

    public int NumberOfLives
    {
        get { return m_NumberOfLives; }
        set { m_NumberOfLives = value; }
    }

    public bool Damageable
    {
        get { return m_Damageable; }
        set { m_Damageable = value; }
    }
}
