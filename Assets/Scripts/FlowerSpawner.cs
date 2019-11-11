using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public List<GameObject> FlowerVariants;

    public Transform Bound1, Bound2;
    public float RaycastLenght = 100;
    public int MaxTries = 10;
    public int InitialSpawnAmount = 20;
    public LayerMask HitLayers;
    [SerializeField]
    private GameObject m_ManaFlowerContainer;
    public static FlowerSpawner m_Instance;
    public static FlowerSpawner Instance
    {
        get
        {
            if (m_Instance == null)
                Debug.LogError("FlowerSpawner has no instance.");

            return m_Instance;
        }
    }

    void Start()
    {

        for (int i = 0; i < InitialSpawnAmount; i++)
        {
            Spawn();
        }
    }

    virtual public bool Spawn()
    {
        RaycastHit t_Hit;
        bool t_DidHit = false; ;

        for (int i = 0; i < MaxTries; i++)
        {
            float t_PosX = Mathf.Lerp(Bound1.position.x, Bound2.position.x, Random.value);
            float t_PosZ = Mathf.Lerp(Bound1.position.z, Bound2.position.z, Random.value);


            t_DidHit = Physics.Raycast(new Vector3(t_PosX, transform.position.y, t_PosZ), Vector3.down, out t_Hit, RaycastLenght, HitLayers);

            if (t_DidHit)
            {
                int t_RandomInt = Random.Range(1, FlowerVariants.Count);
                GameObject t_SpawnedItem = Instantiate(FlowerVariants[t_RandomInt], t_Hit.point, FlowerVariants[t_RandomInt].transform.rotation);
                t_SpawnedItem.transform.parent = m_ManaFlowerContainer.transform;
                break;
            }
        }

        if (!t_DidHit)
            Debug.Log(gameObject.name + " tried to Spawn, but did not find a valid location");


        return t_DidHit;
    }
}
