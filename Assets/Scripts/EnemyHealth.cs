using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public int m_MaxHP;
    private EnemyMovement m_EnemyMovement;

    // Start is called before the first frame update
    void Start()
    {
        m_EnemyMovement = gameObject.GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_MaxHP <= 0)
        {
            m_EnemyMovement.DeathSequence();
        }
    }

    void TakeDamage(int a_DamageAmount)
    {
        m_MaxHP -= a_DamageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Spike")
        {
            TakeDamage(50);
            Destroy(other.gameObject);
        }
    }
}
