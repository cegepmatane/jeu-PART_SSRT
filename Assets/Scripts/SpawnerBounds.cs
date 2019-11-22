using UnityEngine;
using System.Collections;

public class SpawnerBounds : MonoBehaviour
{
    public GameObject[] Prefabs;
    public enum ItemTypeArray { MANAFLOWER, ENEMY, OTHER };
    public ItemTypeArray ItemType;
    public Transform Bound1, Bound2;
    public float RaycastLenght = 100;
    public int MaxTries = 10;
    public int InitialSpawnAmount = 0;
    public Transform ParentContainer;
    public LayerMask ValidLayers;
    public LayerMask InvalidLayers;
    



    private void Awake()
    {
        if(Prefabs.Length < 1)
        {
            Debug.LogError(gameObject.name + "does not have anything to spawn. Stopping.");
            this.enabled = false;
        }
    }

    private void Start()
    {   
        if(ItemType == ItemTypeArray.MANAFLOWER || ItemType == ItemTypeArray.OTHER)
        {
            RandomSpawn(InitialSpawnAmount);
        }
        
        GameManager.Instance.AddGenericSpawner(this.gameObject);
    }

    //Returns the quantity that was actually spawned
    public int RandomSpawn(int a_Qty)
    {
        int t_SpawnedQty = 0;

        RaycastHit t_Hit;
        bool t_DidHit;

        for (int i = 0; i < a_Qty; i++)
        {
            t_DidHit = false;

            for (int j = 0; j < MaxTries; j++)
            {
                //Random pos between bounds
                float t_PosX = Mathf.Lerp(Bound1.position.x, Bound2.position.x, Random.value);
                float t_PosZ = Mathf.Lerp(Bound1.position.z, Bound2.position.z, Random.value);

                t_DidHit = Physics.Raycast(new Vector3(t_PosX, transform.position.y, t_PosZ), Vector3.down, out t_Hit, RaycastLenght, ValidLayers | InvalidLayers);

                //If we had a hit and the hit is on a valid layer, proceed to spawning
                if (t_DidHit && ValidLayers == (ValidLayers | (1 << t_Hit.transform.gameObject.layer)))
                {
                    var t_ToSpawn = Prefabs[Random.Range(0, Prefabs.Length)];

                    GameObject t_SpawnedItem = Instantiate(t_ToSpawn, t_Hit.point, t_ToSpawn.transform.rotation, ParentContainer);
                    t_SpawnedQty++;
                    break;
                }
            }
        }

        Debug.Log(string.Format("{0} spawned {1}/{2} objects.", gameObject.name, t_SpawnedQty, a_Qty));

        return t_SpawnedQty;
    }

    //Il faut absolument que, dans la liste de prefab, que le skelete soit 1er, le golem 2e et le swarmer 3e
    //TODO: choix de type d'ennemi plus flexible
    public int FixedSpawn(int a_Qty, int a_Index)
    {
        int t_SpawnedQty = 0;

        RaycastHit t_Hit;
        bool t_DidHit;

        for (int i = 0; i < a_Qty; i++)
        {
            t_DidHit = false;

            for (int j = 0; j < MaxTries; j++)
            {
                //Random pos between bounds
                float t_PosX = Mathf.Lerp(Bound1.position.x, Bound2.position.x, Random.value);
                float t_PosZ = Mathf.Lerp(Bound1.position.z, Bound2.position.z, Random.value);

                t_DidHit = Physics.Raycast(new Vector3(t_PosX, transform.position.y, t_PosZ), Vector3.down, out t_Hit, RaycastLenght, ValidLayers | InvalidLayers);

                //If we had a hit and the hit is on a valid layer, proceed to spawning
                if (t_DidHit && ValidLayers == (ValidLayers | (1 << t_Hit.transform.gameObject.layer)))
                {
                    var t_ToSpawn = Prefabs[a_Index];

                    GameObject t_SpawnedItem = Instantiate(t_ToSpawn, t_Hit.point, t_ToSpawn.transform.rotation, ParentContainer);
                    if (WaveManager.Instance.TargetTree != null)
                    {
                        //t_EnemyMovement.m_currentWaypoint = TargetTree.transform.GetChild(0);
                        t_SpawnedItem.GetComponent<EnemyMovement>().treeCollider = WaveManager.Instance.TargetTree.GetComponent<Collider>();
                    }
                    t_SpawnedQty++;
                    break;
                }
            }
        }

        //Debug.Log(string.Format("{0} spawned {1}/{2} objects.", gameObject.name, t_SpawnedQty, a_Qty));

        return t_SpawnedQty;
    }
    public IEnumerator SpawnEnemyLoop(int a_BaseCount, int a_HeavyCount, int a_LightCount, int a_Interval, int a_Delay)
    {
       

        // Wait for the delivery delay.
        yield return new WaitForSeconds(a_Delay);
        //Debug.Log("AAA");
        for (; a_BaseCount > 0; a_BaseCount--)
        {
            FixedSpawn(1, 0);

            yield return new WaitForSeconds(a_Interval);
        }

        for (; a_HeavyCount > 0; a_HeavyCount--)
        {

            FixedSpawn(1, 1);

            yield return new WaitForSeconds(a_Interval);
        }

        for (; a_LightCount > 0; a_LightCount--)
        {

            FixedSpawn(1, 2);

            yield return new WaitForSeconds(a_Interval / 2.9f);
        }
        
    }
}
