using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class ShadowEnemyController : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private Animator m_Animator;
    private enum PhaseArray { APPEARING, AGGRESSIVE, DISAPPEARING};
    private PhaseArray m_CurrentPhase;
    [SerializeField]
    private float m_MaxSpeed = 5f, m_Acceleration = 0.01f;
    private float m_MovementSpeed = 0f;

    public AudioClip[] sounds;
    private AudioClip m_SelectedSound;
    private float m_NextSoundTime;
    private float m_LastSoundTime;


    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        if (GetComponent<Animator>() != null)
        {
            m_Animator = GetComponent<Animator>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddShadowEntity(gameObject);
        m_agent.speed = m_MovementSpeed;
        Color t_Color = GetComponent<MeshRenderer>().material.color;
        t_Color.a = 0f;
        GetComponent<MeshRenderer>().material.color = t_Color;
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = t_Color;
        AppearingSequence();
    }

    private void Update()
    {
        if (m_LastSoundTime > m_NextSoundTime)
        {
            //Jouer le son de l'ennemi (pour l'ambiance)
            GetComponent<AudioSource>().PlayOneShot(m_SelectedSound);
            if (sounds.Length > 0)
            {
                m_SelectedSound = sounds[(int)Random.Range(0, sounds.Length)];
                m_NextSoundTime = Random.Range(5, 10);
            }

            m_LastSoundTime = 0f;
        }
        m_LastSoundTime += Time.deltaTime;

        if (m_CurrentPhase == PhaseArray.AGGRESSIVE) 
        {
            m_agent.SetDestination(GameManager.Instance.Player.transform.position);
            if (m_MovementSpeed != m_MaxSpeed)
            {
                float t_SpeedDifference = m_MaxSpeed - m_MovementSpeed;
                m_MovementSpeed += (t_SpeedDifference > m_Acceleration) ? m_Acceleration : t_SpeedDifference;
            }
        }

        m_agent.speed = m_MovementSpeed;   
    }

    public void AppearingSequence()
    {
        
        
        m_CurrentPhase = PhaseArray.APPEARING;
        StartCoroutine(Fade());
        

    }

    public void AggressiveSequence()
    {
        m_CurrentPhase = PhaseArray.AGGRESSIVE;
        

    }

    public void DisappearingSequence()
    {
        m_CurrentPhase = PhaseArray.DISAPPEARING;
        Vector3 t_OppositeDirection = -GameManager.Instance.Player.transform.position.normalized;
        
        
        m_MovementSpeed = 1f;
        m_agent.SetDestination(t_OppositeDirection);
        m_agent.speed = m_MovementSpeed;
        StartCoroutine(Fade());
        

    }

    private IEnumerator Fade()
    {
        float t_FadeDirection = 0f;
        if(m_CurrentPhase == PhaseArray.APPEARING)
        {
            t_FadeDirection = -1f;
        } else if (m_CurrentPhase == PhaseArray.DISAPPEARING)
        {
            t_FadeDirection = 1f;
        }
        while (m_CurrentPhase == PhaseArray.APPEARING || m_CurrentPhase == PhaseArray.DISAPPEARING)
        {
            Color t_Color = GetComponent<MeshRenderer>().material.color;
            t_Color.a -= 0.004f * t_FadeDirection;
           
            if (t_Color.a <= 0f)
            {
                Die();
            } else if(t_Color.a >= 1f)
            {
                t_Color.a = 1.0f;
                AggressiveSequence();
            }
            GetComponent<MeshRenderer>().material.color = t_Color;
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = t_Color;
            yield return null;
        }
        yield return null;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    

    private void OnTriggerExit(Collider collider)
    {
        
    }

}
