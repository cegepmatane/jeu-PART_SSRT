using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent m_agent;

    public List<Transform> m_waypoints;
    //Le currentWaypoint correspond au Transform de l'enfant "basePosition" dans chaque arbre
    public Transform m_currentWaypoint;
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
        //Tri pour que les ennemis se dirigent vers les arbres dans le bon ordre
        GameObject[] t_trees = GameObject.FindGameObjectsWithTag("Tree").OrderBy(go => go.name).ToArray();
        m_waypoints = new List<Transform>();
 
        foreach (var tree in t_trees)
        {
            m_waypoints.Add(tree.transform.GetChild(0));
        }
    }

    private void Update()
    {
        if(m_currentWaypoint != null)
        {
            m_agent.SetDestination(m_currentWaypoint.position);
            //CheckMinDistance();
        }
        else if (UpdateWaypoint())
        {
            Debug.Log("Les ennemis changent de cible !");
        }
        else
        {
            m_agent.SetDestination(transform.position);
            DeathSequence();
        }
    }

    private bool UpdateWaypoint()
    {
        isAttacking = false;
        StopCoroutine("Attack");

        m_waypoints.RemoveAt(0);
        if (m_waypoints.Count == 0)
        {
            return false;
        }

        //Update du waypoint et du collider
        m_currentWaypoint = m_waypoints[0];
        treeCollider = m_currentWaypoint.GetComponentInParent<CapsuleCollider>();

        return true;
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
        Die();
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
        Debug.Log("Touche un arbre");
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
