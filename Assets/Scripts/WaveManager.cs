using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public class Wave
    {
        
        private int m_PositionNumber;
        private float m_Difficulty;
        private string m_WaveType;
        private GameObject m_TargetTree;
        private bool m_IsActive = false;

        public Wave(float a_Difficulty, GameObject a_TargetTree, int a_PositionNumber, string a_WaveType)
        {
            this.m_Difficulty = a_Difficulty;
            this.m_TargetTree = a_TargetTree;
            this.m_PositionNumber = a_PositionNumber;
            this.m_WaveType = a_WaveType;
        }

        public float Difficulty
        {
            get
            {
                return m_Difficulty;
            }
        }

        public int PositionNumber
        {
            get
            {
                return m_PositionNumber;
            }
        }

        public GameObject TargetTree
        {
            get
            {
                return m_TargetTree;
            }
        }

        public string WaveType
        {
            get
            {
                return m_WaveType;
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

    public static WaveManager m_Instance;
    //WaveCount doit toujours être un multiple de 3!
    public int WaveCount;
    public int MinimumWaitBetweenWaves = 5;

    [SerializeField]
    private Texture m_DefaultTreeTexture, m_ActiveTreeTexture;
    private List<SpawnerBehavior> m_Spawners = new List<SpawnerBehavior>();
    private List<Wave> m_Waves = new List<Wave>();
    
    //TODO influencer m_SpawningSpeed au fil du temps
    private int m_SpawningSpeed = 2;
    private int m_EnemyCount = 0;

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
        if(m_EnemyCount == 0)
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
        //Création une par une des vagues avec une difficulté incrémentée ET un changement de l'arbre cible à chaque tier du nombre de vague total
        float t_ScalingDifficulty = 4f;
        
        for(int i = 0; i <= WaveCount; i++)
        {
            if(i + 1 <= WaveCount / 3)
            {
                m_Waves.Add(new Wave(1 + t_ScalingDifficulty, GameManager.Instance.TreeList[0], i + 1, "low-tier"));
                t_ScalingDifficulty += 3;
            } else if (i + 1 <= 2*(WaveCount / 3))
            {
                m_Waves.Add(new Wave(1 + t_ScalingDifficulty, GameManager.Instance.TreeList[1], i + 1, "mid-tier"));
                t_ScalingDifficulty += 3;
            }
            else if(i + 1 > 2 * (WaveCount / 3))
            {
                m_Waves.Add(new Wave(1 + t_ScalingDifficulty, GameManager.Instance.TreeList[2], i + 1, "high-tier"));
                t_ScalingDifficulty += 3;
            }
            else
            {
                Debug.LogError("Le nombre de Vague n'est pas divisible par 3!");
            }
            //Debug.Log(m_Waves[m_Waves.Count - 1].TargetTree);
            



        }
        m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.magenta);
        StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves));
        
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

    private void PrepareNextWave()
    {
        if (GameManager.Instance.DarkModeActivated)
        {
            return;
        }
        if (m_Waves[0].TargetTree.GetComponent<TreeHealth>().IsDead)
        {
            Debug.Log("Vague échouée, elle recommencera sous peu...");
            StartCoroutine(WaitForNextWave(MinimumWaitBetweenWaves));
        }
        else
        {
            m_Waves[0].Stop();
            Debug.Log("La Vague #" + m_Waves[0].PositionNumber + " est terminée!");
            StartCoroutine(GameManager.Instance.RegenerateTree(m_Waves[0].TargetTree));
            //m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", m_DefaultTreeTexture);
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
    }

    //La "prochaine" vague ou la vague active est TOUJOURS la première de la liste, puisque une vague complétée disparais et laisse la place à la deuxieme de la liste
    private IEnumerator WaitForNextWave(int a_Countdown)
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
        for(; a_Countdown > 0; a_Countdown--)
        {
            m_Waves[0].TargetTree.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.magenta);
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
        Debug.Log("La vague #" + a_CurrentWave.PositionNumber + " vient de commencer !");
        //Le nombre d'enemy que chaque spawner aura à spawner est défini par l'arroundissement de la difficultée divisée par le nombre de spawners présents
        int t_BasicEnemyPerSpawn = 0;
        int t_HeavyEnemyPerSpawn = 0;
        int t_LightEnemyPerSpawn = 0;
        if (a_CurrentWave.WaveType == "low-tier")
        {
            t_BasicEnemyPerSpawn = (int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count);
            foreach (var spawner in m_Spawners)
            {
                spawner.BeginWave(t_BasicEnemyPerSpawn, t_HeavyEnemyPerSpawn, t_LightEnemyPerSpawn, m_SpawningSpeed);
            }
        } else if(a_CurrentWave.WaveType == "mid-tier")
        {
            t_BasicEnemyPerSpawn = ((int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count)) / 3 * 2;
            t_HeavyEnemyPerSpawn = ((int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count)) / 3;
            foreach (var spawner in m_Spawners)
            {
                spawner.BeginWave(t_BasicEnemyPerSpawn, t_HeavyEnemyPerSpawn, t_LightEnemyPerSpawn, m_SpawningSpeed);
            }
        }else if (a_CurrentWave.WaveType == "high-tier")
        {
            t_BasicEnemyPerSpawn = ((int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count)) / 3;
            t_HeavyEnemyPerSpawn = ((int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count)) / 3;
            t_LightEnemyPerSpawn = ((int)Mathf.Round(a_CurrentWave.Difficulty / m_Spawners.Count)) / 3;
            foreach (var spawner in m_Spawners)
            {
                spawner.BeginWave(t_BasicEnemyPerSpawn, t_HeavyEnemyPerSpawn, t_LightEnemyPerSpawn, m_SpawningSpeed);
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

    public void AddSpawner(SpawnerBehavior a_SpawnerBehavior)
    {
        m_Spawners.Add(a_SpawnerBehavior);
    }

    public void RemoveSpawner(SpawnerBehavior a_SpawnerBehavior)
    {
        m_Spawners.Remove(a_SpawnerBehavior);
    }


}


