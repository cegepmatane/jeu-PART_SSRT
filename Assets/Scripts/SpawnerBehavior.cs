using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    
    //public int count = 1;
    //public int interval = 1;
    //public int delay = 1;
    private bool m_IsFinished;
    //private List<GameObject> m_SpawnedEnemies;

    
    public GameObject BaseEnemy, HeavyEnemy, LightEnemy, TargetTree;



    private void Awake()
    {
        //WaveManager.Instance.AddSpawner(this);

        //m_SpawnedEnemies = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SpawnEnemy(count, interval, delay));
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void FixedUpdate()
    {


    }
    public bool Finished
    {
        get
        {
            return m_IsFinished;
        }
    }
    /*
    public bool IsDefeated()
    {
        bool t_IsDefeated = false;
        Debug.Log("Ennemis en circulation :" + m_SpawnedEnemies.Count);
        if(m_SpawnedEnemies.Count == 0){t_IsDefeated = true;}
        return t_IsDefeated;
    }
    */

    public void BeginWave(int a_BaseCount, int a_HeavyCount, int a_LightCount, int a_Interval)
    {
        StartCoroutine(SpawnEnemy(a_BaseCount, a_HeavyCount, a_LightCount, a_Interval, 3));
    }

    //a_Count représente le nombre d'ennemis à spawner pour ce spawner particulié
    //a_Interval représente le temps ENTRE chaque création d'un nouvel ennemi
    //a_Delay représente le délais AVANT de créer des ennemis, donc le temps entre l'arrivée des ennemis et le départ du jeu
    public IEnumerator SpawnEnemy(int a_BaseCount, int a_HeavyCount, int a_LightCount, int a_Interval, int a_Delay)
    {
        m_IsFinished = false;
        
        // Wait for the delivery delay.
        yield return new WaitForSeconds(a_Delay);
        //Debug.Log("AAA");
        for (; a_BaseCount > 0; a_BaseCount--)
        {
            
            GameObject t_Enemy = Instantiate(BaseEnemy, this.gameObject.transform.position, Quaternion.identity);
            //m_SpawnedEnemies.Add(t_Enemy);
            EnemyMovement t_EnemyMovement = t_Enemy.GetComponent<EnemyMovement>();
            if(TargetTree != null)
            {
                //t_EnemyMovement.m_currentWaypoint = TargetTree.transform.GetChild(0);
                t_EnemyMovement.treeCollider = TargetTree.GetComponent<Collider>();
            }
            
            yield return new WaitForSeconds(a_Interval);
        }

        for (; a_HeavyCount > 0; a_HeavyCount--)
        {

            GameObject t_Enemy = Instantiate(HeavyEnemy, this.gameObject.transform.position, Quaternion.identity);
            //m_SpawnedEnemies.Add(t_Enemy);
            EnemyMovement t_EnemyMovement = t_Enemy.GetComponent<EnemyMovement>();
            if (TargetTree != null)
            {
                //t_EnemyMovement.m_currentWaypoint = TargetTree.transform.GetChild(0);
                t_EnemyMovement.treeCollider = TargetTree.GetComponent<Collider>();
            }

            yield return new WaitForSeconds(a_Interval);
        }

        for (; a_LightCount > 0; a_LightCount--)
        {

            GameObject t_Enemy1 = Instantiate(LightEnemy, this.gameObject.transform.position, Quaternion.identity);
            GameObject t_Enemy2 = Instantiate(LightEnemy, this.gameObject.transform.position + new Vector3(1,0,0), Quaternion.identity);
            GameObject t_Enemy3 = Instantiate(LightEnemy, this.gameObject.transform.position + new Vector3(0,0,1), Quaternion.identity);
            //m_SpawnedEnemies.Add(t_Enemy);
            EnemyMovement t_EnemyMovement1 = t_Enemy1.GetComponent<EnemyMovement>();
            EnemyMovement t_EnemyMovement2 = t_Enemy2.GetComponent<EnemyMovement>();
            EnemyMovement t_EnemyMovement3 = t_Enemy3.GetComponent<EnemyMovement>();
            if (TargetTree != null)
            {
                //t_EnemyMovement.m_currentWaypoint = TargetTree.transform.GetChild(0);
                t_EnemyMovement1.treeCollider = TargetTree.GetComponent<Collider>();
                t_EnemyMovement2.treeCollider = TargetTree.GetComponent<Collider>();
                t_EnemyMovement3.treeCollider = TargetTree.GetComponent<Collider>();
            }

            yield return new WaitForSeconds(a_Interval);
        }
        m_IsFinished = true;
    }
}
