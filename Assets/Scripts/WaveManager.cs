using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int WaveCount;
    private List<GameObject> m_Spawners;
    private List<Wave> m_Waves;
    //TODO influencer m_SpawningSpeed au fil du temps
    private int m_SpawningSpeed = 2;

    private void Awake()
    {
        //Reçois et converti le tableau de tous les spawners en liste
        m_Spawners = new List<GameObject>();
        m_Waves = new List<Wave>();
        GameObject[] t_Spawners =  GameObject.FindGameObjectsWithTag("Spawner");
        foreach(GameObject spawner in t_Spawners)
        {
            m_Spawners.Add(spawner);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Création une par une des vagues avec une difficulté incrémentée
        float t_ScalingDifficulty = 4f;
        
        for(int i = 0; i < WaveCount; i++)
        {
            m_Waves.Add(new Wave(1 + t_ScalingDifficulty));
            t_ScalingDifficulty += 2;
        }
        m_Waves[0].Begin();
        InitiateWave(m_Waves[0]);
        
    }
    // Update is called once per frame
    void Update()
    {
        bool t_IsWaveFinished = false;
        foreach(GameObject spawner in m_Spawners)
        {
            if(spawner.GetComponent<SpawnerBehavior>().IsFinished() && spawner.GetComponent<SpawnerBehavior>().IsDefeated()){ t_IsWaveFinished = true;}
            else{t_IsWaveFinished = false;}
        }
        if(t_IsWaveFinished){m_Waves.RemoveAt(0);}

        if(!m_Waves[0].IsActive() && m_Waves != null)
        {
            m_Waves[0].Begin();
            InitiateWave(m_Waves[0]);
        }
    }
    
    private void InitiateWave(Wave a_CurrentWave)
    {  
        //Le nombre d'ennemy que chaque spawner aura à spawner est défini par l'arroundissement de la difficultée divisée par le nombre de spawners présents
        int t_EnemyPerSpawn = (int) Mathf.Round(a_CurrentWave.GetDifficulty()/m_Spawners.Count);
        foreach(GameObject spawner in m_Spawners)
        {
            Debug.Log("Spawner Initié!");
            spawner.GetComponent<SpawnerBehavior>().BeginWave(t_EnemyPerSpawn, m_SpawningSpeed);
        }
        //StartCoroutine(PlayingWave(a_CurrentWave, t_EnemyPerSpawn));
    } 


    public class Wave
    {
        private float m_Difficulty;
        private bool m_IsActive = false;
        public Wave(float a_Difficulty)
        {
            this.m_Difficulty = a_Difficulty;
        }

        public float GetDifficulty()
        {
            return m_Difficulty;
        }

        public void Begin()
        {
            m_IsActive = true;
        }

        public void Finish()
        {
            m_IsActive = false;
        }

        public bool IsActive()
        {
            return m_IsActive;
        }
    }
    
}


