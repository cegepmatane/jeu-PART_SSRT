using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int DarknessPerDefeat = 20;
    [SerializeField]
    private float DarkModeTransitionTime = 100;
    [SerializeField]
    private GameObject Prefab_PlayerModel, m_GlowyAmbience;
    private List<GameObject> m_Trees = new List<GameObject>();
    private GameObject m_PlayerSpawner;
    private bool m_DarkModeActivated = false;
    private GameObject m_Player;
    private GameObject m_ManaFlowerContainer;
    private float m_regenRate = 0.1f;
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
        if(m_ManaFlowerContainer.transform.childCount < FlowerSpawner.Instance.InitialSpawnAmount)
        {
            FlowerSpawner.Instance.Spawn();
            Debug.Log("Nouvelle fleur sur la map!");
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
        StartCoroutine(RegenerateTree(a_Tree));

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
            RenderSettings.fogColor = new Color32(15, 0, 0, 1);
  
            //m_ShadowAmbience.transform.eulerAngles = new Vector3(-80, 0, 0);
            StartCoroutine(ShadowTransition(DarkModeTransitionTime));
            
            //t_GlowyLight.color = new Color32(235, 52, 52, 1);
            
        }
        
    }

    private IEnumerator ShadowTransition(float a_Duration)
    {
        
        Light t_GlowyLight = m_GlowyAmbience.GetComponent<Light>();
        Color t_GlowColor = new Color32(235, 52, 52, 1);
        float t_ElapsedTime = 0;
        //Debug.Log("Intensité brouillard : " + RenderSettings.fogDensity);
        
        while (t_ElapsedTime / a_Duration < 1)
        {
            float t = t_ElapsedTime / a_Duration;
            if (!m_DarkModeActivated)
            {
                t = 1 - t;
            }
            t_GlowyLight.intensity = Mathf.Lerp(t_GlowyLight.intensity, 0.2f, t);
            t_GlowyLight.color = Color.Lerp(t_GlowyLight.color, new Color32(3, 240, 252, 1), t);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.1f, t);
            //RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, -4f, t_elapsedTime / a_Duration);
            RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, new Color32(40,40,40,1), t);
            //m_ShadowAmbience.transform.rotation = Quaternion.Lerp(m_ShadowAmbience.transform.rotation, Quaternion.Euler(new Vector3(-20,-180,0)) , t_elapsedTime / a_Duration);
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
