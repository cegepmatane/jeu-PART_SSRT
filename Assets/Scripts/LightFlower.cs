using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFlower : Activable
{
    [SerializeField]
    private int m_DarknessDecrease = 20;
    private enum PhaseArray { APPEARING, DISAPPEARING };
    private PhaseArray m_CurrentPhase;
    private GameObject m_player;

    private void Start()
    {
        Debug.LogWarning("LIGH FLOWER SPAWNED");
        GameManager.Instance.AddLightFlower(gameObject);

        Color t_Color = GetComponent<MeshRenderer>().material.color;
        t_Color.a = 0f;
        GetComponent<MeshRenderer>().material.color = t_Color;
        AppearingSequence();
    }

    public void AppearingSequence()
    {
        m_CurrentPhase = PhaseArray.APPEARING;
        StartCoroutine(Fade());
    }

    public void DisappearingSequence()
    {
        m_CurrentPhase = PhaseArray.DISAPPEARING;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float t_FadeDirection = 0f;
        if (m_CurrentPhase == PhaseArray.APPEARING)
        {
            t_FadeDirection = -1f;
        }
        else if (m_CurrentPhase == PhaseArray.DISAPPEARING)
        {
            t_FadeDirection = 1f;
        }
        while (m_CurrentPhase == PhaseArray.APPEARING || m_CurrentPhase == PhaseArray.DISAPPEARING)
        {
            Color t_Color = GetComponent<MeshRenderer>().material.color;
            t_Color.a -= 0.001f * t_FadeDirection;

            if (t_Color.a <= 0f)
            {
                Die();
            }
            else if (t_Color.a >= 1f)
            {
                t_Color.a = 1.0f;
            }
            GetComponent<MeshRenderer>().material.color = t_Color;
            yield return null;
        }
        yield return null;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    protected override void Activate()
    {
        m_player = GameManager.Instance.Player;
        DecreaseDarkness();
    }

    private void DecreaseDarkness()
    {
        if (m_player.GetComponent<PlayerAbilities>().decreaseDarkness(m_DarknessDecrease))
        {
            Destroy(gameObject);

        }


    }
}

