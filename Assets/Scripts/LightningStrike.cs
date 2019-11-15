using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float travellingSpeed = 20f;
    public float lightningRange = 50f;
    public int lightningDamage = 75;
    private bool m_Trajectoire;
    private RaycastHit m_Hit;

    void Start()
    {
        DigitalRuby.LightningBolt.LightningBoltScript t_OriginalScript = GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();

        t_OriginalScript.StartObject = null;
        t_OriginalScript.EndObject = null;
        t_OriginalScript.StartPosition = transform.position;

        if(Physics.Raycast(transform.position, transform.forward, out m_Hit, lightningRange))
            t_OriginalScript.EndPosition = m_Hit.point;
        else
            t_OriginalScript.EndPosition = transform.position + (transform.forward * lightningRange);

        Destroy(gameObject, 1f);
    }

    private void Update()
    {
        float speed = travellingSpeed * Time.deltaTime;
        //transform.position = GameManager.Instance.Player.GetComponentInChildren<SpellEventManager>().rightHand.transform.position;
        //transform.position += transform.forward * speed;

    }
}
