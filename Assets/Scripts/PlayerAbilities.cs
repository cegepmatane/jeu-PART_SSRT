using System.Collections;
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

        public Spell(GameObject a_Prefab, int a_Cost)
        {
            m_Prefab = a_Prefab;
            m_Cost = a_Cost;
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
    }

    private const int MAX_MANA = 100;
    private const int MAX_DARKNESS = 100;
    private int m_Mana;
    private int m_Darkness;

    private float m_CastingCooldown;
    private int m_SelectedCost;

    //SPELLS
    //private bool m_IsMousePressed;
    private List<Spell> m_Spells;
    private Spell m_SelectedSpell;

    //Spells gameobjects
    public GameObject Fireball;
    public GameObject LightningStrike;

    private Camera m_camera;
    //private Text m_UiText;
    private GameObject m_UiManaBar;
    private GameObject m_UiDarknessBar;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip m_FireballSound;
    [SerializeField] private AudioClip m_LightningStrikeSound;


    private void Start()
    {
        m_Mana = MAX_MANA;
        m_Darkness = 0;

        m_Spells = new List<Spell>();
        //Construction de la liste de sorts
        m_Spells.Add(new Spell(Fireball, 10));
        m_Spells.Add(new Spell(LightningStrike, 30));
    


        //Le premier sort est sélectionné par défaut
        m_SelectedSpell = m_Spells[1];
        m_SelectedCost = m_SelectedSpell.Cost;
        m_SelectedCost = m_SelectedSpell.Cost;

        m_camera = GetComponentInChildren<Camera>();
        //m_UiText = transform.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>();
        m_UiManaBar = transform.Find("Canvas").transform.Find("ManaBar").gameObject;
        m_UiDarknessBar = transform.Find("Canvas").transform.Find("DarknessBar").gameObject;

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

                switch(m_SelectedSpell.Prefab.name)
                {
                    case "Fireball":
                        GetComponentInChildren<Animator>().SetTrigger("FireballCast");
                        m_Audio.clip = m_FireballSound;
                        break;
                    case "LightningStrike":
                        Debug.Log(m_SelectedSpell.Prefab.name);
                        GetComponentInChildren<Animator>().SetTrigger("LightningStrikeCast");
                        m_Audio.clip = m_LightningStrikeSound;
                        break;
                }
                m_Audio.PlayOneShot(m_Audio.clip);
                //
                //A un certain point de l'animation du sort, un Animation Event appelle la fonction InstantiateSpell
                //
                //m_IsMousePressed = false;
                m_CastingCooldown = 1f;
            }
        }

        updateUI();
    }

    private void SpellChoice()
    {
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

    }

    public void InstantiateSpell(Vector3 a_SpellPos)
    {
        m_SelectedSpell.BasePosition = a_SpellPos;
        //Instantiation
        Instantiate(m_SelectedSpell.Prefab, m_SelectedSpell.BasePosition, Quaternion.LookRotation(m_camera.transform.forward));
        //Retrait du mana
        m_Mana -= m_SelectedCost;
    }

    private void updateUI()
    {
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
