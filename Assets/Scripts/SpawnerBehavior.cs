using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    
    public int count = 1;
    public int interval = 1;
    public int delay = 1;

    [SerializeField]
    private GameObject Enemy, TargetTree;
    
    


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy(count, interval, delay));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {


    }

    public IEnumerator SpawnEnemy(int a_Count, int a_Interval, int a_Delay)
    {
        // Wait for the delivery delay.
        yield return new WaitForSeconds(a_Delay);
        Debug.Log("AAA");
        for (; a_Count > 0; a_Count--)
        {
            bool foundTile = false;
            
            GameObject t_Enemy = Instantiate(Enemy, this.gameObject.transform.position, Quaternion.identity);
            EnemyMovement t_EnemyMovement = t_Enemy.GetComponent<EnemyMovement>();
            t_EnemyMovement.treePosition = TargetTree.transform;
            t_EnemyMovement.treeCollider = TargetTree.GetComponent<Collider>();
            yield return new WaitForSeconds(a_Interval);
        }
    }
       
}
