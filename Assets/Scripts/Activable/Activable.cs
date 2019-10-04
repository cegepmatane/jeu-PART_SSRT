using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Activable : MonoBehaviour
{
    [SerializeField]
    private bool m_AutoPickup;
    [SerializeField]
    private KeyCode m_PickupButton;

    private int m_TriggerCount; //In case we have multiple Triggers for the object
    
    public int TriggerCount
    {
        get { return m_TriggerCount; }
        private set
        {
            m_TriggerCount = value;
            this.enabled = value > 0 && !m_AutoPickup; // Activate/disable the Update for key interaction
        }
    }


    private void Awake()
    {
        m_TriggerCount = 0;

        // Checks if the Pickupable has at least 1 Trigger
        bool t_HasTrigger = false;
        foreach (var t_Coll in GetComponentsInChildren<Collider>())
            t_HasTrigger |= t_Coll.isTrigger;
        if (!t_HasTrigger)
            Debug.LogError(gameObject.name + " is aa Activable object but has no Trigger.");

        // Ensure a button is set
        if(!m_AutoPickup && m_PickupButton == KeyCode.None)
            Debug.LogError(gameObject.name + " is an automatically Activable object but has no Button set.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure only the Player can trigger the object
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        m_TriggerCount++;

        if (m_AutoPickup)
            Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        // Ensure only the Player can trigger the object
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        m_TriggerCount--;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_PickupButton))
        {
            Activate();
        }
    }

    protected abstract void Activate();
}
