using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private Animator m_Animator;
    //public List<Transform> m_waypoints;
    //Le currentWaypoint correspond au Transform de l'enfant "basePosition" dans chaque arbre
    //public Transform m_currentWaypoint;
    public Collider treeCollider;
    public enum EnemyTypeEnum { BASIC, SKELETAL, WONDERWALL, BIGGIE};
    public EnemyTypeEnum EnemyType = EnemyTypeEnum.BASIC;
    private bool IsAttacking;
    private bool IsDying;
    [SerializeField]
    private Material m_AlternateMaterial;
    [SerializeField]
    public float Damage = 1f, AttackRate = 2f;
    private Vector3 m_OriginalPos;
    private float m_LastPosUpdateTime;
    private Vector3 m_LastUpdatedPos;

    public AudioClip[] sounds;
    public AudioClip hurtSound;
    private AudioClip m_SelectedSound;
    private float m_NextSoundTime;
    private float m_LastSoundTime;

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_OriginalPos = transform.position;
        m_LastUpdatedPos = transform.position;
        m_LastPosUpdateTime = 0;
        if(sounds.Length > 0)
        {
            m_SelectedSound = sounds[(int)Random.Range(0, sounds.Length)];
        }
        
        m_LastSoundTime = 0f;
        m_NextSoundTime = Random.Range(5, 10);

        IsDying = false;
        IsAttacking = false;
        EventManager.TriggerEvent("Enemy_Spawn");

        //Pour que les ennemis ne rentre pas dans l'arbre
        m_agent.stoppingDistance = WaveManager.Instance.TargetTree.GetComponent<CapsuleCollider>().radius;
        m_agent.autoBraking = true;

        treeCollider = WaveManager.Instance.TargetTree.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {

        if(m_LastSoundTime > m_NextSoundTime)
        {
            //Jouer le son de l'ennemi (pour l'ambiance)
            GetComponent<AudioSource>().PlayOneShot(m_SelectedSound);
            if (sounds.Length > 0)
            {
                m_SelectedSound = sounds[(int)Random.Range(1, sounds.Length - 1)];
                m_NextSoundTime = Random.Range(5, 10);
            }

            m_LastSoundTime = 0f;
        }
        m_LastSoundTime += Time.deltaTime;

        if (m_Animator)
        {
            if (m_agent.velocity != new Vector3(0, 0, 0))
            {
                m_Animator.SetBool("IsMoving", true);
            } else
            {
                m_Animator.SetBool("IsMoving", false);
            }
            
        }
        if (!WaveManager.Instance.IsGameFinished)
        {
            if (WaveManager.Instance.TargetTree != null && !IsDying && m_agent.enabled)
            {
                if (IsAttacking)
                {
                    m_agent.SetDestination(transform.position);
                }
                else
                {
                    if (!m_agent.isOnNavMesh)
                        DeathSequence();
                    //Si l'ennemi se coince dans la NavMesh
                    //Sauvegarde la position de l'ennemi il y a 5 secondes
                    else if (m_LastPosUpdateTime > 5f)
                    {
                        //Warp à sa position initiale si il "n'a pas bougé" durant les 5 dernières secondes
                        if (Vector3.Distance(m_LastUpdatedPos, transform.position) < 0.5f)
                            m_agent.Warp(m_OriginalPos);

                        m_LastPosUpdateTime = 0f;
                        m_LastUpdatedPos = transform.position;
                    }

                    m_agent.SetDestination(WaveManager.Instance.TargetTree.transform.GetChild(0).position);
                }
                m_LastPosUpdateTime += Time.deltaTime;
            }

            //Les ennemis ne peuvent plus changer de cible; La cible meure, ils meurent aussi, et la vague recommence quand la cible ressucite

            if (WaveManager.Instance.TargetTree.GetComponent<TreeHealth>().IsDead && !IsDying)
            {
                DeathSequence();
            }
        }
    }

    private IEnumerator Attack()
    {
        while (IsAttacking)
        {
            treeCollider.gameObject.GetComponent<TreeHealth>().ApplyDamage(Damage);
            if (m_Animator)
            {
                m_Animator.SetTrigger("TriggerAttack");
            }
            yield return new WaitForSeconds(AttackRate);                   
        }
    }

    public void DeathSequence()
    {
        if (!IsDying)
        {
            IsAttacking = false;
            IsDying = true;
            m_agent.isStopped = true;
            if (m_Animator)
            {
                m_Animator.SetTrigger("TriggerDeath");
            }
            StopCoroutine("Attack");
            StartCoroutine("Fade");
        }
    }

    private IEnumerator Fade()
    {
        //Debug.Log("Un ennemi s'efface...");
        if (EnemyType == EnemyTypeEnum.BASIC)
        {
            while (IsDying)
            {
                Material t_Mat1 = GetComponent<MeshRenderer>().material;
                Material t_Mat2 = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                Color t_Color = GetComponent<MeshRenderer>().material.color;
                t_Color.a -= 0.01f;
                if (t_Color.a <= 0f)
                {

                    Die();
                }
                t_Mat1.color = t_Color;
                t_Mat2.color = t_Color;
                //Debug.Log("Alpha = " + this.GetComponent<MeshRenderer>().material.color.a);
                yield return null;
            }
        } else if (EnemyType == EnemyTypeEnum.SKELETAL || EnemyType == EnemyTypeEnum.WONDERWALL || EnemyType == EnemyTypeEnum.BIGGIE)
        {
            gameObject.transform.GetComponentInChildren<SkinnedMeshRenderer>().material = m_AlternateMaterial;
            Material t_Mat = gameObject.transform.GetComponentInChildren<SkinnedMeshRenderer>().material;
            while (IsDying)
            {
                Color t_Color = t_Mat.color;
                t_Color.a -= 0.01f;
                if (t_Color.a <= 0f)
                {
                    Die();
                }
                t_Mat.color = t_Color;   
                
                yield return null; 
            }
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
