using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private float m_Lifetime = 2f;
    [SerializeField] private float m_ShockwaveRadius = 5f;
    [SerializeField] private int m_ShockwaveDamage = 10;
    private Vector3 m_RepulsionDirection;
    [SerializeField] private float m_RepulsionForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
        ApplyShockwaveForce();

        Destroy(gameObject, m_Lifetime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ShockwaveRadius);
    }

    private void ApplyShockwaveForce()
    {
        Collider[] t_TouchedObjectsColliders = Physics.OverlapSphere(transform.position, m_ShockwaveRadius);
        foreach (Collider col in t_TouchedObjectsColliders)
        {
            if (col.gameObject.GetComponent<EnemyHealth>() != null)
            {
                //Appliquer les degats
                col.gameObject.GetComponent<EnemyHealth>().TakeDamage(m_ShockwaveDamage);

                //Calcul de la direction de répulsion
                m_RepulsionDirection = (transform.position - col.gameObject.transform.position).normalized;

                //Désactiver le NavMeshAgent
                col.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                //Appliquer la répulsion
                col.gameObject.GetComponent<Rigidbody>().AddForce((-m_RepulsionDirection) * m_RepulsionForce, ForceMode.Impulse);
                //col.gameObject.GetComponent<NavMeshAgent>().velocity = -m_RepulsionDirection * m_RepulsionForce;

                //Réactiver le NavMeshAgent
                StartCoroutine("ActivateAgent", col.gameObject);

                if (col.gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
                {
                    col.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.yellow;
                }
                else
                {
                    col.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }

            }
        }

    }

    IEnumerator ActivateAgent(GameObject a_enemy)
    {
        yield return new WaitForSeconds(0.5f);

        a_enemy.GetComponent<NavMeshAgent>().enabled = true;
        Vector3 t_Target = GameManager.Instance.Player.transform.position;

        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, t_Target, NavMesh.AllAreas, path))
        {
            bool isvalid = true;

            if (path.status != NavMeshPathStatus.PathComplete)
                isvalid = false;

            if (isvalid)
            {
                a_enemy.GetComponent<NavMeshAgent>().Warp(a_enemy.transform.position);
                a_enemy.GetComponent<NavMeshAgent>().SetDestination(t_Target);
                a_enemy.GetComponent<Rigidbody>().isKinematic = true;
            }
                
        }

        StopCoroutine("ActivateAgent");
    }
}
