using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent m_agent;

    public Transform treePosition;
    public Collider treeCollider;

    private int m_Health;
    private bool isAttacking = false;
    public float damage = 1f;
    public float attackRate = 2f;

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(treePosition != null)
        {
            m_agent.SetDestination(treePosition.position);
        } else {
            m_agent.SetDestination(transform.position);
            DeathSequence();
        }
    }


    private IEnumerator Attack()
    {
        while (isAttacking)
        {
            treeCollider.gameObject.GetComponent<TreeHealth>().ApplyDamage(damage);
            yield return new WaitForSeconds(attackRate);                   
        }
    }

    public void DeathSequence()
    {
        StopCoroutine("Attack");
        StartCoroutine("Fade");
    }

    private IEnumerator Fade()
    {
        
        Color t_Color = GetComponent<MeshRenderer>().material.color;
        t_Color.a -= 0.008f;
        if(t_Color.a <= 0f)
        {
            Die();
        }
        GetComponent<MeshRenderer>().material.color = t_Color;
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = t_Color;
        //Debug.Log("Alpha = " + this.GetComponent<MeshRenderer>().material.color.a);
        
        yield return null;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == treeCollider)
        {
            isAttacking = true;
            StartCoroutine("Attack");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider == treeCollider)
        {
            isAttacking = false;
        }
    }

}
