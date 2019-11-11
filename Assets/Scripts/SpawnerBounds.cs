using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBounds : MonoBehaviour
{
    public GameObject Prefab;

    public Transform Bound1, Bound2;
    public float RaycastLenght = 100;
    public int MaxTries = 10;
    public int InitialSpawnAmount = 20;
    private GameObject m_ManaFlowerContainer;
    public LayerMask HitLayers;

    private void Start()
    {
        m_ManaFlowerContainer = GameObject.Find("ManaFlowers");
        for(int i = 0; i < InitialSpawnAmount; i++)
        {
            Spawn();
        }
    }

    public bool Spawn()
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
                GameObject t_SpawnedItem = Instantiate(Prefab, t_Hit.point, Prefab.transform.rotation);
                if(t_SpawnedItem.GetComponent<ManaFlower>() != null)
                {
                    t_SpawnedItem.transform.parent = m_ManaFlowerContainer.transform;
                }
                break;
            }
        }

        if (!t_DidHit)
            Debug.Log(gameObject.name + " tried to Spawn, but did not find a valid location");


        return t_DidHit;
    }
}
