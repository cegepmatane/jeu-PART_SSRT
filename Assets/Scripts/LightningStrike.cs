using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class LightningStrike : MonoBehaviour
{
    public float travellingSpeed = 20f;
    public float lightningRange = 50f;
    public int lightningDamage = 75;

    [SerializeField] private LayerMask m_LayersHit;
    private bool m_Trajectoire;
    private RaycastHit m_Hit;

    [SerializeField] private GameObject m_Sparks;

    void Start()
    {
        GameObject t_Sparks = Instantiate(m_Sparks, transform.position, Quaternion.LookRotation(transform.forward));

        LightningBoltScript t_OriginalScript = GetComponent<LightningBoltScript>();
        t_OriginalScript.StartObject = null;
        t_OriginalScript.EndObject = null;
        t_OriginalScript.StartPosition = transform.position;

        //On définit la position ou l'éclair s'arrête
        if(Physics.Raycast(transform.position, transform.forward, out m_Hit, lightningRange, m_LayersHit))
        {
            t_OriginalScript.EndPosition = m_Hit.point;
        }
        else
            t_OriginalScript.EndPosition = transform.position + (transform.forward * lightningRange);


        BoxCollider t_LightningCollider = GetComponent<BoxCollider>();
        //On étire le collider sur la longueur de l'éclair
        t_LightningCollider.center = Vector3.forward * (t_OriginalScript.StartPosition - t_OriginalScript.EndPosition).magnitude / 2;
        t_LightningCollider.size = new Vector3(0.25f, 0.25f, (t_OriginalScript.StartPosition - t_OriginalScript.EndPosition).magnitude);

        Destroy(t_Sparks, 0.5f);
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyHealth>() != null)
        {
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(lightningDamage);
            if (other.gameObject.GetComponent<EnemyMovement>().EnemyType == EnemyMovement.EnemyTypeEnum.SKELETAL)
            {
                other.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.magenta;
            }
            else
            {
                other.gameObject.GetComponent<MeshRenderer>().material.color = Color.magenta;
            }
        }
    }
}
