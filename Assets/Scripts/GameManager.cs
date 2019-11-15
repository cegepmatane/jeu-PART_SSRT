using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int DarknessPerDefeat = 20;
    [SerializeField]
    private float DarkModeTransitionTime = 5;
    [SerializeField]
    private GameObject Prefab_PlayerModel, m_GlowyAmbience;
    private List<GameObject> m_Trees = new List<GameObject>();
    private GameObject m_PlayerSpawner;
    private bool m_DarkModeActivated = false;
    private GameObject m_Player;
    private GameObject m_ManaFlowerContainer;
    private float m_regenRate = 0.1f;
    private GameObject m_FlowerSpawner;
    private Color m_InitialGlowyColor;
    private float m_InitialGlowyIntensity;
    private float m_InitialFogdensity;
    private Color m_InitialSkyColor;
    //public List<Transform> m_Waypoints;

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
            m_FlowerSpawner.GetComponent<SpawnerBounds>().Spawn(t_FlowerAmountDifference);
        }

        if (m_DarkModeActivated)
        {
            if (!m_Player.GetComponent<PlayerAbilities>().decreaseDarkness(0.1f))
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
        if(a_GenericSpawner.GetComponent<SpawnerBounds>().ItemType == SpawnerBounds.ItemTypeArray.MANAFLOWER)
        {
            m_FlowerSpawner = a_GenericSpawner;
        }

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

            //TEST D'AMBIANCE
            //Activation du fog (rougeâtre et dense)
            RenderSettings.fog = true;
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
        
        while (t_ElapsedTime / a_Duration < 1)
        {
            float t = t_ElapsedTime / a_Duration;
            if (!IsInDarkMode)
            {
                t = 1 - t;
                
            }
            //Debug.Log("/" + t_ElapsedTime + "/" + a_Duration + "/" + t + "/");
            t_GlowyLight.intensity = Mathf.Lerp(m_InitialGlowyIntensity, 0.2f, t);
            t_GlowyLight.color = Color.Lerp(m_InitialGlowyColor, new Color32(3, 240, 252, 1), t);
            RenderSettings.fogDensity = Mathf.Lerp(m_InitialFogdensity, 0.1f, t);
            
            RenderSettings.ambientSkyColor = Color.Lerp(m_InitialSkyColor, new Color32(40,40,40,1), t);
       
            t_ElapsedTime += Time.deltaTime;
            yield return null;
        }
        
        
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

    /*
    public List<Transform> Waypoints
    {
        get
        {
            return m_Waypoints;
        }
    } 
    */
}
