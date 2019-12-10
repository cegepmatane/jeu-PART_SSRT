using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public int maxHP;
    private int m_currentHP;
    private EnemyMovement m_EnemyMovement;

    // Start is called before the first frame update
    void Start()
    {
        m_EnemyMovement = gameObject.GetComponent<EnemyMovement>();
        m_currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_currentHP <= 0)
        {
            m_EnemyMovement.DeathSequence();
        }
    }

    public void TakeDamage(int a_DamageAmount)
    {
        m_currentHP -= a_DamageAmount;
        GetComponent<AudioSource>().volume = 0.5f;
        GetComponent<AudioSource>().PlayOneShot(GetComponent<EnemyMovement>().hurtSound);
    }
}
