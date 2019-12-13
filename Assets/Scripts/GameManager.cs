using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int DarknessPerDefeat = 20;
    [SerializeField]
    private float DarkModeTransitionTime = 5, m_DarknessDissipationRate = 0.1f;
    [SerializeField]
    private GameObject Prefab_PlayerModel, m_GlowyAmbience;
    private List<GameObject> m_Trees = new List<GameObject>();
    private List<GameObject> m_EnemySpawners = new List<GameObject>();
    private GameObject m_PlayerSpawner, m_ShadowSpawner;
    private bool m_DarkModeActivated = false;
    private GameObject m_Player, m_ShadowEntity, m_LightFlower = null;
    private GameObject m_ManaFlowerContainer;
    private float m_regenRate = 0.1f;
    private GameObject m_FlowerSpawner;
    private Color m_InitialGlowyColor;
    private float m_InitialGlowyIntensity;
    private float m_InitialFogdensity;
    private Color m_InitialSkyColor;
    private AudioSource m_MusicSource;
    [SerializeField] private AudioClip m_DarkTheme;

    public static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
                Debug.LogError("GameManager has no instance.");

            return m_Instance;
        }
    }

    public bool DarkModeActivated
    {
        get
        {
            return m_DarkModeActivated;
        }
    }

    public GameObject ContainerManaFlower
    {
        get { return m_ManaFlowerContainer; }
    }

    public GameObject LightFlower
    {
        get { return m_LightFlower; }
    }

    void Awake()
    {
        m_Instance = this;
        m_ManaFlowerContainer = GameObject.Find("ManaFlowers");
        if(m_ManaFlowerContainer == null)
        {
            Debug.LogError("\"ManaFlowers\" is missing!");
        }
        if(m_GlowyAmbience == null)
        {
            Debug.LogError("GlowyAmbience is missing!");
        }
        m_InitialGlowyColor = m_GlowyAmbience.GetComponent<Light>().color;
        m_InitialGlowyIntensity = m_GlowyAmbience.GetComponent<Light>().intensity;
        m_InitialSkyColor = RenderSettings.ambientSkyColor;
        m_InitialFogdensity = RenderSettings.fogDensity;

        m_MusicSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (m_PlayerSpawner != null)
        {
            m_Player = Instantiate(Prefab_PlayerModel, m_PlayerSpawner.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No Player Spawner Found");
        }

        //m_Waypoints = new List<Transform>();
       
        GameObject t_FirstTree = null;
        GameObject t_SecondTree = null;
        GameObject t_ThirdTree = null;
        
        //On réordone les arbres pour savoir lequel sera le premier et lesquels suivent, 
        //car on ne peut prévoir dans quel ordre ils s'auto réfèrent au Manager
        for (int i = 0; i < m_Trees.Count; i++)
        {
            //Debug.Log("Ordre de l'arbre :" + m_Trees[i].GetComponent<TreeHealth>().Order);
            if (m_Trees[i].GetComponent<TreeHealth>().Order == 0)
            {
                t_FirstTree = m_Trees[i];
            } else if(m_Trees[i].GetComponent<TreeHealth>().Order == 1)
            {
                t_SecondTree = m_Trees[i];
            }
            else if (m_Trees[i].GetComponent<TreeHealth>().Order == 2)
            {
                t_ThirdTree = m_Trees[i];
            }

        }
        m_Trees.Clear();
        m_Trees.Add(t_FirstTree);
        m_Trees.Add(t_SecondTree);
        m_Trees.Add(t_ThirdTree);
            //Debug.Log("Montant de waypoints : " + m_Waypoints.Count);
            
        
        /*
        for (int i = 0; i < m_Waypoints.Count; i++)
        {
            Transform temp = m_Waypoints[i];
            int randomIndex = Random.Range(i, m_Waypoints.Count);
            m_Waypoints[i] = m_Waypoints[randomIndex];
            m_Waypoints[randomIndex] = temp;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        int t_FlowerAmountDifference = m_FlowerSpawner.GetComponent<SpawnerBounds>().InitialSpawnAmount - m_ManaFlowerContainer.transform.childCount;
        if (t_FlowerAmountDifference > 0)
        {
            m_FlowerSpawner.GetComponent<SpawnerBounds>().RandomSpawn(t_FlowerAmountDifference);
        }

        if (m_DarkModeActivated)
        {
            if (!m_Player.GetComponent<PlayerAbilities>().decreaseDarkness(m_DarknessDissipationRate))
            {
                ReturnFromShadowRealm();
            }
        }
    }

    public void AddTree(GameObject a_Tree)
    {
        m_Trees.Add(a_Tree);
    }

    public List<GameObject> TreeList
    {
        get
        {
            return m_Trees;
        }
    }

    public void AddPlayerSpawner(GameObject a_PlayerSpawner)
    {
        m_PlayerSpawner = a_PlayerSpawner;
    }

    public void AddGenericSpawner(GameObject a_GenericSpawner)
    {
        switch (a_GenericSpawner.GetComponent<SpawnerBounds>().ItemType)
        {
            case SpawnerBounds.ItemTypeArray.MANAFLOWER:
                m_FlowerSpawner = a_GenericSpawner;
                break;
            case SpawnerBounds.ItemTypeArray.ENEMY:
                m_EnemySpawners.Add(a_GenericSpawner);

                break;
            case SpawnerBounds.ItemTypeArray.SHADOW:
                m_ShadowSpawner = a_GenericSpawner;
                break;
        }       
    }

    public void AddShadowEntity(GameObject a_ShadowEntity)
    {
        if(m_ShadowEntity != null)
        {
            m_ShadowEntity.GetComponent<ShadowEnemyController>().DisappearingSequence();
        }
        m_ShadowEntity = a_ShadowEntity;
    }

    public void AddLightFlower(GameObject a_LightFlower)
    {
        m_LightFlower = a_LightFlower;
    }

    //Appelez cette fonction à chaque fois que le joueur se fait toucher par le shadow enemy, OU qu'il ramasse une lightflower
    public void EndShadowCycle()
    {
        m_ShadowEntity.GetComponent<ShadowEnemyController>().DisappearingSequence();
        m_LightFlower.GetComponent<LightFlower>().DisappearingSequence();
        StartCoroutine(WaitForEnemyToDisappear());

    }

    

    private IEnumerator WaitForEnemyToDisappear()
    {
        while(m_ShadowEntity != null)
        {
            yield return null;
        }
        m_ShadowSpawner.GetComponent<SpawnerBounds>().FixedSpawn(1, 0);
        m_ShadowSpawner.GetComponent<SpawnerBounds>().FixedSpawn(1, 1);
        yield break; 
    }

    public void KillTree(GameObject a_Tree)
    {
        //Debug.Log("Test1 :" + m_Waypoints.Count);
        //m_Waypoints.RemoveAt(0);
        //Debug.Log("Test2 :" + m_Waypoints.Count);
        Debug.Log("L'arbre est mort et se regénère!");

        //Pour tester le ShadowRealm, mettez la valeur en paramètre à 100 (ou plus) pour le trigger après le premier échec!
        if (!m_Player.GetComponent<PlayerAbilities>().increaseDarkness(DarknessPerDefeat))
        {
            InitiateShadowRealm();
        }
        //StartCoroutine(RegenerateTree(a_Tree));
    }

    private void InitiateShadowRealm()
    {
        if (!m_DarkModeActivated)
        {
            Debug.Log("Spooky time");
            m_DarkModeActivated = true;

            //Jouer le dark theme
            m_MusicSource.clip = m_DarkTheme;
            m_MusicSource.loop = true;
            m_MusicSource.volume = 0.2f;
            m_MusicSource.Play();

            //TEST D'AMBIANCE
            //Activation du fog (rougeâtre et dense)
            //RenderSettings.fog = true;
            //RenderSettings.fogColor = new Color32(15, 0, 0, 1);
  
            //m_ShadowAmbience.transform.eulerAngles = new Vector3(-80, 0, 0);
            StartCoroutine(ShadowTransition(DarkModeTransitionTime, true));
            
            //t_GlowyLight.color = new Color32(235, 52, 52, 1);
            
        }
        
    }

    private void ReturnFromShadowRealm()
    {
        if (m_DarkModeActivated)
        {
            Debug.Log("Vous vous échappez de la noirceur...");
            m_Player.GetComponent<PlayerAbilities>().NumberOfLives = m_Player.GetComponent<PlayerAbilities>().MaxNumberOfLives;
            m_ShadowEntity.GetComponent<ShadowEnemyController>().DisappearingSequence();
            m_LightFlower.GetComponent<LightFlower>().DisappearingSequence();
            m_DarkModeActivated = false;

            
            //RenderSettings.fog = false;
            //RenderSettings.fogColor = new Color32(15, 0, 0, 1);

            
            StartCoroutine(ShadowTransition(DarkModeTransitionTime, false));

            

        }
    }

    private IEnumerator ShadowTransition(float a_Duration, bool IsInDarkMode)
    {
        
        
        //Color t_GlowColor = new Color32(235, 52, 52, 1);
        float t_ElapsedTime = 0;
        Light t_GlowyLight = m_GlowyAmbience.GetComponent<Light>();
        //Debug.Log("Intensité brouillard : " + RenderSettings.fogDensity);
        if (!IsInDarkMode)
        {
            if (m_ShadowEntity)
            {
                m_ShadowEntity.GetComponent<ShadowEnemyController>().DisappearingSequence();
                m_LightFlower.GetComponent<LightFlower>().DisappearingSequence();
            }
        }
        while (t_ElapsedTime / a_Duration <= 1)
        {
            float t = t_ElapsedTime / a_Duration;
            if (!IsInDarkMode)
            {
                t = 1 - t;
                
            }
            //Debug.Log("/" + t_ElapsedTime + "/" + a_Duration + "/" + t + "/");
            t_GlowyLight.intensity = Mathf.Lerp(m_InitialGlowyIntensity, 0.12f, t);
            t_GlowyLight.color = Color.Lerp(m_InitialGlowyColor, new Color32(3, 240, 252, 1), t);
            //RenderSettings.fogDensity = Mathf.Lerp(m_InitialFogdensity, 0.25f, t);

            RenderSettings.ambientSkyColor = Color.Lerp(m_InitialSkyColor, new Color32(20,20,20,1), t);
       
            t_ElapsedTime += Time.deltaTime;
            yield return null;
        }
        if (IsInDarkMode)
        {
            m_ShadowSpawner.GetComponent<SpawnerBounds>().FixedSpawn(1, 0);
            m_ShadowSpawner.GetComponent<SpawnerBounds>().FixedSpawn(1, 1);
        }
        WaveManager.Instance.PrepareNextWave();
        
        
        yield break;
    }
    public IEnumerator RegenerateTree(GameObject a_Tree)
    {
        
        TreeHealth t_TreeHealth = a_Tree.GetComponent<TreeHealth>();
        t_TreeHealth.IsHealing = true;
        while (t_TreeHealth.IsHurt) 
        {
            t_TreeHealth.HealDamage();
            yield return new WaitForSeconds(m_regenRate);
        }
        yield break;

    }
    
    public void Victory()
    {
        Debug.Log("Toute les vagues ont été détruites! Ceci est un message placeholder! GG");
        m_Player.GetComponent<FirstPersonController>().enabled = false;
        m_Player.transform.Find("UIGame").gameObject.SetActive(false);
        m_Player.transform.Find("UIGameEnd").gameObject.SetActive(true);
        //m_Player.transform.Find("EndingMessage").GetComponent<GUIText>().text = "Victory!";
        //m_Player.transform.Find("EndingMessageShadow").GetComponent<GUIText>().text = "Victory!";
        m_Player.GetComponent<PlayerAbilities>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Defeat()
    {
        m_Player.GetComponent<FirstPersonController>().enabled = false;
        m_Player.transform.Find("UIGame").gameObject.SetActive(false);
        m_Player.transform.Find("UIGameEnd").gameObject.SetActive(true);
        m_Player.GetComponent<PlayerAbilities>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Vous avez échoué...");
    }

    public void ReviveTree(GameObject a_Tree)
    {
        Debug.Log("Arbre ressucité!");
        a_Tree.GetComponent<TreeHealth>().IsHealing = false;
        //m_Waypoints.Add(a_Tree.transform.GetChild(0));
        StopCoroutine(RegenerateTree(a_Tree));
    }

    public GameObject Player
    {
        get { return m_Player; }
    }
    public GameObject ShadowEntity
    {
        get { return m_ShadowEntity; }     
    }


    public List<GameObject> EnemySpawners
    {
        get { return m_EnemySpawners; }
    }

    public AudioSource MusicPlayer
    {
        get { return m_MusicSource; }
    }
}
