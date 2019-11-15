using UnityEngine;

public class SpawnerBounds : MonoBehaviour
{
    public GameObject Prefab;

    public Transform Bound1, Bound2;
    public float RaycastLenght = 100;
    public int MaxTries = 10;
    public int InitialSpawnAmount = 0;
    public Transform ParentContainer;
    public LayerMask ValidLayers;
    public LayerMask InvalidLayers;


    private void Start()
    {
        Spawn(InitialSpawnAmount);
    }

    //Returns the quantity that was actually spawned
    public int Spawn(int a_Qty)
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
                    GameObject t_SpawnedItem = Instantiate(Prefab, t_Hit.point, Prefab.transform.rotation, ParentContainer);
                    t_SpawnedQty++;
                    break;
                }
            }
        }

        Debug.Log(string.Format("{0} spawned {1}/{2} {3}.", gameObject.name, t_SpawnedQty, a_Qty, Prefab.name));

        return t_SpawnedQty;
    }
}
