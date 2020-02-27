using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    
    
    [Serializable]
    public class Wave
    {
        private int m_PositionNumber;
        public enum PossibleTrees { FirstTree, SecondTree, ThirdTree};
        [SerializeField]
        private int m_NumberOfSkeletons, m_NumberOfGolems, m_NumberOfSwarmers;
        public PossibleTrees FocusedTree;
        private GameObject m_TargetTree;
        private bool m_IsActive = false;


        public int NumberOfSkeletons
        {
            get
            {
                return m_NumberOfSkeletons;
            }
        }
        public int NumberOfGolems
        {
            get
            {
                return m_NumberOfGolems;
            }
        }
        public int NumberOfSwarmers
        {
            get
            {
                return m_NumberOfSwarmers;
            }
        }

        public int PositionNumber
        {
            get
            {
                return m_PositionNumber;
            }

            set
            {
                m_PositionNumber = value;
            }
        }

        public GameObject TargetTree
        {
            get
            {
                return m_TargetTree;
            }
            set
            {
                m_TargetTree = value;
            }
        }

        public void Start()
        {
            m_IsActive = true;
        }

        public void Stop()
        {
            m_IsActive = false;
        }

        public bool Active
        {
            get
            {
                return m_IsActive;
            }
        }
    }
    private const float COLOR_CHANGE_DURATION = 3f;
    public static WaveManager m_Instance; 
    public int MinimumWaitBetweenWaves = 5;
    
    private Texture m_DefaultTreeTexture, m_ActiveTreeTexture;
    [SerializeField]
    private List<Wave> m_Waves = new List<Wave>();
    [SerializeField]
    private float m_BaseSpawningSpeed = 2, m_HeavySpawningSpeed = 2, m_LightSpawningSpeed = 2;
    private int m_EnemyCount = 0;
    private bool m_IsGameFinished = false;

    [SerializeField] private AudioClip m_BattleTheme;
    
    public static WaveManager Instance
    {
        get
        {
            if (m_Instance == null)
                Debug.LogError("WaveManager has no instance.");

            return m_Instance;
        }
    }
    void OnEnable()
    {
        EventManager.AddListner("Enemy_Spawn", IncreaseMonsterNumber);
        EventManager.AddListner("Enemy_Died", DecreaseMonsterNumber);
        
    }

    void OnDisable()
    {
        EventManager.RemoveListner("Enemy_Spawn", IncreaseMonsterNumber);
        EventManager.RemoveListner("Enemy_Died", DecreaseMonsterNumber);
    }
    
    private void IncreaseMonsterNumber()
    {
        m_EnemyCount++;
        //Debug.Log("EnemyCount incremented :" + m_EnemyCount);
    }

    private void DecreaseMonsterNumber()
    {
        m_EnemyCount--;
        if(m_EnemyCount == 0 && !IsGameFinished)
        {
            //m_Waves[0].Stop();
            PrepareNextWave();

        }
        //Debug.Log("EnemyCount decremented :" + m_EnemyCount);
    }
    
    private void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        m_BaseSpawningSpeed = 1 / m_BaseSpawningSpeed;
        m_HeavySpawningSpeed = 1 / m_HeavySpawningSpeed;
        m_LightSpawningSpeed = 1 / m_LightSpawningSpeed;
        for (int i = 0; i < m_Waves.Count; i++)
        {
            m_Waves[i].PositionNumber = i + 1;
            switch (m_Waves[i].FocusedTree)
            {
                case Wave.PossibleTrees.FirstTree:
                    m_Waves[i].TargetTree = GameManager.Instance.TreeList[0];
                    break;
                case Wave.PossibleTrees.SecondTree:
                    m_Waves[i].TargetTree = GameManager.Instance.TreeList[1];
                    break;
                case Wave.PossibleTrees.ThirdTree:
                    m_Waves[i].TargetTree = GameManager.Instance.TreeList[2];
                    break;
            }
        }
        
        
        
        
            
        StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves, m_Waves[0].TargetTree));
        
    }

    void Update()
    {
        //Désuet!!
        /*
        bool t_IsWaveFinished = true;
        foreach(var spawner in m_Spawners)
        {
            //Debug.Log("/" + spawner.Finished + "/" + spawner.IsDefeated() + "/");
            t_IsWaveFinished = spawner.Finished && m_EnemyCount == 0;
            if (!t_IsWaveFinished)
                break;
        }
        Debug.Log(m_Waves.Count + " Vagues au total");
        if (t_IsWaveFinished && m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsHealing)
        {
            Debug.Log("Vague échouée, elle recommencera sous peu...");
            StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves));
        } else if(t_IsWaveFinished && !m_Waves[0].Active)
        {
            Debug.Log("La Vague #" + m_Waves[0].PositionNumber + " est terminée!");
            m_Waves.RemoveAt(0);
            if (m_Waves.Count > 0)
            {
                StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves));
            }
            else
            {
                GameManager.Instance.Victory();
            }
        }
        */

        
    }

    public void PrepareNextWave()
    {
        GameObject t_PreviousTree = m_Waves[0].TargetTree;
        if (GameManager.Instance.DarkModeActivated)
        {
            return;
        }
        if (m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsDead)
        {
            Debug.Log("Vague échouée, elle recommencera sous peu...");
            StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves, t_PreviousTree));
            StartCoroutine(GameManager.Instance.RegenerateTree(m_Waves[0].TargetTree));
        }
        else
        {
            m_Waves[0].Stop();
            GameManager.Instance.MusicPlayer.Stop();
            Debug.Log("La Vague #" + m_Waves[0].PositionNumber + " est terminée!");
            StartCoroutine(GameManager.Instance.RegenerateTree(m_Waves[0].TargetTree));
            if (m_Waves.Count > 1)
            {
                if (m_Waves[0].FocusedTree != m_Waves[1].FocusedTree)
                {
                    StartCoroutine(m_Waves[0].TargetTree.GetComponent<TreeHealth>().FadeTreeColor(m_Waves[0].TargetTree.GetComponent<TreeHealth>().UnfocusedColor, COLOR_CHANGE_DURATION));
                }
            }
            //m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", m_DefaultTreeTexture);
            m_Waves.RemoveAt(0);            
            if (m_Waves.Count > 0)
            {
                StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves, t_PreviousTree));
            }
            else
            {
                m_IsGameFinished = true;
                GameManager.Instance.Victory();
            }
        }
    }

    //La "prochaine" vague ou la vague active est TOUJOURS la première de la liste, puisque une vague complétée disparais et laisse la place à la deuxieme de la liste
    private IEnumerator WaitForNextWave(int a_Countdown, GameObject a_PreviousTree)
    {
        m_Waves[0].Start();
        
        //Debug.Log(m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsHurt);
        while (m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsHurt)
        {
            if(!m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsHealing)
            {
                StartCoroutine(GameManager.Instance.RegenerateTree(m_Waves[0].TargetTree));
            }
            Debug.Log("Attente de la guérison de l'arbre avant la vague #" + m_Waves[0].PositionNumber + "...");
            yield return new WaitForSecondsRealtime(1);
        }

        var t_TreeHealth = m_Waves[0].TargetTree.GetComponent<TreeHealth>();
        StartCoroutine(t_TreeHealth.FadeTreeColor(t_TreeHealth.FocusedColor, COLOR_CHANGE_DURATION));


        for (; a_Countdown > 0; a_Countdown--)
        {
            
            //m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.magenta);
            //Debug.Log(m_Waves[0].TargetTree);
            Debug.Log("La vague #" + m_Waves[0].PositionNumber + " commence dans : " + a_Countdown);
            yield return new WaitForSecondsRealtime(1);
        }
        /*
        if (m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsHurt)
        {
            yield return null;
        }
        */
        
        InitiateWave(m_Waves[0]);
        yield break;
        
    }

    

    private void InitiateWave(Wave a_CurrentWave)
    {
        //Jouer la musique de bataille
        GameManager.Instance.MusicPlayer.clip = m_BattleTheme;
        GameManager.Instance.MusicPlayer.loop = true;
        GameManager.Instance.MusicPlayer.volume = 0.2f;
        GameManager.Instance.MusicPlayer.Play();

        Debug.Log("La vague #" + a_CurrentWave.PositionNumber + " vient de commencer !");
        List<GameObject> t_Spawners = GameManager.Instance.EnemySpawners;
        int t_SkeletonsToSpawn = Mathf.FloorToInt(a_CurrentWave.NumberOfSkeletons / t_Spawners.Count);
        int t_ExtraSkeletons = a_CurrentWave.NumberOfSkeletons % t_Spawners.Count;
        int t_GolemsToSpawn = Mathf.FloorToInt(a_CurrentWave.NumberOfGolems / t_Spawners.Count);
        int t_ExtraGolems = a_CurrentWave.NumberOfGolems % t_Spawners.Count;
        int t_SwarmersToSpawn = Mathf.FloorToInt(a_CurrentWave.NumberOfSwarmers / t_Spawners.Count);
        int t_ExtraSwarmers = a_CurrentWave.NumberOfSwarmers % t_Spawners.Count;
        int t_RandomNumber = UnityEngine.Random.Range(0, t_Spawners.Count - 1);
        for(int i = 0; i < t_Spawners.Count; i++)
        {
            //TODO Remplacer le magic number 3 qui représente le délais avant le commencement de la boucle de spawning
            if(i == t_RandomNumber)
            {
                StartCoroutine(t_Spawners[i].GetComponent<SpawnerBounds>().SpawnEnemyLoop(t_SkeletonsToSpawn + t_ExtraSkeletons, t_GolemsToSpawn + t_ExtraGolems, t_SwarmersToSpawn + t_ExtraSwarmers,
                    m_BaseSpawningSpeed, m_HeavySpawningSpeed, m_LightSpawningSpeed, 3));             
            } else
            {
                StartCoroutine(t_Spawners[i].GetComponent<SpawnerBounds>().SpawnEnemyLoop(t_SkeletonsToSpawn, t_GolemsToSpawn, t_SwarmersToSpawn,
                    m_BaseSpawningSpeed, m_HeavySpawningSpeed, m_LightSpawningSpeed, 3));
            }
        }



        a_CurrentWave.Start();
      
        
    }

    

    public GameObject TargetTree
    {
        get
        {
            
            return m_Waves[0].TargetTree;
        }
    }

    public bool IsGameFinished
    {
        get
        {

            return m_IsGameFinished;
        }
    }

    /*
    public void AddSpawner(SpawnerBehavior a_SpawnerBehavior)
    {
        m_Spawners.Add(a_SpawnerBehavior);
    }

    public void RemoveSpawner(SpawnerBehavior a_SpawnerBehavior)
    {
        m_Spawners.Remove(a_SpawnerBehavior);
    }
    */

}