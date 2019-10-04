using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent m_agent;

    public Transform treePosition;
    public Collider treeCollider;

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
        m_agent.SetDestination(treePosition.position);
        if (GameObject.Find("CylinderTree") == null)
        {
            Destroy(gameObject);
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == treeCollider)
        {
            isAttacking = true;
            StartCoroutine("Attack");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == treeCollider)
            isAttacking = false;
    }

}
