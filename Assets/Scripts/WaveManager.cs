using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public class Wave
    {
        private float m_Difficulty;
        private bool m_IsActive = false;
        public Wave(float a_Difficulty)
        {
            this.m_Difficulty = a_Difficulty;
        }

        public float Difficulty
        {
            get
            {
                return m_Difficulty;
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

    public int WaveCount;
    private List<SpawnerBehavior> m_Spawners = new List<SpawnerBehavior>();
    private List<Wave> m_Waves = new List<Wave>();
    //TODO influencer m_SpawningSpeed au fil du temps
    private int m_SpawningSpeed = 2;


    public static WaveManager Instance
    {
        get
        {
            if (m_Instance == null)
                Debug.LogError("WaveManager has no instance.");

            return m_Instance;
        }
    }

    private void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        //Création une par une des vagues avec une difficulté incrémentée
        float t_ScalingDifficulty = 4f;
        
        for(int i = 0; i < WaveCount; i++)
        {
            m_Waves.Add(new Wave(1 + t_ScalingDifficulty));
            t_ScalingDifficulty += 2;
        }
        m_Waves[0].Start();
        InitiateWave(m_Waves[0]);
        
    }

    void Update()
    {
        bool t_IsWaveFinished = true;
        foreach(var spawner in m_Spawners)
        {
            t_IsWaveFinished = spawner.Finished && spawner.IsDefeated();
            if (!t_IsWaveFinished)
                break;
        }
        
        if (t_IsWaveFinished)
        {
            m_Waves.RemoveAt(0);
        }

        if(m_Waves.Count > 0 && !m_Waves[0].Active)
        {
            m_Waves[0].Start();
            InitiateWave(m_Waves[0]);
        }
    }
    
    private void InitiateWave(Wave a_CurrentWave)
    {  
        //Le nombre d'ennemy que chaque spawner aura à spawner est défini par l'arroundissement de la difficultée divisée par le nombre de spawners présents
        int t_EnemyPerSpawn = (int) Mathf.Round(a_CurrentWave.Difficulty/m_Spawners.Count);
        foreach(var spawner in m_Spawners)
        {
            Debug.Log("Spawner Initié!");
            spawner.BeginWave(t_EnemyPerSpawn, m_SpawningSpeed);
        }
        //StartCoroutine(PlayingWave(a_CurrentWave, t_EnemyPerSpawn));
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


