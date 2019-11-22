using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private float m_Lifetime = 2f;
    [SerializeField] private float m_ShockwaveRadius = 5f;
    [SerializeField] private int m_ShockwaveDamage = 10;
    private Vector3 m_RepulsionDirection;
    [SerializeField] private float m_RepulsionForce = 10000f;

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
                //Appliquer la répulsion
                col.gameObject.transform.Translate((m_RepulsionDirection + Vector3.up) * m_RepulsionForce);

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
}
