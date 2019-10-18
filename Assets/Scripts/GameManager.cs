using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject PlayerModel;
    private List<GameObject> m_Trees = new List<GameObject>();
    private GameObject m_PlayerSpawner;
    private GameObject m_Player;
    private float regenRate = 0.1f;
    public List<Transform> m_Waypoints;

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

    void Awake()
    {
        m_Instance = this;
    }
    void Start()
    {
        if (m_PlayerSpawner != null)
        {
            m_Player = Instantiate(PlayerModel, m_PlayerSpawner.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("No Player Spawner Found");
        }

        m_Waypoints = new List<Transform>();
        bool Completion = false;
        
        while (!Completion)
        {
            
            for(int i = 0; i < m_Trees.Count; i++)
            {
                //Debug.Log("Ordre de l'arbre :" + m_Trees[i].GetComponent<TreeHealth>().Order);
                if (m_Trees[i].GetComponent<TreeHealth>().Order == m_Waypoints.Count)
                {
                    Debug.Log("AJOUT!");
                    m_Waypoints.Add(m_Trees[i].transform.GetChild(0));
                } 
                
            }
            //Debug.Log("Montant de waypoints : " + m_Waypoints.Count);
            if(m_Waypoints.Count == m_Trees.Count)
            {
                Completion = true;
            }
        }
        
        for (int i = 0; i < m_Waypoints.Count; i++)
        {
            Transform temp = m_Waypoints[i];
            int randomIndex = Random.Range(i, m_Waypoints.Count);
            m_Waypoints[i] = m_Waypoints[randomIndex];
            m_Waypoints[randomIndex] = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTree(GameObject a_Tree)
    {
        m_Trees.Add(a_Tree);
    }

    public void AddPlayerSpawner(GameObject a_PlayerSpawner)
    {
        m_PlayerSpawner = a_PlayerSpawner;
    }

    public void KillTree(GameObject a_Tree)
    {
        Debug.Log("Test1 :" + m_Waypoints.Count);
        m_Waypoints.RemoveAt(0);
        Debug.Log("Test2 :" + m_Waypoints.Count);
        Debug.Log("L'arbre est mort et se regénère!");
        StartCoroutine(RegenerateTree(a_Tree));

    }

    private IEnumerator RegenerateTree(GameObject a_Tree)
    {
        TreeHealth t_TreeHealth = a_Tree.GetComponent<TreeHealth>();
        while (t_TreeHealth.IsHurt()) 
        {
            t_TreeHealth.HealDamage();
            yield return new WaitForSeconds(regenRate);
        }
    }
    public void ReviveTree(GameObject a_Tree)
    {
        Debug.Log("Abre ressucité!");
        m_Waypoints.Add(a_Tree.transform.GetChild(0));
        StopCoroutine(RegenerateTree(a_Tree));
    }

    public List<Transform> Waypoints
    {
        get
        {
            return m_Waypoints;
        }
    }
}
