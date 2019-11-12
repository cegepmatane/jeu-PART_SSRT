using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEventManager : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    private Vector3 m_SpellPosition;

    private void CastFireball()
    {
        //La position du sort est la main droite
        m_SpellPosition = rightHand.transform.position;
        GameManager.Instance.Player.GetComponent<PlayerAbilities>().InstantiateSpell(m_SpellPosition);
    }

    private void CastLaserbeam()
    {

    }

    private void CastLighting()
    {

    }

    private void CastShockwave()
    {

    }
}
