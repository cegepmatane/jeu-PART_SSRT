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

    private bool IsAttacking;
    private bool IsDying;
    public float damage = 1f;
    public float attackRate = 2f;

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        IsDying = false;
        IsAttacking = false;
        EventManager.TriggerEvent("Enemy_Spawn");
        //Tri pour que les ennemis se dirigent vers les arbres dans le bon ordre
        //Ce comportement est maintenant désuet et géré par le GameManager


        /* GameObject[] t_trees = GameObject.FindGameObjectsWithTag("Tree").OrderBy(go => go.name).ToArray();
        m_waypoints = new List<Transform>();
 
        foreach (var tree in t_trees)
        {
            m_waypoints.Add(tree.transform.GetChild(0));
        } */

        treeCollider = WaveManager.Instance.TargetTree.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if(WaveManager.Instance.TargetTree != null && !IsDying)
        {
            m_agent.SetDestination(WaveManager.Instance.TargetTree.transform.GetChild(0).position);
            //CheckMinDistance();
        }

        //Les ennemis ne peuvent plus changer de cible; La cible meure, ils meurent aussi, et la vague recommence quand la cible ressucite

        /*
        else if (UpdateWaypoint())
        {
            Debug.Log("Les ennemis changent de cible !");
        } 
        */

        if (WaveManager.Instance.TargetTree.GetComponent<TreeHealth>().IsDead && !IsDying)
        {
            
            m_agent.SetDestination(transform.position);
            DeathSequence();
        }
    }
    /*
    private bool UpdateWaypoint()
    {
        isAttacking = false;
        StopCoroutine("Attack");

       
        if (GameManager.Instance.Waypoints.Count == 0)
        {
            return false;
        }

        //Update du waypoint et du collider
        m_currentWaypoint = GameManager.Instance.Waypoints[0];
        treeCollider = m_currentWaypoint.GetComponentInParent<CapsuleCollider>();

        return true;
    }
    */

    private IEnumerator Attack()
    {
        while (IsAttacking)
        {
            treeCollider.gameObject.GetComponent<TreeHealth>().ApplyDamage(damage);
            yield return new WaitForSeconds(attackRate);                   
        }
    }

    public void DeathSequence()
    {
        IsAttacking = false;
        IsDying = true;
        StopCoroutine("Attack");
        StartCoroutine("Fade"); 
    }

    private IEnumerator Fade()
    {
        //Debug.Log("Un ennemi s'efface...");
        while(IsDying)
        {
            Color t_Color = GetComponent<MeshRenderer>().material.color;
            t_Color.a -= 0.008f;
            if(t_Color.a <= 0f)
            {
                IsDying = false;
                Die();
            }
            GetComponent<MeshRenderer>().material.color = t_Color;
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = t_Color;
            //Debug.Log("Alpha = " + this.GetComponent<MeshRenderer>().material.color.a);
            yield return null;
        }
        
        yield return null;
    }

    private void Die()
    {
        //Debug.Log("Ennemi décédé");
        EventManager.TriggerEvent("Enemy_Died");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Touche un arbre");
        if (collider == treeCollider && !IsDying)
        {
            IsAttacking = true;
            StartCoroutine("Attack");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider == treeCollider && !IsDying)
        {
            IsAttacking = false;
            StopCoroutine("Attack");
        }
    }

}
